using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Core;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Mehdime.Entity;

using SDDB.Domain.Abstract;
using SDDB.Domain.DbContexts;
using SDDB.Domain.Entities;
using SDDB.Domain.Services;
using SDDB.Domain.Infrastructure;
using SDDB.WebUI.Infrastructure;

namespace SDDB.WebUI
{
    public class AutofacConfig
    {
        public static void ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            //retrieve global properties
            var LDAPAuthenticationEnabled = bool.Parse(ConfigurationManager.AppSettings["LDAPAuthenticationEnabled"] ?? "false");
            var dbLoggingLevel = int.Parse(ConfigurationManager.AppSettings["dbLoggingLevel"] ?? "1");
            var procTooLongmSec = int.Parse(ConfigurationManager.AppSettings["procTooLongmSec"] ?? "0");

            //register DB contexts
            builder.RegisterType<EFDbContext>().AsSelf().InstancePerDependency();

            //register Mehdime DBContextScope
            builder.RegisterType<DbContextScopeFactory>().As<IDbContextScopeFactory>().SingleInstance();
            builder.RegisterType<AmbientDbContextLocator>().As<IAmbientDbContextLocator>().SingleInstance();

            //register repositories


            //register infrastructure

            // register services
            builder.RegisterType<DBLogger>().As<ILogger>().WithParameter("dbLogginglevel", dbLoggingLevel).WithParameter("procTooLongmSec", procTooLongmSec).InstancePerDependency();
            
            builder.RegisterType<DBUserService>().AsSelf().WithParameter("ldapAuthenticationEnabled", LDAPAuthenticationEnabled).InstancePerDependency();
            builder.RegisterType<PersonService>().AsSelf().InstancePerDependency();
            builder.RegisterType<PersonGroupService>().AsSelf().InstancePerDependency();
            builder.RegisterType<PersonActivityTypeService>().AsSelf().InstancePerDependency();
            
            builder.RegisterType<ProjectService>().AsSelf().InstancePerDependency();
            builder.RegisterType<ProjectEventService>().AsSelf().InstancePerDependency();
            
            builder.RegisterType<DocumentService>().AsSelf().InstancePerDependency();
            builder.RegisterType<DocumentTypeService>().AsSelf().InstancePerDependency();
            
            builder.RegisterType<LocationService>().AsSelf().InstancePerDependency();
            builder.RegisterType<LocationTypeService>().AsSelf().InstancePerDependency();
            
            builder.RegisterType<AssemblyDbService>().AsSelf().InstancePerDependency();
            builder.RegisterType<AssemblyTypeService>().AsSelf().InstancePerDependency();
            builder.RegisterType<AssemblyModelService>().AsSelf().InstancePerDependency();
            builder.RegisterType<AssemblyStatusService>().AsSelf().InstancePerDependency();

            builder.RegisterType<ComponentService>().AsSelf().InstancePerDependency();
            builder.RegisterType<ComponentTypeService>().AsSelf().InstancePerDependency();
            builder.RegisterType<ComponentModelService>().AsSelf().InstancePerDependency();
            builder.RegisterType<ComponentStatusService>().AsSelf().InstancePerDependency();

            //----Identity Setup-----
            builder.RegisterType<AppUserStore>().AsImplementedInterfaces().InstancePerRequest();
            builder.Register<IdentityFactoryOptions<AppUserManager>>(c => new IdentityFactoryOptions<AppUserManager>()
            {
                DataProtectionProvider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("SDDB")
            });
            builder.RegisterType<AppUserManager>().As<IAppUserManager>().InstancePerRequest();
            builder.RegisterType<AppRoleStore>().AsSelf().InstancePerRequest();
            builder.Register<IdentityFactoryOptions<AppRoleManager>>(c => new IdentityFactoryOptions<AppRoleManager>()
            {
                DataProtectionProvider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("SDDB")
            });
            builder.RegisterType<AppRoleManager>().As<IAppRoleManager>().InstancePerRequest();


            // Register your MVC controllers.
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            //Inject Properties into custom atttributes
            builder.RegisterType<DBSrvAuthAttribute>().PropertiesAutowired();

            // OPTIONAL: Register model binders that require DI.
            //builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
            //builder.RegisterModelBinderProvider();

            // OPTIONAL: Register web abstractions like HttpContextBase.
            //builder.RegisterModule<AutofacWebTypesModule>();

            // OPTIONAL: Enable property injection in view pages.
            //builder.RegisterSource(new ViewRegistrationSource());

            //Enable property injection into action filters.
            builder.RegisterFilterProvider();

            //Register global filters.
            builder.RegisterType<DBExceptionAttribute>().PropertiesAutowired().AsExceptionFilterFor<Controller>().InstancePerRequest();

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

        }
    }
}