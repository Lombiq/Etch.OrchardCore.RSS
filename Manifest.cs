using OrchardCore.Modules.Manifest;

[assembly: Module(
    Author = "Etch UK.",
    Category = "Content",
    Description = "RSS feed to include meta title, image and description.",
    Name = "RSS",
    Version = "0.0.1",
    Website = "https://etchuk.com"
)]

[assembly: Feature(
    Id = "Etch.OrchardCore.RSS",
    Name = "RSS",
    Description = "RSS feed to include meta title, image and description.",
    Category = "Content",
    Dependencies = new[] { "Etch.OrchardCore.Fields.Query", "Etch.OrchardCore.SEO.MetaTags", "OrchardCore.ContentFields", "OrchardCore.Title" }
)]