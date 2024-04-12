using System;
using System.Collections.Generic;

namespace ArticleRewriteWorker.Models;

public partial class ReutersItem
{
    public int Rid { get; set; }

    public DateTime? DateRupdated { get; set; }

    public string? Id { get; set; }

    public string? Guid { get; set; }

    public DateTime? DateCreated { get; set; }

    public string? Slug { get; set; }

    public string? Headline { get; set; }

    public string? Channel { get; set; }

    public string? Xml { get; set; }
}
