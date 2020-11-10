using Etch.OrchardCore.Fields.Query.Fields;
using Etch.OrchardCore.RSS.Services;
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
        private readonly IRssFeedService _rssFeedService;

        #endregion

        #region Constructor

        public FeedController(IContentManager contentManager, IRssFeedService rssFeedService)
        {
            _contentManager = contentManager;
            _rssFeedService = rssFeedService;
        }

        #endregion

        [HttpGet]
        public async Task<ActionResult> Index(string id)
        {
            var contentItem = await _contentManager.GetAsync(id);
            
            if (contentItem == null || !contentItem.Published)
            {
                return NotFound();
            }

            return new ContentResult
            {
                Content = (await _rssFeedService.CreateFeedAsync(contentItem, Url.ActionContext)).ToString(),
                ContentType = "text/xml",
                StatusCode = 200
            };
        }
    }
}
