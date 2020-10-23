using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrchardCore.Security.Permissions;

namespace Etch.OrchardCore.RSS
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ManageRSSFeedSettings = new Permission(nameof(ManageRSSFeedSettings), "Manage RSS Feed settings");

        public Task<IEnumerable<Permission>> GetPermissionsAsync()
        {
            return Task.FromResult(new[]
            {
                    ManageRSSFeedSettings
                }
            .AsEnumerable());
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            yield return new PermissionStereotype
            {
                Name = "Administrator",
                Permissions = new[]
                {
                    ManageRSSFeedSettings
                }
            };
        }
    }

}
