using System.Linq.Expressions;

namespace DotMatrix.Datatables.DynamicLinq
{
	internal class DynamicOrdering
	{
		public Expression Selector;
		public bool Ascending;
	}
}