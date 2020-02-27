using System.Data;

namespace BitPixel.Common.DataContext
{
	public interface IDataContextFactory
	{
		IDataContext CreateContext();
		IDbConnection CreateConnection();
		IDataContext CreateReadOnlyContext();
	}
}
