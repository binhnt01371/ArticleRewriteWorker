using ArticleRewriteWorker;
using ArticleRewriteWorker.Models;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddDbContext<ReutersContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("ReutersContext"))
	.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution),
	ServiceLifetime.Transient);
var host = builder.Build();
host.Run();
