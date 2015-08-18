using System;
using System.ServiceProcess;

namespace Overseer.ServiceConsole
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			var service = new Service();

			if (Environment.UserInteractive)
			{
				service.StartConsole();
			}
			else
			{
				ServiceBase.Run(new ServiceBase[] { service });
			}
		}
	}
}
