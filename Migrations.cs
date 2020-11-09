using Etch.OrchardCore.Fields.Query.Fields;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentFields.Settings;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using OrchardCore.Title.Models;

namespace Etch.OrchardCore.RSS
{
    public class Migrations : DataMigration
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public Migrations(IContentDefinitionManager contentDefinitionManager)
        {
            _contentDefinitionManager = contentDefinitionManager;
        }

        public int Create()
        {
            _contentDefinitionManager.AlterPartDefinition(Constants.RSSFeedContentType, part => part
                .WithDescription("Create an RSS feed of content.")
                .WithDisplayName(Constants.RSSFeedContentType));

            _contentDefinitionManager.AlterPartDefinition(Constants.RSSFeedContentType, part => part
                .WithField(Constants.SourceFieldName, field => field
                    .OfType(nameof(QueryField))
                    .WithDisplayName(Constants.SourceFieldName)
                )
            );

            _contentDefinitionManager.AlterTypeDefinition(Constants.RSSFeedContentType, type => type
                .Draftable()
                .Versionable()
                .Listable()
                .Creatable()
                .Securable()
                .WithPart(nameof(TitlePart))
                .WithPart(Constants.RSSFeedContentType)
                .DisplayedAs(Constants.RSSFeedContentTypeDisplayName));

            return 1;
        }

        public int UpdateFrom1()
        {
            _contentDefinitionManager.AlterPartDefinition(Constants.RSSFeedContentType, part => part
                .WithField(Constants.LinkFieldName, field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName(Constants.LinkFieldName)
                    .WithSettings(new TextFieldSettings
                    {
                        Hint = "URL to homepage."
                    })
                )
            );

            _contentDefinitionManager.AlterPartDefinition(Constants.RSSFeedContentType, part => part
                .WithField(Constants.DescriptionFieldName, field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName(Constants.DescriptionFieldName)
                    .WithSettings(new TextFieldSettings
                    {
                        Hint = "Short description of content within feed."
                    })
                    .WithEditor("TextArea")
                )
            );

            return 2;
        }
    }
}
