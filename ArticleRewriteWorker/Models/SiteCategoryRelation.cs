using System;
using System.Collections.Generic;

namespace ArticleRewriteWorker.Models;

public partial class SiteCategoryRelation
{
    public int? CategoryId { get; set; }

    public int? SiteCategoryId { get; set; }

    public bool? IsGoodForNewsSelection { get; set; }

    public int? SiteId { get; set; }
}
