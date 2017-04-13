using DotMatrix.Common.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Data.DataContext
{
	public class DataContextFactory : IDataContextFactory
	{
		public IDataContext CreateContext()
		{
			return new DataContext();
		}
	}
}
