using System.Data;
using System.Data.SqlClient;

using BitPixel.Common.DataContext;

namespace BitPixel.Data.DataContext
{
	public class DataContextFactory : IDataContextFactory
	{
		public IDataContext CreateContext()
		{
			return new DataContext();
		}

		public IDbConnection CreateConnection()
		{
			return new SqlConnection(ConnectionString.DefaultConnection);
		}

		public IDataContext CreateReadOnlyContext()
		{
			var context = new DataContext();
			context.Configuration.AutoDetectChangesEnabled = false;
			context.Configuration.LazyLoadingEnabled = false;
			context.Configuration.ProxyCreationEnabled = false;
			return context;
		}
	}
}
