using Apollo.Core.Infrastructure;
using Apollo.Core.Infrastructure.DependencyManagement;
using Apollo.Core.Logging;
using Apollo.Core.Performance;
using Autofac;

namespace Apollo.FrontStore.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order
        {
            get { return 2; }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<LogBuilder>().As<ILogBuilder>();
            builder.RegisterType<ApolloSessionState>().SingleInstance();
            builder.RegisterType<CachePerformanceDataManager>();
        }
    }
}