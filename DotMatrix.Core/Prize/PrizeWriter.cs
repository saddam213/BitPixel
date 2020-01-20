using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotMatrix.Common.DataContext;
using DotMatrix.Common.Prize;

namespace DotMatrix.Core.Prize
{
	public class PrizeWriter : IPrizeWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }

	}
}
