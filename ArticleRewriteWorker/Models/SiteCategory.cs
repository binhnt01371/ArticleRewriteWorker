using System;
using System.Collections.Generic;

namespace ArticleRewriteWorker.Models;

public partial class SiteCategory
{
    public int SiteCategoryId { get; set; }

    public int? SiteId { get; set; }

    public string? SiteCategoryName { get; set; }

    public string? SiteCategoryUrl { get; set; }

    public bool? IsActiveOnSite { get; set; }

    public int? PositionOnSiteMenu { get; set; }

    public int? ParentSiteCategoryId { get; set; }
}
