using System;

namespace BitPixel.Base.Logging
{
	/// <summary>
	/// Outputs messages to the console
	/// </summary>
	public class ConsoleLogger : Logger
	{
		public ConsoleLogger(LogLevel level)
				: base(level)
		{
		}

		protected override void LogQueuedMessage(string message)
		{
			Console.WriteLine(message);
		}
	}
}