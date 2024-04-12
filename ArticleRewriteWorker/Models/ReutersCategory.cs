using System;
using System.Collections.Generic;

namespace ArticleRewriteWorker.Models;

public partial class ReutersCategory
{
    public int CategoryId { get; set; }

    public string? Code { get; set; }

    public string? Type { get; set; }

    public string? Description { get; set; }
}
