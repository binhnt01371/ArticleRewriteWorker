using System;
using System.Collections.Generic;

namespace ArticleRewriteWorker.Models;

public partial class ReutersItemsDetailsError
{
    public int Rid { get; set; }

    public string? TransmitId { get; set; }

    public string? Error { get; set; }

    public DateTime? DateCreated { get; set; }
}
