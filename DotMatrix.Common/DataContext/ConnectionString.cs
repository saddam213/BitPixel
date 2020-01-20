using System.Configuration;

namespace DotMatrix.Common.DataContext
{
	public static class ConnectionString
	{
		public static string DefaultConnection
		{
			get
			{
				var defaultConnection = ConfigurationManager.ConnectionStrings["DefaultConnection"];
				if (defaultConnection != null)
					return defaultConnection.ConnectionString;

				return string.Empty;
			}
		}
	}
}
