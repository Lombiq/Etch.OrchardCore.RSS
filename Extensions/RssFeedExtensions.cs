using Etch.OrchardCore.SEO.MetaTags.Models;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Models;

namespace Etch.OrchardCore.RSS.Extensions
{
    public static class RssFeedExtensions
    {
        public static string GetDescription(this ContentItem contentItem, BodyAspect bodyAspect)
        {
            var rssFeedItemPart = contentItem.Get<ContentPart>(Constants.RssFeedItem.ContentPart);

            if (rssFeedItemPart != null && !string.IsNullOrWhiteSpace(rssFeedItemPart.Get<TextField>(Constants.RssFeedItem.DescriptionFieldName).Text))
            {
                return rssFeedItemPart.Get<TextField>(Constants.RssFeedItem.DescriptionFieldName).Text;
            }

            if (contentItem.Has<MetaTagsPart>() && !string.IsNullOrWhiteSpace(contentItem.As<MetaTagsPart>()?.Description))
            {
                return contentItem.As<MetaTagsPart>()?.Description;
            }

            return bodyAspect.Body != null ? $"<![CDATA[{bodyAspect.Body?.ToString()}]]>" : string.Empty;
        }

        public static string GetTitle(this ContentItem contentItem)
        {
            var rssFeedItemPart = contentItem.Get<ContentPart>(Constants.RssFeedItem.ContentPart);

            if (rssFeedItemPart != null && !string.IsNullOrWhiteSpace(rssFeedItemPart.Get<TextField>(Constants.RssFeedItem.TitleFieldName).Text))
            {
                return rssFeedItemPart.Get<TextField>(Constants.RssFeedItem.TitleFieldName).Text;
            }

            if (contentItem.Has<MetaTagsPart>() && !string.IsNullOrWhiteSpace(contentItem.As<MetaTagsPart>()?.Title))
            {
                return contentItem.As<MetaTagsPart>()?.Title;
            }

            return contentItem.DisplayText;
        }
    }
}
