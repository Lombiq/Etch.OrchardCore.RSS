# Etch.OrchardCore.RSS

[Orchard Core](https://github.com/orchardcms/orchardcore) module handling the creation of RSS feeds whose contents are determined by queries.

## Build Status

[![Build Status](https://secure.travis-ci.org/etchuk/Etch.OrchardCore.RSS.png?branch=master)](http://travis-ci.org/etchuk/Etch.OrchardCore.RSS) [![NuGet](https://img.shields.io/nuget/v/Etch.OrchardCore.RSS.svg)](https://www.nuget.org/packages/Etch.OrchardCore.RSS)

## Orchard Core Reference

This module is referencing a stable build of Orchard Core ([`1.0.0`](https://www.nuget.org/packages/OrchardCore.Module.Targets/1.0.0)).

## Installing

This module is available on NuGet. Add a reference to your Orchard Core web project via the NuGet package manager. Search for "Etch.OrchardCore.RSS", ensuring include prereleases is checked.

## Usage

Enable "RSS Feed" feature within the admin dashboard. This will create a new "RSS Feed" content type. To create an RSS feed you'll need to have a [query](https://docs.orchardcore.net/en/dev/docs/reference/modules/Queries/) that returns content items that should be included in the feed. This query can be selected from a drop down field and there are other fields for customising meta data displayed in the RSS feed. Once an RSS feed content item has been created, the URL to access the feed will be displayed within the editor.

### Customising Item Properties

This module creates an "RSS Feed Item Part" that can be attached to a content type to give content editors the ability to modify values shown within an `<item>` in an RSS feed. Once attached, the editor will display an "RSS" tab for editing the title, description or enclosure. When generating content for the RSS feed it'll automatically pick up this part and use those values to populate the content in the feed.