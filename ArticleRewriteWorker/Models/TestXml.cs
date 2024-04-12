using System;
using System.Collections.Generic;

namespace ArticleRewriteWorker.Models;

public partial class TestXml
{
    public int Id { get; set; }

    public string? Xml { get; set; }

    public string? Note { get; set; }
}
