using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Etch.OrchardCore.RSS.Settings;
using Etch.OrchardCore.SEO.MetaTags.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Models;
using OrchardCore.Entities;
using OrchardCore.Feeds;
using OrchardCore.Feeds.Models;
using OrchardCore.Media;
using OrchardCore.Settings;

namespace Etch.OrchardCore.RSS.Contents
{
    public class Feeds : IFeedItemBuilder
    {
        #region Dependencies

        private readonly IContentManager _contentManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediaFileStore _mediaFileStore;
        private readonly ISiteService _siteService;

        #endregion

        #region Constructor

        public Feeds(IContentManager contentManager, IHttpContextAccessor httpContextAccessor, IMediaFileStore mediaFileStore, ISiteService siteService)
        {
            _contentManager = contentManager;
            _httpContextAccessor = httpContextAccessor;
            _mediaFileStore = mediaFileStore;
            _siteService = siteService;
        }

        #endregion

        #region Implementation

        public async Task PopulateAsync(FeedContext context)
        {
            foreach (var feedItem in context.Response.Items.OfType<FeedItem<ContentItem>>())
            {
                var contentItem = feedItem.Item;
                var contentItemMetadata = await _contentManager.PopulateAspectAsync<ContentItemMetadata>(contentItem);
                var bodyAspect = await _contentManager.PopulateAspectAsync<BodyAspect>(contentItem);
                var routes = contentItemMetadata.DisplayRouteValues;

                var metaTag = contentItem?.As<MetaTagsPart>();

                // add to known formats
                if (context.Format == "rss")
                {

                    // Skip the news article if the created time is before the delay time
                    var settings = (await _siteService.GetSiteSettingsAsync()).As<RSSFeedSettings>();
                    if (contentItem.CreatedUtc != null && contentItem.CreatedUtc.Value.AddHours(settings.Hour) > DateTime.UtcNow)
                    {
                        continue;
                    }


                    var link = new XElement("link");
                    var guid = new XElement("guid", new XAttribute("isPermaLink", "true"));

                    context.Response.Contextualize(contextualize =>
                    {
                        var request = contextualize.Url.ActionContext.HttpContext.Request;
                        var url = contextualize.Url.Action(routes["action"].ToString(), routes["controller"].ToString(), routes, request.Scheme);

                        link.Add(url);
                        guid.Add(url);
                    });

                    feedItem.Element.SetElementValue("title", WebUtility.HtmlEncode(!string.IsNullOrEmpty(metaTag?.Title) ? metaTag.Title : contentItem.DisplayText));
                    feedItem.Element.Add(link);


                    if (!string.IsNullOrEmpty(metaTag?.Description))
                    {
                        feedItem.Element.SetElementValue("description", $"{metaTag.Description}");

                        feedItem.Element.Add(new XElement("fullDescription", new XCData(
                            $"<img src=\"{GetMediaUrl(metaTag.Images[0])}\">" +
                            $"<br><br>" +
                            $"<h2>{metaTag.Title}</h2>" +
                            $"<p>{metaTag.Description}</p>"
                        )));
                    }
                    else
                    {
                        feedItem.Element.SetElementValue("description", bodyAspect.Body != null ? $"<![CDATA[{bodyAspect.Body?.ToString()}]]>" : string.Empty);
                    }

                    if (metaTag?.Images != null && metaTag.Images.Length > 0)
                    {
                        feedItem.Element.Add(new XElement("enclosure", new XAttribute[] {
                            new XAttribute("url", GetMediaUrl(metaTag.Images[0])),
                            new XAttribute("type", GetMimeType(metaTag.Images[0])),
                        }));
                    }



                    if (contentItem.CreatedUtc != null)
                    {
                        // RFC833
                        // The "R" or "r" standard format specifier represents a custom date and time format string that is defined by
                        // the DateTimeFormatInfo.RFC1123Pattern property. The pattern reflects a defined standard, and the property
                        // is read-only. Therefore, it is always the same, regardless of the culture used or the format provider supplied.
                        // The custom format string is "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'". When this standard format specifier is used,
                        // the formatting or parsing operation always uses the invariant culture.
                        feedItem.Element.SetElementValue("createdDate", contentItem.CreatedUtc.Value.ToString("r"));
                    }

                    feedItem.Element.Add(guid);
                }
            }
        }

        #endregion

        #region Private Methods

        private string GetHostUrl()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            return $"{request.Scheme}://{request.Host}";
        }

        public string GetMediaUrl(string path)
        {
            var imageUrl = _mediaFileStore.MapPathToPublicUrl(path);
            return imageUrl.StartsWith("http") ? imageUrl : $"{GetHostUrl()}{imageUrl}";
        }

        private string GetMimeType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(fileName, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }

        #endregion
    }

}
