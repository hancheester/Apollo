using Autofac;
using Apollo.Core.Infrastructure.DependencyManagement;
using Apollo.Core.Logging;
using Apollo.Core.Model.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Apollo.Core.Services.Tasks
{
    public partial class Task
    {
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly ContainerManager _containerManager;
        private readonly ILogger _logger;
        private TaskFrequencyType? _frequency;
        private DateTime? _nextStartUtc;

        public bool IsRunning { get; private set; }
        public DateTime? LastStartUtc { get; private set; }
        public DateTime? LastEndUtc { get; private set; }
        public DateTime? LastSuccessUtc { get; private set; }
        public string Type { get; private set; }
        public bool StopOnError { get; private set; }
        public string Name { get; private set; }
        public bool Enabled { get; set; }
        
        private Task()
        {
            Enabled = true;
        }

        public Task(
            ContainerManager containerManager,
            ScheduleTask task, 
            IScheduleTaskService scheduleTaskService,
            ILogger logger,
            TaskFrequencyType? frequency,
            DateTime? nextStartUtc)
        {
            Type = task.Type;
            Enabled = task.Enabled;
            StopOnError = task.StopOnError;
            Name = task.Name;

            _scheduleTaskService = scheduleTaskService;
            _containerManager = containerManager;
            _logger = logger;
            _frequency = frequency;
            _nextStartUtc = nextStartUtc;
        }
        
        /// <summary>
        /// Executes the task
        /// </summary>
        /// <param name="throwException">A value indicating whether exception should be thrown if some error happens</param>
        /// <param name="dispose">A value indicating whether all instances hsould be disposed after task run</param>
        public void Execute(bool throwException = false, bool dispose = true)
        {
            IsRunning = true;
            DateTime? nextNewStartUtc = null;

            if (_frequency.HasValue && _nextStartUtc.HasValue)
            {
                if (_nextStartUtc.Value.CompareTo(DateTime.Now) > 0)
                {
                    return;
                }
                else
                {
                    switch (_frequency)
                    {
                        case TaskFrequencyType.Daily:
                            nextNewStartUtc = _nextStartUtc.Value.AddDays(1D);
                            break;
                        case TaskFrequencyType.Weekly:
                            nextNewStartUtc = _nextStartUtc.Value.AddDays(7D);
                            break;
                        case TaskFrequencyType.Monthly:
                            nextNewStartUtc = _nextStartUtc.Value.AddMonths(1);
                            break;
                        default:
                            break;
                    }

                    _nextStartUtc = nextNewStartUtc;
                }
            }
            
            //background tasks has an issue with Autofac
            //because scope is generated each time it's requested
            //that's why we get one single scope here
            //this way we can also dispose resources once a task is completed            
            var scheduleTask = _scheduleTaskService.GetTaskByType(Type);
            var scope = _containerManager.Scope();

            try
            {    
                var task = CreateTask(scope);
                if (task != null)
                {
                    LastStartUtc = DateTime.Now;
                    if (scheduleTask != null)
                    {
                        //update appropriate datetime properties
                        scheduleTask.LastStartUtc = LastStartUtc;
                        scheduleTask.NextStartUtc = nextNewStartUtc;
                        _scheduleTaskService.UpdateTask(scheduleTask);
                    }

                    var sw = Stopwatch.StartNew();

                    //execute task
                    task.Execute();

                    sw.Stop();

                    _logger.InsertLog(LogLevel.Information, string.Format("Task={{{0}}}, Time Elapsed(ms)={{{1}}}", Name, sw.ElapsedMilliseconds));

                    LastEndUtc = LastSuccessUtc = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Enabled = !StopOnError;
                LastEndUtc = DateTime.Now;

                //log error
                _logger.InsertLog(LogLevel.Error, string.Format("Error while running the '{0}' schedule task. {1}", Name, ex.Message), ex);
                
                if (throwException) throw;
            }

            if (scheduleTask != null)
            {
                //update appropriate datetime properties
                scheduleTask.LastEndUtc = LastEndUtc;
                scheduleTask.LastSuccessUtc = LastSuccessUtc;
                _scheduleTaskService.UpdateTask(scheduleTask);
            }

            //dispose all resources
            if (dispose)
            {
                scope.Dispose();
            }

            IsRunning = false;
        }

        private ITask CreateTask(ILifetimeScope scope)
        {
            ITask task = null;
            if (Enabled)
            {
                var type2 = System.Type.GetType(Type);
                if (type2 != null)
                {
                    object instance;

                    if (!scope.TryResolve(type2, out instance))
                    {
                        //not resolved
                        instance = ResolveUnregistered(type2, _containerManager.Scope());
                    }
                    task = instance as ITask;
                }
            }
            return task;
        }

        private object ResolveUnregistered(Type type, ILifetimeScope scope = null)
        {
            //if (scope == null)
            //{
            //    //no scope specified
            //    scope = Scope();
            //}
            var constructors = type.GetConstructors();
            foreach (var constructor in constructors)
            {
                try
                {
                    var parameters = constructor.GetParameters();
                    var parameterInstances = new List<object>();
                    foreach (var parameter in parameters)
                    {
                        var service = scope.Resolve(parameter.ParameterType);
                        if (service == null) throw new ApolloException("Unkown dependency");
                        parameterInstances.Add(service);
                    }
                    return Activator.CreateInstance(type, parameterInstances.ToArray());
                }
                catch (ApolloException)
                {

                }
            }
            throw new ApolloException("No contructor was found that had all the dependencies satisfied.");
        }
    }
}
