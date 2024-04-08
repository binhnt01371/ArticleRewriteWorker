using SpinRewriter;
using System.Globalization;

namespace ArticleRewriteWorker
{
	public class Worker : IHostedService, IDisposable
	{
		private readonly ILogger<Worker> _logger;
		private Timer? _timer = null;
		static readonly string text =
			@"Ukrainian sea drones damaged a Russian Black Sea Fleet patrol ship off occupied Crimea, Ukrainian military intelligence said on Tuesday.

The intelligence agency said on Telegram messaging app that its special unit Group 13 attacked the Russian Black Sea Fleet patrol ship Sergey Kotov near the Kerch Strait.

""As a result of a strike by Magura V5 maritime drones, the Russian ship Project 22160 Sergey Kotov sustained damage to the stern, starboard and port sides,"" it said. The message added that the ship was worth about $65 million.

Andriy Yermak, President Volodymyr Zelenskiy's chief of staff, said on Telegram on Tuesday that ""The Russian Black Sea Fleet is a symbol of occupation. It cannot be in the Ukrainian Crimea,"" in an apparent reference to the attack.

Train traffic was temporarily stopped on the bridge linking the Crimean peninsula to the Russian mainland, according to the Telegram channel of a Russia-installed official in Crimea.

Highway traffic was also suspended for several hours before reopening just before 0700 GMT, according to the Telegram channel of the Russian-installed administration managing the bridge.

Reuters was unable to verify the reports.The Russian defence ministry did not immediately replied to a Reuters request for comment.

Ukrainian military said last month it had destroyed a Russian landing warship near Crimea in an operation with naval drones that breached the vessel's port side and caused it to sink.";

		public Worker(ILogger<Worker> logger)
		{
			_logger = logger;
		}

		public Task StartAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("Timed Hosted Service running.");

			_timer = new Timer(DoWork, null, getJobRunDelay(),
				TimeSpan.FromSeconds(5));

			return Task.CompletedTask;
		}

		private void DoWork(object? state)
		{
			var sw = new SpinRewriterAPI("bibik@marinelink.com", "b9c98eb#5360b87_77b64d4?8d5cba3");

			sw.setConfidenceLevel(ConfidenceLevels.High);
			sw.setAutoSentenceTrees(true);
			sw.setAutoSentences(true);

			var r = sw.getUniqueVariation(text);
			_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
			Console.Write(r.ToString());

			Console.Read();
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

		private static TimeSpan getScheduledParsedTime()
		{
			string[] formats = { @"hh\:mm\:ss", "hh\\:mm" };
			string jobStartTime = "23:33"; // Start time
			TimeSpan.TryParseExact(jobStartTime, formats, CultureInfo.InvariantCulture, out TimeSpan ScheduledTimespan);
			return ScheduledTimespan;
		}

		private static TimeSpan getJobRunDelay()
		{
			TimeSpan scheduledParsedTime = getScheduledParsedTime();
			TimeSpan curentTimeOftheDay = TimeSpan.Parse(DateTime.Now.TimeOfDay.ToString("hh\\:mm"));
			TimeSpan delayTime = scheduledParsedTime >= curentTimeOftheDay
				? scheduledParsedTime - curentTimeOftheDay     // Initial Run, when ETA is within 24 hours
				: new TimeSpan(24, 0, 0) - curentTimeOftheDay + scheduledParsedTime;   // For every other subsequent runs
			return delayTime;
		}
	}
}
