using Etch.OrchardCore.RSS.Models;
using Etch.OrchardCore.RSS.ViewModels;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using System.Threading.Tasks;

namespace Etch.OrchardCore.RSS.Drivers
{
    public class RssFeedDisplayDriver : ContentPartDisplayDriver<RssFeed>
    {
        #region Overrides

        public override IDisplayResult Edit(RssFeed part, BuildPartEditorContext context)
        {
            return Initialize<EditRssFeedViewModel>(GetEditorShapeType(context), model =>
            {
                model.ContentItem = part.ContentItem;
            });
        }

        public override async Task<IDisplayResult> UpdateAsync(RssFeed part, IUpdateModel updater, UpdatePartEditorContext context)
        {
            var viewModel = new EditRssFeedViewModel();

            await updater.TryUpdateModelAsync(viewModel, Prefix);

            return Edit(part, context);
        }

        #endregion
    }
}
