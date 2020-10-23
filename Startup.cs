using Etch.OrchardCore.RSS.Contents;
using Etch.OrchardCore.RSS.Drivers;
using Fluid;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Feeds;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Security.Permissions;
using OrchardCore.Settings;

namespace Etch.OrchardCore.RSS
{
    public class Startup : StartupBase
    {
        static Startup()
        {
            TemplateContext.GlobalMemberAccessStrategy.Register<ContentItem>();
            TemplateContext.GlobalMemberAccessStrategy.Register<ContentElement>();
            TemplateContext.GlobalMemberAccessStrategy.Register<ShapeViewModel<ContentItem>>();
            TemplateContext.GlobalMemberAccessStrategy.Register<ContentTypePartDefinition>();
            TemplateContext.GlobalMemberAccessStrategy.Register<ContentPartFieldDefinition>();
            TemplateContext.GlobalMemberAccessStrategy.Register<ContentFieldDefinition>();
            TemplateContext.GlobalMemberAccessStrategy.Register<ContentPartDefinition>();
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IDisplayDriver<ISite>, RSSFeedSettingsDisplayDriver>();
            services.AddScoped<INavigationProvider, AdminMenu>();
            services.AddScoped<IPermissionProvider, Permissions>();

            // Feeds
            services.AddScoped<IFeedItemBuilder, Feeds>();
        }
        
    }
}