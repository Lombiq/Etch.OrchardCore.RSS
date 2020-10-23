using System.Threading.Tasks;
using Etch.OrchardCore.RSS.Settings;
using Etch.OrchardCore.RSS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using OrchardCore.DisplayManagement.Entities;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.DisplayManagement.Views;
using OrchardCore.Settings;

namespace Etch.OrchardCore.RSS.Drivers
{
    public class RSSFeedSettingsDisplayDriver : SectionDisplayDriver<ISite, RSSFeedSettings>
    {
        #region Dependencies

        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Constructor

        public RSSFeedSettingsDisplayDriver(
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Overrides

        public override async Task<IDisplayResult> EditAsync(RSSFeedSettings settings, BuildEditorContext context)
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (!await _authorizationService.AuthorizeAsync(user, Permissions.ManageRSSFeedSettings))
            {
                return null;
            }

            return Initialize<RSSFeedSettingsViewModel>("RSSFeedSettings_Edit", model =>
            {
                model.Hour = settings.Hour;
            }).Location("Content:5").OnGroup(Constants.GroupId);
        }

        public override async Task<IDisplayResult> UpdateAsync(RSSFeedSettings settings, BuildEditorContext context)
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (!await _authorizationService.AuthorizeAsync(user, Permissions.ManageRSSFeedSettings))
            {
                return null;
            }

            if (context.GroupId == Constants.GroupId)
            {
                var model = new RSSFeedSettingsViewModel();
                await context.Updater.TryUpdateModelAsync(model, Prefix);

                if (context.Updater.ModelState.IsValid)
                {
                    settings.Hour = model.Hour;
                }
            }

            return await EditAsync(settings, context);
        }

        #endregion
    }
}
