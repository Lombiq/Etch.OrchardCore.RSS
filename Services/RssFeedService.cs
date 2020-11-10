using Etch.OrchardCore.Fields.Query.Fields;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;
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

        private readonly IContentManager _contentManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IQueryManager _queryManager;
        private readonly IUrlHelperFactory _urlHelperFactory;

        #endregion

        #region Constructor

        public RssFeedService(IContentManager contentManager, IHttpContextAccessor httpContextAccessor, IQueryManager queryManager, IUrlHelperFactory urlHelperFactory)
        {
            _contentManager = contentManager;
            _httpContextAccessor = httpContextAccessor;
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
            rss.Add(await CreateItemsAsync(results.Items.Cast<ContentItem>(), actionContext));

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

            var metadata = await _contentManager.PopulateAspectAsync<ContentItemMetadata>(contentItem);
            var url = $"{request.Scheme}://{request.Host}{urlHelper.Action(metadata.DisplayRouteValues["action"].ToString(), metadata.DisplayRouteValues)}";

            var item = new XElement("item");

            item.Add(new XElement("title", contentItem.DisplayText));
            item.Add(new XElement("link", url));
            item.Add(new XElement("guid", new XAttribute("isPermaLink", "true"), url));
            item.Add(new XElement("pubDate", contentItem.PublishedUtc.Value.ToString("r")));

            return item;
        }

        protected async Task<IList<XElement>> CreateItemsAsync(IEnumerable<ContentItem> contentItems, ActionContext actionContext)
        {
            return await Task.WhenAll(contentItems.Select(x => CreateItemAsync(x, actionContext)));
        }

        #endregion
    }

    public interface IRssFeedService
    {
        Task<XDocument> CreateFeedAsync(ContentItem contentItem, ActionContext actionContext);
    }
}
