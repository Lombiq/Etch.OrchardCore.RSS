using Etch.OrchardCore.Fields.Query.Fields;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentFields.Settings;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using OrchardCore.Media.Fields;
using OrchardCore.Media.Settings;
using OrchardCore.Title.Models;
using System.Threading.Tasks;

namespace Etch.OrchardCore.RSS
{
    public class Migrations : DataMigration
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public Migrations(IContentDefinitionManager contentDefinitionManager)
        {
            _contentDefinitionManager = contentDefinitionManager;
        }

        public async Task<int> CreateAsync()
        {
            await _contentDefinitionManager.AlterPartDefinitionAsync(Constants.RssFeed.ContentType, part => part
                .WithDescription("Create an RSS feed of content.")
                .WithDisplayName(Constants.RssFeed.DisplayName));

            await _contentDefinitionManager.AlterPartDefinitionAsync(Constants.RssFeed.ContentType, part => part
                .WithField(Constants.RssFeed.SourceFieldName, field => field
                    .OfType(nameof(QueryField))
                    .WithDisplayName(Constants.RssFeed.SourceFieldName)
                )
            );

            await _contentDefinitionManager.AlterTypeDefinitionAsync(Constants.RssFeed.ContentType, type => type
                .Draftable()
                .Versionable()
                .Listable()
                .Creatable()
                .Securable()
                .WithPart(nameof(TitlePart))
                .WithPart(Constants.RssFeed.ContentType)
                .DisplayedAs(Constants.RssFeed.DisplayName));

            return 1;
        }

        public async Task<int> UpdateFrom1Async()
        {
            await _contentDefinitionManager.AlterPartDefinitionAsync(Constants.RssFeed.ContentType, part => part
                .WithField(Constants.RssFeed.LinkFieldName, field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName(Constants.RssFeed.LinkFieldName)
                    .WithSettings(new TextFieldSettings
                    {
                        Hint = "URL to homepage."
                    })
                )
            );

            await _contentDefinitionManager.AlterPartDefinitionAsync(Constants.RssFeed.ContentType, part => part
                .WithField(Constants.RssFeed.DescriptionFieldName, field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName(Constants.RssFeed.DescriptionFieldName)
                    .WithSettings(new TextFieldSettings
                    {
                        Hint = "Short description of content within feed."
                    })
                    .WithEditor("TextArea")
                )
            );

            return 2;
        }

        public async Task<int> UpdateFrom2Async()
        {
            await _contentDefinitionManager.AlterPartDefinitionAsync(Constants.RssFeed.ContentType, part => part
                .WithField(Constants.RssFeed.DelayFieldName, field => field
                    .OfType(nameof(NumericField))
                    .WithDisplayName(Constants.RssFeed.DelayFieldName)
                    .WithSettings(new NumericFieldSettings
                    {
                        DefaultValue = "0",
                        Hint = "Delay when published content items appear in feed. Specify delay in minutes.",
                        Minimum = 0
                    })
                )
            );

            return 3;
        }

        public async Task<int> UpdateFrom3Async()
        {
            await _contentDefinitionManager.AlterPartDefinitionAsync(Constants.RssFeedItem.ContentPart, part => part
                .WithDescription("Manage values for item element within RSS feed.")
                .WithDisplayName("RSS Feed Item")
                .Attachable()
                .WithField(Constants.RssFeedItem.TitleFieldName, field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName(Constants.RssFeedItem.TitleFieldName)
                    .WithSettings(new TextFieldSettings
                    {
                        Hint = "The title of the item."
                    })
                )
                .WithField(Constants.RssFeedItem.DescriptionFieldName, field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName(Constants.RssFeedItem.DescriptionFieldName)
                    .WithEditor("TextArea")
                    .WithSettings(new TextFieldSettings
                    {
                        Hint = "The item synopsis"
                    })
                )
                .WithField(Constants.RssFeedItem.EnclosureFieldName, field => field
                    .OfType(nameof(MediaField))
                    .WithDisplayName(Constants.RssFeedItem.EnclosureFieldName)
                    .WithSettings(new MediaFieldSettings
                    {
                        Hint = "Describes a media object that is attached to the item.",
                        Multiple = false
                    })
                )
            );

            return 4;
        }
    }
}
