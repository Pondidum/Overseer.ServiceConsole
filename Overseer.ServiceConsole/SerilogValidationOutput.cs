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
				var messages = new List<string>();
				BuildMessageLine(messages, result.Children);

				Log.Write(level, "{message}", string.Join(", ", messages));
			}

		}

		private void  BuildMessageLine(List<string> parts, IEnumerable<ValidationNode> nodes)
		{
			foreach (var node in nodes)
			{
				parts.Add($"[{node.Status}]: {node.ValidationMessage}");
				BuildMessageLine(parts, node.Children);
			}
		}

	}
}
