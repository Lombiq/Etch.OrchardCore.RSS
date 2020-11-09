using Etch.OrchardCore.RSS.Controllers;
using Etch.OrchardCore.RSS.Drivers;
using Etch.OrchardCore.RSS.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using OrchardCore.Mvc.Core.Utilities;
using System;

namespace Etch.OrchardCore.RSS
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddContentPart<RssFeed>()
                .UseDisplayDriver<RssFeedDisplayDriver>();

            services.AddScoped<IDataMigration, Migrations>();
        }

        public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
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