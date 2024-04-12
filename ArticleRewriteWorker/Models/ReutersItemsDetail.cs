using System;
using System.Collections.Generic;

namespace ArticleRewriteWorker.Models;

public partial class ReutersItemsDetail
{
    public int ReuterItemId { get; set; }

    public string? TransmitId { get; set; }

    public string? ArticleId { get; set; }

    public DateTime? VersionCreated { get; set; }

    public string? Slugline { get; set; }

    public string? Headline { get; set; }

    public int? Wordcount { get; set; }

    public string? Content { get; set; }

    public string? Xml { get; set; }

    public DateTime? DateRupdated { get; set; }
}
