using ArticleRewriteWorker.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SpinRewriter;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ArticleRewriteWorker
{
	public class Worker : IHostedService, IDisposable
	{
		private readonly ILogger<Worker> _logger;
		private Timer? _timer = null;
		private SpinRewriterAPI spinRewriter;
		private IConfiguration _configuration;
		private int retry = 0;
		private static string[] removedUpdateFromHeadLine = new string[] { 
			"CORRECTED-UPDATE",
			"REFILE-UPDATE",
			"RPT-UPDATE",
			"UPDATE",
			"WRAPUP"
		};

		private static string[] removedPrefixFromHeadLine = new string[] {
			"GLOBAL MARKETS-",
			"RPT-COLUMN-",
			"CORRECTED-ANALYSIS-",
			"CORRECTED-GLOBAL MARKETS-",
			"BRIEF-DFDS -",
			"RPT-INSIGHT-",
			"MIDEAST STOCKS-",
			"EMERGING MARKETS-",
			"GLOBAL LNG-",
			"PRESS DIGEST-",
			"US STOCKS-",
			"CORRECTED-(OFFICIAL)-",
			"CORRECTED (OFFICIAL)--",
			"REFILE-EXPLAINER-",
			"Med crude-",
			"PRESS DIGEST-",
			"INDIA STOCKS-",
			"EUROPE POWER-",
			"CANADA STOCKS-",
			"EMERGING MARKETS-",
			"OFFICIAL-CORRECTED-",
			"RPT-POLL-",
			"WIDER IMAGE-",
			"CANADA FX DEBT-",
			"CEE MARKETS-",
			"CORRECTED-CERAWEEK-",
			"CORRECTED-EXCLUSIVE-",
			"Middle East Crude-",
			"MORNING BID-",
			"RPT-ANALYSIS-",
			"RPT-CERAWEEK-",
			"FACTBOX-",
			"BRIEF-",
			"EXCLUSIVE-",
			"COLUMN-",
			"TABLE-",
			"REFILE-",
			"RPT-",
			"Golf-",
			"Rowing-",
			"Sailing-",
			"INSIGHT-",
			"WRAPUP-",
			"EXPLAINER-",
			"FOCUS-",
			"Soccer-",
			"GRAPHIC-",
			"PREVIEW-",
			"ANALYSIS-",
			"MOVES-",
			"TIMELINE-",
			"METALS-",
			"FEATURE-",
			"POLL-",
			"TREASURIES-",
			"PRECIOUS-",
			"CERAWEEK-",
			"CORRECTED-",
			"NEWSMAKER-",
		};

		private readonly IServiceScopeFactory _scopeFactory;
		public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration)
		{
			_configuration = configuration;
			_logger = logger;
			_scopeFactory = scopeFactory; 
			spinRewriter = new SpinRewriterAPI(_configuration["SpinRewriterAccount"], _configuration["SpinRewriterAPIKey"]);

			spinRewriter.setConfidenceLevel(ConfidenceLevels.High);
			spinRewriter.setAutoSentenceTrees(true);
			spinRewriter.setAutoSentences(true);
		}

		public Task StartAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("Timed Hosted Service running.");

			_timer = new Timer(DoWork, null, getJobRunDelay(_configuration["JobStartTime"]),
				TimeSpan.FromHours(24));

			return Task.CompletedTask;
		}

		private void DoWork(object? state)
		{
			try
			{
				using (var scope = _scopeFactory.CreateScope())
				{
					Console.WriteLine();
					Console.WriteLine($"===============================Start {DateTime.Now.ToString("dd-MM-yyyy")} - {retry}============================");
					Console.WriteLine();
					var _context = scope.ServiceProvider.GetRequiredService<ReutersContext>();
					_context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
					var itemsToConvert = _context.RsItemsToConverts.Where(x => x.StoryId == null).ToList();
					//var itemsToConvert = _context.RsItemsToConverts.Where(x => x.ReuterItemId == 200115).ToList();
					if (itemsToConvert == null || itemsToConvert.Count == 0)
					{
						Console.WriteLine();
						Console.WriteLine($"{DateTime.Now.ToString("dd-MM-yyyy")} : There is no item to run!");
						Console.WriteLine($"===============================End {DateTime.Now.ToString("dd-MM-yyyy")} - {retry}============================");
						Console.WriteLine();
						return;
					}
					var startItemId = itemsToConvert.FirstOrDefault();
					var count = 0;
					foreach (var itemToConvert in itemsToConvert)
					{
						count++;
						var itemDetails = _context.ReutersItemsDetails.FirstOrDefault(s => s.ReuterItemId == itemToConvert.ReuterItemId);
						if (itemDetails != null)
						{
							XDocument purchaseOrder = XDocument.Parse(itemDetails.Content);
							var body = purchaseOrder.Elements().Elements().Elements().ToArray()[1];
							var ptags = body.Elements();
							foreach (var tag in ptags)
							{
								tag.Name = tag.Name.LocalName;
							}
							var ptagsString = body.Elements().Select(s => s.ToString()).ToArray();
							StringBuilder contentStringBuilder = new StringBuilder();
							StringBuilder contentStringBuilder_1 = new StringBuilder();
							bool startToAppend = false;
							var ptagsStringLength = ptagsString.Length;
							for (int i = 0; i < ptagsStringLength; i++)
							{
								var stringToAppend = ptagsString[i].ToString();
								if (stringToAppend.Contains("(Reuters)"))
								{
									startToAppend = true;
									string removeString = stringToAppend.Split('-')[0];
									stringToAppend = stringToAppend.Replace(removeString, String.Empty).Replace("-", String.Empty);
									stringToAppend = $"<p>{stringToAppend}";
								}
								if (stringToAppend.Contains("<p>(") && stringToAppend.Contains(")</p>"))
								{
									continue;
								}
								if (i == ptagsStringLength - 1)
								{
									int bracketsIndex = stringToAppend.IndexOf("(");
									if (bracketsIndex != -1)
									{
										stringToAppend = $"{stringToAppend.Substring(0, bracketsIndex)}</p>";
									}
								}
								if (startToAppend)
									contentStringBuilder.AppendLine(stringToAppend);
								contentStringBuilder_1.AppendLine(stringToAppend);
							}

							var spinnedContentRequest = spinRewriter.getUniqueVariation(startToAppend ? contentStringBuilder.ToString() : contentStringBuilder_1.ToString());
							if (spinnedContentRequest["status"].ToString() != "\"OK\"")
							{
								throw new Exception(spinnedContentRequest["response"].ToString());
							}
							var spinnedContentResponse = spinnedContentRequest["response"].ToString().Replace("\"", "");

							var spinnedHeadLineRequest = spinRewriter.getUniqueVariation(ClearHeadline(itemDetails.Headline));
							if (spinnedHeadLineRequest["status"].ToString() != "\"OK\"")
							{
								throw new Exception(spinnedHeadLineRequest["response"].ToString());
							}
							var spinnedHeadLineResponse = spinnedHeadLineRequest["response"].ToString().Replace("\"", "");

							var con = _configuration.GetConnectionString("ReutersContext").ToString();
							using (var connection = new SqlConnection(_configuration.GetConnectionString("ReutersContext")))
							{
								connection.Open();
								SqlCommand command = connection.CreateCommand();
								// Start a local transaction.
								SqlTransaction transaction = connection.BeginTransaction();

								try
								{
									// Must assign both transaction object and connection
									// to Command object for a pending local transaction
									command.Connection = connection;
									command.Transaction = transaction;
									command.CommandText = "INSERT INTO [dbo].[site_Stories] ([UrlId] ,[Headline] ,[ContentHTML],[ContentText],[Author],[Description],[Status],[DatePublised],[DatePubUpdated],[Notes]) values (@UrlId, @Headline, @ContentHTML, @ContentText, @Author, @Description, @Status, @DatePublised, @DatePubUpdated, @Notes) ; SELECT SCOPE_IDENTITY();";
									command.Parameters.AddWithValue("@UrlId", DBNull.Value);
									command.Parameters.AddWithValue("@Headline", spinnedHeadLineResponse.ToString());
									command.Parameters.AddWithValue("@ContentHTML", spinnedContentResponse.ToString());
									command.Parameters.AddWithValue("@ContentText", Regex.Replace(spinnedContentResponse, "<.*?>", String.Empty).ToString());
									command.Parameters.AddWithValue("@Author", DBNull.Value);
									command.Parameters.AddWithValue("@Description", DBNull.Value);
									command.Parameters.AddWithValue("@Status", "1");
									command.Parameters.AddWithValue("@DatePublised", DateTime.Now.ToString());
									command.Parameters.AddWithValue("@DatePubUpdated", DateTime.Now.ToString());
									command.Parameters.AddWithValue("@Notes", DBNull.Value);
									var storyId = (decimal)command.ExecuteScalar();

									command.CommandText = "UPDATE [dbo].[rs_ItemsToConvert] SET [StoryId] = @StoryId ,[DateSpined] = @DateSpined,[SpinerService] = @SpinerService WHERE ReuterItemId = @ReuterItemId";
									command.Parameters.AddWithValue("@StoryId", storyId.ToString());
									command.Parameters.AddWithValue("@DateSpined", DateTime.Now.ToString());
									command.Parameters.AddWithValue("@SpinerService", "spinrewriter");
									command.Parameters.AddWithValue("@ReuterItemId", itemToConvert.ReuterItemId.ToString());
									command.ExecuteNonQuery();

									// Attempt to commit the transaction.
									transaction.Commit();
								}
								catch (Exception ex)
								{
									transaction.Rollback();
									throw new Exception(ex.Message);
								}
								connection.Close();
							}
							Console.WriteLine();
							Console.WriteLine($"Insert story with ID: {itemToConvert.ReuterItemId} to database.");
							Console.WriteLine();
						}
					}
					Console.WriteLine();
					Console.WriteLine($"Insert {count} stories to database.");
					Console.WriteLine($"===============================End {DateTime.Now.ToString("dd-MM-yyyy")} - {retry}============================");
					Console.WriteLine();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine();
				Console.WriteLine(ex.Message);
				Console.WriteLine($"===============================End {DateTime.Now.ToString("dd-MM-yyyy")} - {retry}============================");
				Console.WriteLine();
				retry++;
				if (retry != 5)
				{
					DoWork(null);
				}
				else
				{
					retry = 0;
					Console.WriteLine("Fail after retrying 5 times. The next run will be started next day.");
				}
			}
			
		}

		private string ClearHeadline(string headline)
		{
			foreach (string updateString in removedUpdateFromHeadLine)
			{
				for (int i = 0; i < 21; i++)
				{
					var stringToRemove = $"{updateString} {i}-";
					if (headline.Contains(stringToRemove))
					{
						return headline.Replace(stringToRemove, "");
					}
				}
			}

			foreach (string stringToRemove in removedPrefixFromHeadLine)
			{
				if (headline.Contains(stringToRemove))
				{
					return headline.Replace(stringToRemove, "");
				}
			}

			return headline;
		}

		public Task StopAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("Timed Hosted Service is stopping.");

			_timer?.Change(Timeout.Infinite, 0);

			return Task.CompletedTask;
		}

		public void Dispose()
		{
			_timer?.Dispose();
		}

		private static TimeSpan getScheduledParsedTime(string jobStartTime)
		{
			string[] formats = { @"hh\:mm\:ss", "hh\\:mm" };
			//string jobStartTime = "22:53"; // Start time
			TimeSpan.TryParseExact(jobStartTime, formats, CultureInfo.InvariantCulture, out TimeSpan ScheduledTimespan);
			return ScheduledTimespan;
		}

		private static TimeSpan getJobRunDelay(string jobStartTime)
		{
			TimeSpan scheduledParsedTime = getScheduledParsedTime(jobStartTime);
			TimeSpan curentTimeOftheDay = TimeSpan.Parse(DateTime.Now.TimeOfDay.ToString("hh\\:mm"));
			TimeSpan delayTime = scheduledParsedTime >= curentTimeOftheDay
				? scheduledParsedTime - curentTimeOftheDay     // Initial Run, when ETA is within 24 hours
				: new TimeSpan(24, 0, 0) - curentTimeOftheDay + scheduledParsedTime;   // For every other subsequent runs
			return delayTime;
		}
	}
}
