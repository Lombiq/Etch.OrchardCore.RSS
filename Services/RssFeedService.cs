using Etch.OrchardCore.Fields.Query.Fields;
using Etch.OrchardCore.RSS.Extensions;
using Etch.OrchardCore.SEO.MetaTags.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.StaticFiles;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Models;
using OrchardCore.Media;
using OrchardCore.Modules;
using OrchardCore.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Etch.OrchardCore.RSS.Services
{
    public class RssFeedService : IRssFeedService
    {
        #region Dependencies

        private readonly IClock _clock;
        private readonly IContentManager _contentManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediaFileStore _mediaFileStore;
        private readonly IQueryManager _queryManager;
        private readonly IUrlHelperFactory _urlHelperFactory;

        #endregion

        #region Constructor

        public RssFeedService(IClock clock, IContentManager contentManager, IHttpContextAccessor httpContextAccessor, IMediaFileStore mediaFileStore, IQueryManager queryManager, IUrlHelperFactory urlHelperFactory)
        {
            _clock = clock;
            _contentManager = contentManager;
            _httpContextAccessor = httpContextAccessor;
            _mediaFileStore = mediaFileStore;
            _queryManager = queryManager;
            _urlHelperFactory = urlHelperFactory;
        }

        #endregion

        #region Implementation

        public async Task<XDocument> CreateFeedAsync(ContentItem contentItem, ActionContext actionContext)
        {
            var contentPart = contentItem.Get<ContentPart>(Constants.RSSFeedContentType);
            var query = await _queryManager.GetQueryAsync(contentPart?.Get<QueryField>(Constants.SourceFieldName)?.Value);
            var results = await _queryManager.ExecuteQueryAsync(query, null);
            var rss = new XElement("rss", new XAttribute("version", "2.0"), new XAttribute(XNamespace.Xmlns + "atom", "http://www.w3.org/2005/Atom"));

            rss.Add(CreateChannelMeta(contentItem));
            rss.Add(await CreateItemsAsync(contentItem, results.Items.Cast<ContentItem>(), actionContext));

            return new XDocument(new XDeclaration("1.0", "utf-8", "yes"), rss);
        }

        #endregion

        #region Helpers

        protected IList<XElement> CreateChannelMeta(ContentItem contentItem)
        {
            var contentPart = contentItem.Get<ContentPart>(Constants.RSSFeedContentType);
            var request = _httpContextAccessor.HttpContext.Request;

            XNamespace atom = "http://www.w3.org/2005/Atom";

            var atomLink = new XElement(atom + "link");
            atomLink.SetAttributeValue("href", $"{request.Scheme}://{request.Host}{request.Path}");
            atomLink.SetAttributeValue("rel", "self");
            atomLink.SetAttributeValue("type", "application/rss+xml");

            return new List<XElement>
            {
                atomLink,
                new XElement("title", contentItem.DisplayText),
                new XElement("link", contentPart?.Get<TextField>(Constants.LinkFieldName)?.Text),
                new XElement("description", contentPart?.Get<TextField>(Constants.DescriptionFieldName)?.Text)
            };
        }

        protected async Task<XElement> CreateItemAsync(ContentItem contentItem, ActionContext actionContext)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var urlHelper = _urlHelperFactory.GetUrlHelper(actionContext);

            var bodyAspect = await _contentManager.PopulateAspectAsync<BodyAspect>(contentItem);
            var metadata = await _contentManager.PopulateAspectAsync<ContentItemMetadata>(contentItem);
            var url = $"{request.Scheme}://{request.Host}{urlHelper.Action(metadata.DisplayRouteValues["action"].ToString(), metadata.DisplayRouteValues)}";

            var item = new XElement("item");

            item.Add(new XElement("title", contentItem.GetTitle()));
            item.Add(new XElement("description", contentItem.GetDescription(bodyAspect)));
            item.Add(new XElement("link", url));
            item.Add(new XElement("guid", new XAttribute("isPermaLink", "true"), url));
            item.Add(new XElement("pubDate", contentItem.PublishedUtc.Value.ToString("r")));

            var enclosure = GetEnclosure(contentItem);

            if (enclosure != null)
            {
                item.Add(enclosure);
            }

            return item;
        }

        protected async Task<IList<XElement>> CreateItemsAsync(ContentItem contentItem, IEnumerable<ContentItem> contentItems, ActionContext actionContext)
        {
            var delay = contentItem.Get<ContentPart>(Constants.RSSFeedContentType)?.Get<NumericField>(Constants.Delay)?.Value ?? 0;

            return await Task.WhenAll(contentItems.Where(x => ShouldInclude(x, delay)).Select(x => CreateItemAsync(x, actionContext)));
        }

        private XElement GetEnclosure(ContentItem contentItem)
        {
            var metaTag = contentItem.As<MetaTagsPart>();

            if (metaTag == null)
            {
                return null;
            }

            return new XElement("enclosure", new XAttribute[] {
                new XAttribute("url", GetMediaUrl(metaTag.Images[0])),
                new XAttribute("type", GetMimeType(metaTag.Images[0])),
            });
        }

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

            if (!provider.TryGetContentType(fileName, out string contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }

        private bool ShouldInclude(ContentItem contentItem, decimal delay)
        {
            if (delay == 0)
            {
                return true;
            }

            return contentItem.PublishedUtc.Value.AddMinutes(double.Parse(delay.ToString())) < _clock.UtcNow;
        }

        #endregion
    }

    public interface IRssFeedService
    {
        Task<XDocument> CreateFeedAsync(ContentItem contentItem, ActionContext actionContext);
    }
}
