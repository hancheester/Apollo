using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Infrastructure;
using Apollo.Core.Infrastructure.DependencyManagement;
using Apollo.Core.Logging;
using Apollo.Core.Performance;
using Autofac;

namespace Apollo.AdminStore.WebForm.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order
        {
            get { return 3; }
        }
        
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<LogBuilder>().As<ILogBuilder>();
            builder.RegisterType<CachePerformanceDataManager>();
            builder.RegisterType<AdminStoreUtility>().SingleInstance();
            builder.RegisterType<OfferUtility>().SingleInstance();
            builder.RegisterType<ImageUtility>().SingleInstance();
            builder.RegisterType<ExcelUtility>().SingleInstance();
        }
    }
}