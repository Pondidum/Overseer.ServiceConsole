using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Overseer.Service
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			var service = new Service();

			if (Debugger.IsAttached)
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
