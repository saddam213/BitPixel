using System.Reflection;
using BitPixel.Datatables.Models;

namespace BitPixel.Datatables
{
	public class DataTablesRowIdAttribute : DataTablesAttributeBase
	{
		public bool EmitAsColumnName { get; set; }

		public override void ApplyTo(ColDef colDef, PropertyInfo pi)
		{
			// This attribute does not affect rendering
		}

		public DataTablesRowIdAttribute()
		{
			EmitAsColumnName = true;
		}
	}
}