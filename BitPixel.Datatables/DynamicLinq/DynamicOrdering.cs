using System.Linq.Expressions;

namespace BitPixel.Datatables.DynamicLinq
{
	internal class DynamicOrdering
	{
		public Expression Selector;
		public bool Ascending;
	}
}