using Etch.OrchardCore.Fields.Query.Fields;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;
using OrchardCore.Queries;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Etch.OrchardCore.RSS.Controllers
{
    public class FeedController : Controller
    {
        #region Dependencies

        private readonly IContentManager _contentManager;
        private readonly IQueryManager _queryManager;
        private readonly IUrlHelperFactory _urlHelperFactory;

        #endregion

        #region Constructor

        public FeedController(IContentManager contentManager, IQueryManager queryManager, IUrlHelperFactory urlHelperFactory)
        {
            _contentManager = contentManager;
            _queryManager = queryManager;
            _urlHelperFactory = urlHelperFactory;
        }

        #endregion

        [HttpGet]
        public async Task<ActionResult> Index(string id)
        {
            var feedContentItem = await _contentManager.GetAsync(id);
            
            if (feedContentItem == null || !feedContentItem.Published)
            {
                return NotFound();
            }

            var contentPart = feedContentItem.Get<ContentPart>(Constants.RSSFeedContentType);

            var query = await _queryManager.GetQueryAsync(contentPart?.Get<QueryField>(Constants.SourceFieldName)?.Value);
            var results = await _queryManager.ExecuteQueryAsync(query, null);

            var rss = new XElement("rss", new XAttribute("version", "2.0"), new XAttribute(XNamespace.Xmlns + "atom", "http://www.w3.org/2005/Atom"));
            rss.Add(await ApplyContentItemsAsync(results.Items.Cast<ContentItem>(), ApplyChannelMeta(feedContentItem, new XElement("channel"))));

            return new ContentResult
            {
                Content = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), rss).ToString(),
                ContentType = "text/xml",
                StatusCode = 200
            };
        }

        private async Task<XElement> ApplyContentItemsAsync(IEnumerable<ContentItem> contentItems, XElement channel)
        {
            foreach (var contentItem in contentItems)
            {
                var metadata = await _contentManager.PopulateAspectAsync<ContentItemMetadata>(contentItem);
                var url = $"{Request.Scheme}://{Request.Host}{Url.Action(metadata.DisplayRouteValues["action"].ToString(), metadata.DisplayRouteValues)}";

                var item = new XElement("item");
                item.Add(new XElement("title", contentItem.DisplayText));
                item.Add(new XElement("link", url));
                item.Add(new XElement("guid", new XAttribute("isPermaLink", "true"), url));
                item.Add(new XElement("pubDate", contentItem.PublishedUtc.Value.ToString("r")));

                channel.Add(item);
            }

            return channel;
        }

        private XElement ApplyChannelMeta(ContentItem contentItem, XElement channel)
        {
            var contentPart = contentItem.Get<ContentPart>(Constants.RSSFeedContentType);

            XNamespace atom = "http://www.w3.org/2005/Atom";

            var atomLink = new XElement(atom + "link");
            atomLink.SetAttributeValue("href", $"{Request.Scheme}://{Request.Host}{Request.Path}");
            atomLink.SetAttributeValue("rel", "self");
            atomLink.SetAttributeValue("type", "application/rss+xml");

            channel.Add(atomLink);
            channel.Add(new XElement("title", contentItem.DisplayText));
            channel.Add(new XElement("link", contentPart?.Get<TextField>(Constants.LinkFieldName)?.Text));
            channel.Add(new XElement("description", contentPart?.Get<TextField>(Constants.DescriptionFieldName)?.Text));

            return channel;
        }
    }
}
