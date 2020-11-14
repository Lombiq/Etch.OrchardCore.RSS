namespace Etch.OrchardCore.RSS
{
    public static class Constants
    {
        public const string GroupId = "RSSFeed";

        public static class RssFeed
        {
            public const string ContentType = "RssFeed";
            public const string DisplayName = "RSS Feed";

            public const string DescriptionFieldName = "Description";
            public const string DelayFieldName = "Delay";
            public const string LinkFieldName = "Link";
            public const string SourceFieldName = "Source";
        }

        public static class RssFeedItem
        {
            public const string ContentPart = "RssFeedItemPart";
            public const string DescriptionFieldName = "Description";
            public const string EnclosureFieldName = "Enclosure";
            public const string TitleFieldName = "Title";
        }
    }
}
