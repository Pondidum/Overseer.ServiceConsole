using System.Collections.Generic;
using System.Linq;
using Serilog;
using Serilog.Context;
using Serilog.Events;

namespace Overseer.ServiceConsole
{
	public class SerilogValidationOutput : IValidationOutput
	{
		private static readonly ILogger Log = Serilog.Log.ForContext<SerilogValidationOutput>();

		private static readonly Dictionary<Status, LogEventLevel> StatusMap = new Dictionary<Status, LogEventLevel>
		{
			{Status.NotInterested, LogEventLevel.Debug},
			{Status.Pass, LogEventLevel.Information},
			{Status.Warning,  LogEventLevel.Warning},
			{Status.Fail, LogEventLevel.Error}
		};

		public void Write(ValidationResult result)
		{
			var level = StatusMap[result.Status];

			using (LogContext.PushProperty("ValidationResult", result))
			{
				var messages = BuildMessageLine(result);

				Log.Write(level, "{message}", string.Join(", ", messages));
			}

		}

		private IEnumerable<string> BuildMessageLine(ValidationResult result)
		{
			var node = result as ValidationResultNode;

			if (node == null)
			{
				return new[]
				{
					string.Format("[{0}] {1}", result.Status, result.Message)	
				};
			}

			return node.Results.SelectMany(BuildMessageLine);
		}

	}
}
