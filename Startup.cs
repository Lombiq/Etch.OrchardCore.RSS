using Etch.OrchardCore.RSS.Contents;
using Etch.OrchardCore.RSS.Controllers;
using Etch.OrchardCore.RSS.Drivers;
using Etch.OrchardCore.RSS.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.Data.Migration;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Feeds;
using OrchardCore.Modules;
using OrchardCore.Mvc.Core.Utilities;
using OrchardCore.Navigation;
using OrchardCore.Security.Permissions;
using OrchardCore.Settings;
using System;

namespace Etch.OrchardCore.RSS
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddContentPart<RssFeed>()
                .UseDisplayDriver<RssFeedDisplayDriver>();

            services.AddScoped<IDisplayDriver<ISite>, RSSFeedSettingsDisplayDriver>();
            services.AddScoped<INavigationProvider, AdminMenu>();
            services.AddScoped<IPermissionProvider, Permissions>();

            services.AddScoped<IDataMigration, Migrations>();

            // Feeds
            services.AddScoped<IFeedItemBuilder, Feeds>();
        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapAreaControllerRoute(
                name: "Rss.Feed",
                areaName: "Etch.OrchardCore.RSS",
                pattern: "/rss/{id}",
                defaults: new { controller = typeof(FeedController).ControllerName(), action = nameof(FeedController.Index) }
            );
        }
    }
}