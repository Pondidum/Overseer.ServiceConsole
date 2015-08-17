using System;
using System.IO;
using System.ServiceProcess;
using Serilog;

namespace Overseer.Service
{
	public partial class Service : ServiceBase
	{
		public Service()
		{
			InitializeComponent();
		}

		public void StartConsole()
		{
			OnStart(new string[] { });
		}

		protected override void OnStart(string[] args)
		{
			ConfigureLogging();
		}

		protected override void OnStop()
		{
		}

		private void ConfigureLogging()
		{
			var appRoot = AppDomain.CurrentDomain.BaseDirectory;
			var logs = Path.Combine(appRoot, "logs");

			Directory.CreateDirectory(logs);

			Log.Logger = new LoggerConfiguration()
				.WriteTo.ColoredConsole()
				.WriteTo.RollingFile(Path.Combine(logs, "{Date}.log"))
				.CreateLogger();
		}
	}
}
