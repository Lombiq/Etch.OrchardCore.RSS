using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;

namespace Etch.OrchardCore.RSS
{
    public class AdminMenu : INavigationProvider
    {
        private readonly IStringLocalizer S;

        public AdminMenu(IStringLocalizer<AdminMenu> localizer)
        {
            S = localizer;
        }

        public Task BuildNavigationAsync(string name, NavigationBuilder builder)
        {
            if (String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                builder.Add(S["Configuration"], configuration => configuration
                        .Add(S["Settings"], settings => settings
                            .Add(S["RSS Feed"], S["RSS Feed"], settings => settings
                            .Action("Index", "Admin", new { area = "OrchardCore.Settings", groupId = Constants.GroupId })
                                .Permission(Permissions.ManageRSSFeedSettings)
                                .LocalNav())
                            )
                        );
            }
            return Task.CompletedTask;
        }
    }

}
