using System;
using System.Collections.Generic;

namespace ArticleRewriteWorker.Models;

public partial class RsItemsToConvert
{
    public int? ReuterItemId { get; set; }

    public int? SiteId { get; set; }

    public DateTime? DateSelected { get; set; }

    public int? StoryId { get; set; }

    public DateTime? DateSpined { get; set; }

    public string? SpinerService { get; set; }

    public string? Note { get; set; }
}
