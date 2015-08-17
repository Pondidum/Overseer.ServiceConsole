using System;
using System.IO;
using System.ServiceProcess;

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
		}

		protected override void OnStop()
		{
		}
	}
}
