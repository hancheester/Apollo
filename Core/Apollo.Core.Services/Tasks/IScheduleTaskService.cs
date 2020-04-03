using Apollo.Core.Model.Entity;
using System.Collections.Generic;

namespace Apollo.Core.Services.Tasks
{
    public partial interface IScheduleTaskService
    {
        void DeleteTask(ScheduleTask task);
        ScheduleTask GetTaskById(int taskId);
        ScheduleTask GetTaskByType(string type);
        IList<ScheduleTask> GetAllTasks(bool showHidden = false);
        void InsertTask(ScheduleTask task);
        void UpdateTask(ScheduleTask task);
    }
}
