using System;
using System.Collections.Generic;

namespace ArticleRewriteWorker.Models;

public partial class TEnergyCategory
{
    public double? CategoryId { get; set; }

    public string? Code { get; set; }

    public string? Description { get; set; }

    public double? StoriesCount { get; set; }

    public double? StoriesPercent { get; set; }

    public string? Energy { get; set; }
}
