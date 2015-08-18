using System;
using System.IO;
using System.ServiceProcess;
using Overseer.Converters;
using Overseer.Readers;
using Overseer.Sources;
using Serilog;

namespace Overseer.ServiceConsole
{
	public partial class Service : ServiceBase
	{
		private QueueMonitor _monitor;
		private readonly string _baseDirectory;

		public Service()
		{
			_baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

			InitializeComponent();
			InitializeLogging();
		}

		public void StartConsole()
		{
			Console.WriteLine("Press any key to exit...");
			OnStart(new string[] { });

			Console.ReadKey();
			OnStop();
		}

		protected override void OnStart(string[] args)
		{
			// replace these with your own adapters
			IMessageReader reader = new InMemoryMessageReader();
			IMessageConverter converter = new DirectMessageConverter();
			IValidatorSource source = new FileValidatorSource(Path.Combine(_baseDirectory, "validators"));
			
			var output = new SerilogValidationOutput();

			_monitor = new QueueMonitor(reader, converter, new MessageValidator(source), output);
			_monitor.Start();
		}

		protected override void OnStop()
		{
			_monitor.Stop();
		}

		private void InitializeLogging()
		{
			var logs = Path.Combine(_baseDirectory, "logs");

			Directory.CreateDirectory(logs);

			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.WriteTo.ColoredConsole()
				.WriteTo.RollingFile(Path.Combine(logs, "{Date}.log"))
				.CreateLogger();
		}
	}
}
