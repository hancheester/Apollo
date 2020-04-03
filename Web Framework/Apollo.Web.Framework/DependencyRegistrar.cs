using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Apollo.Core;
using Apollo.Core.Caching;
using Apollo.Core.Configuration;
using Apollo.Core.Fakes;
using Apollo.Core.Infrastructure;
using Apollo.Core.Infrastructure.DependencyManagement;
using Apollo.Core.Plugins;
using Apollo.Core.Services.Interfaces;
using Apollo.Web.Framework.Services.Authentication;
using Apollo.Web.Framework.Services.Catalog;
using Apollo.Web.Framework.Services.Helpers;
using Apollo.Web.Framework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Apollo.Core.Services.Common;
using Apollo.Core.Services.Accounts;
using Apollo.Core.Services.Shipping;
using Apollo.Core.Services.Catalog;
using Apollo.Core.Services.Cms;
using Apollo.Core.Services.Payment;
using Apollo.Core.Services.Cart;
using Apollo.Core.Services.Payment.SagePay;
using Apollo.Core.Services.Blogs;
using Apollo.Core.Services.Orders;
using Apollo.Core.Services.Offer.OfferProcessor;
using Apollo.Core.Services.Offer;
using Apollo.Core.Services.Report;
using Apollo.Core.Services.Configuration;
using Apollo.Core.Services.Interfaces.Cms;
using Apollo.Core.Services.Directory;
using Apollo.Core.Services.Tasks;
using Apollo.Core.Services.Security;
using System.Security.Cryptography;
using Apollo.Core.Services.Caching;
using Apollo.Core.Services.Directory.IP2Country;
using Apollo.Core.Services.Directory.IP2Country.Net;
using System.Configuration;
using Apollo.Core.Services.DataBuilder;
using Apollo.Core.Services.Interfaces.DataBuilder;
using Apollo.DataAccess.EF;
using Apollo.DataAccess;
using Apollo.DataAccess.Interfaces;
using Apollo.Core.Services.Accounts.Identity;

namespace Apollo.Web.Framework
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order
        {
            get { return 0; }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //HTTP context and other related stuff
            builder.Register(c =>
                //register FakeHttpContext when HttpContext is not available
                HttpContext.Current != null ?
                (new HttpContextWrapper(HttpContext.Current) as HttpContextBase) :
                (new FakeHttpContext("~/") as HttpContextBase))
                .As<HttpContextBase>()
                .InstancePerLifetimeScope();

            builder.Register(c => c.Resolve<HttpContextBase>().Request)
                .As<HttpRequestBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Response)
                .As<HttpResponseBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Server)
                .As<HttpServerUtilityBase>()
                .InstancePerLifetimeScope();
            builder.Register(c => c.Resolve<HttpContextBase>().Session)
                .As<HttpSessionStateBase>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ApolloContext>().As<IDbContext>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();

            //web helper
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerLifetimeScope();
            //user agent helper
            builder.RegisterType<UserAgentHelper>().As<IUserAgentHelper>().InstancePerLifetimeScope();

            //controllers
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());

            //plugins
            builder.RegisterType<PluginFinder>().As<IPluginFinder>().InstancePerLifetimeScope();
            //builder.RegisterType<OfficialFeedManager>().As<IOfficialFeedManager>().InstancePerLifetimeScope();
            
            //cache manager
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().Named<ICacheManager>("Apollo_cache_static").SingleInstance();            
            
            builder.RegisterType<WebWorkContext>().As<IWorkContext>().InstancePerLifetimeScope();
            builder.RegisterType<PageHeadBuilder>().As<IPageHeadBuilder>().InstancePerLifetimeScope();

            #region ASP.NET Identity

            builder.RegisterType<IdentityMembership>().As<IWebMembership>().InstancePerLifetimeScope();
            builder.RegisterType<ApplicationIdentityContext>().InstancePerLifetimeScope();
            builder.RegisterType<ApplicationUserStore>().InstancePerLifetimeScope();
            builder.RegisterType<ApplicationUserManager>().InstancePerLifetimeScope();
            builder.RegisterType<ApplicationRoleStore>().InstancePerLifetimeScope();
            builder.RegisterType<ApplicationRoleManager>().InstancePerLifetimeScope();

            #endregion
            
            #region Core.Services

            builder.RegisterType<AccountService>().As<IAccountService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Apollo_cache_static"))
                .As<IAccountService>();

            builder.RegisterType<ShippingService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Apollo_cache_static"))
                .As<IShippingService>();

            builder.RegisterType<ProductService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Apollo_cache_static"))
                .As<IProductService>();

            builder.RegisterType<CampaignService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Apollo_cache_static"))
                .As<ICampaignService>();

            builder.RegisterType<CartService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Apollo_cache_static"))
                .As<ICartService>();

            builder.RegisterType<CategoryService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Apollo_cache_static"))
                .As<ICategoryService>();

            builder.RegisterType<BrandService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Apollo_cache_static"))
                .As<IBrandService>();

            builder.RegisterType<OrderService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Apollo_cache_static"))
                .As<IOrderService>();

            builder.RegisterType<AttributeUtility>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Apollo_cache_static"))
                .AsSelf();
            
            builder.RegisterType<OfferService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Apollo_cache_static"))
                .As<IOfferService>();

            builder.RegisterType<ReportService>().As<IReportService>();

            builder.RegisterType<SettingService>()
               .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Apollo_cache_static"))
               .As<ISettingService>()
               .SingleInstance();

            builder.RegisterType<WidgetService>()
               .As<IWidgetService>()
               .SingleInstance();

            builder.RegisterType<CurrencyService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Apollo_cache_static"))
                .As<ICurrencyService>();

            builder.RegisterType<ScheduleTaskService>().As<IScheduleTaskService>();
            builder.RegisterType<PdfService>().As<IPdfService>();
            builder.RegisterType<CsvService>().As<ICsvService>();

            builder.RegisterType<SystemCheckService>()
                .As<ISystemCheckService>()
                .SingleInstance();

            builder.RegisterType<CryptographyService<RijndaelManaged>>()
               .As<ICryptographyService>()
               .SingleInstance();

            builder.RegisterType<SpellCheckerService>()
                .As<ISpellCheckerService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Apollo_cache_static"))
                .SingleInstance();

            builder.RegisterType<GoogleFeedGenerator>()
                .As<IFeedGenerator>()
                .WithMetadata<IFeedGeneratorMetadata>(m =>
                {
                    m.For(t => t.Type, FeedGeneratorType.Google);
                });
            builder.RegisterType<AffiliateWindowFeedGenerator>()
                .As<IFeedGenerator>()
                .WithMetadata<IFeedGeneratorMetadata>(m =>
                {
                    m.For(t => t.Type, FeedGeneratorType.AffilicateWindow);
                });

            builder.RegisterType<ScheduleTaskService>()
               .As<IScheduleTaskService>()
               .SingleInstance();

            builder.RegisterType<CacheNotifier>().As<ICacheNotifier>();

            builder.RegisterType<DefaultSitemapGenerator>().As<ISitemapGenerator>();

            builder.RegisterType<GenericAttributeService>()
                .As<IGenericAttributeService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Apollo_cache_static"));

            builder.Register(s => InitIIPToCountry()).As<IIPToCountry>().SingleInstance();

            builder.RegisterType<EmailManager>().As<IEmailManager>().SingleInstance();

            builder.RegisterType<CartValidator>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Apollo_cache_static"))
                .As<ICartValidator>()
                .SingleInstance();

            builder.RegisterType<CartOfferProcessor>().As<ICartOfferProcessor>();
            builder.RegisterType<CatalogOfferProcessor>().As<ICatalogOfferProcessor>();
            builder.RegisterType<OrderCalculator>().As<IOrderCalculator>();
            builder.RegisterType<BlogService>().As<IBlogService>();
            builder.RegisterType<SagePayPaymentSystemService>().As<IPaymentSystemService>();
            builder.RegisterType<PaymentService>().As<IPaymentService>();
            builder.RegisterType<SearchService>().As<ISearchService>();
            builder.RegisterType<UtilityService>().As<IUtilityService>();

            builder.RegisterType<CartUtility>().AsSelf();
            builder.RegisterType<CatalogUtility>().AsSelf();

            builder.RegisterType<WidgetService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Apollo_cache_static"))
                .As<IWidgetService>()
                .SingleInstance();

            #endregion

            #region Services.DataBuilder

            builder.RegisterType<AddressBuilder>().As<IAddressBuilder>();
            builder.RegisterType<AccountBuilder>().As<IAccountBuilder>();
            builder.RegisterType<OrderBuilder>().As<IOrderBuilder>();
            builder.RegisterType<ProductBuilder>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Apollo_cache_static"))
                .As<IProductBuilder>();
            builder.RegisterType<CartItemBuilder>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Apollo_cache_static"))
                .As<ICartItemBuilder>();
            builder.RegisterType<LineItemBuilder>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Apollo_cache_static"))
                .As<ILineItemBuilder>();
            builder.RegisterType<OfferBuilder>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("Apollo_cache_static"))
                .As<IOfferBuilder>();

            #endregion
            
            //web framework services
            builder.RegisterType<PriceFormatter>().As<IPriceFormatter>().InstancePerLifetimeScope();
            builder.RegisterType<RecentlyViewedProductsService>().As<IRecentlyViewedProductsService>().InstancePerLifetimeScope();
            //builder.RegisterType<FormsAuthenticationService>().As<IAuthenticationService>().InstancePerLifetimeScope();
            builder.RegisterType<IdentityAuthenticationService>()
                .As<IAuthenticationService>()
                .As<IIdentityExternalAuthService>()
                .InstancePerLifetimeScope();

            builder.RegisterSource(new SettingsSource());
        }

        private static IIPToCountry InitIIPToCountry()
        {
            var config = ConfigurationManager.AppSettings["IP2CountryResourcePath"];
            var ip2Country = new IPToCountry();

            var resource1 = config + "delegated-afrinic-latest";
            var resource2 = config + "delegated-apnic-latest";
            var resource3 = config + "delegated-arin-latest";
            var resource4 = config + "delegated-lacnic-latest";
            var resource5 = config + "delegated-ripencc-latest";

            ip2Country.Load(resource1);
            ip2Country.Load(resource2);
            ip2Country.Load(resource3);
            ip2Country.Load(resource4);
            ip2Country.Load(resource5);

            return ip2Country;
        }

        public class SettingsSource : IRegistrationSource
        {
            public bool IsAdapterForIndividualComponents { get { return false; } }

            static readonly MethodInfo BuildMethod = typeof(SettingsSource).GetMethod("BuildRegistration", BindingFlags.Static | BindingFlags.NonPublic);

            public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<IComponentRegistration>> registrations)
            {
                var ts = service as TypedService;
                if (ts != null && typeof(ISettings).IsAssignableFrom(ts.ServiceType))
                {
                    var buildMethod = BuildMethod.MakeGenericMethod(ts.ServiceType);
                    yield return (IComponentRegistration)buildMethod.Invoke(null, null);
                }
            }

            static IComponentRegistration BuildRegistration<TSettings>() where TSettings : ISettings, new()
            {
                return RegistrationBuilder
                    .ForDelegate((c, p) =>
                    {
                        return c.Resolve<ISettingService>().LoadSetting<TSettings>();
                    })
                    .InstancePerLifetimeScope()
                    .CreateRegistration();
            }
        }
    }
}
