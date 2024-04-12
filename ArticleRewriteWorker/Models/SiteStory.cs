using System;
using System.Collections.Generic;

namespace ArticleRewriteWorker.Models;

public partial class SiteStory
{
    public int StoryId { get; set; }

    public string? UrlId { get; set; }

    public string? Headline { get; set; }

    public string? ContentHtml { get; set; }

    public string? ContentText { get; set; }

    public string? Author { get; set; }

    public string? Description { get; set; }

    public int? Status { get; set; }

    public DateTime? DatePublised { get; set; }

    public DateTime? DatePubUpdated { get; set; }

    public string? Notes { get; set; }
}
