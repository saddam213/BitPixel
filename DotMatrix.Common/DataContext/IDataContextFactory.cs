using System.Data;

namespace DotMatrix.Common.DataContext
{
	public interface IDataContextFactory
	{
		IDataContext CreateContext();
		IDbConnection CreateConnection();
		IDataContext CreateReadOnlyContext();
	}
}
