using System.Data;
using System.Data.SqlClient;

using DotMatrix.Common.DataContext;

namespace DotMatrix.Data.DataContext
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
