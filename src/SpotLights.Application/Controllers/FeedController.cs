using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SpotLights.Domain.Dto;
using SpotLights.Core.Interfaces;
using SpotLights.Core.Interfaces.Post;

namespace SpotLights.Controllers;

public class FeedController : Controller
{
  private readonly IBlogService _blogManager;
  private readonly IPostService _postProvider;
  private readonly IMarkdigService _markdigProvider;

  public FeedController(
      IBlogService blogManager,
      IPostService postProvider,
      IMarkdigService markdigProvider
  )
  {
    _blogManager = blogManager;
    _postProvider = postProvider;
    _markdigProvider = markdigProvider;
  }

  [ResponseCache(Duration = 1200)]
  [HttpGet("feed")]
  public async Task<IActionResult> Rss()
  {
    string host = Request.Scheme + "://" + Request.Host;
    BlogData data = await _blogManager.GetAsync();
    IEnumerable<Shared.PostDto> posts = await _postProvider.GetAsync();
    List<SyndicationItem> items = [];

    DateTime publishedAt = DateTime.UtcNow;
    if (posts != null)
      foreach (Shared.PostDto post in posts)
      {
        string url = $"{host}/posts/{post.Slug}";
        string description = _markdigProvider.ToHtml(post.Content);
        SyndicationItem item =
            new(post.Title, description, new Uri(url), url, publishedAt)
            {
              PublishDate = publishedAt
            };
        items.Add(item);
      }
    SyndicationFeed feed =
        new(data.Title, data.Description, new Uri(host), host, publishedAt) { Items = items };
    XmlWriterSettings settings =
        new()
        {
          Encoding = Encoding.UTF8,
          NewLineHandling = NewLineHandling.Entitize,
          NewLineOnAttributes = true,
          Indent = true
        };
    using MemoryStream stream = new();
    using (XmlWriter xmlWriter = XmlWriter.Create(stream, settings))
    {
      Rss20FeedFormatter rssFormatter = new(feed, false);
      rssFormatter.WriteTo(xmlWriter);
      xmlWriter.Flush();
    }
    return File(stream.ToArray(), "application/xml; charset=utf-8");
  }
}
