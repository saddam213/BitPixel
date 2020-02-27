using System;
using System.Linq;
using System.Reflection;
using BitPixel.Datatables.Models;
using BitPixel.Datatables.Reflection;

namespace BitPixel.Datatables
{
	public class DataTablesAttribute : DataTablesAttributeBase
	{
		public DataTablesAttribute()
		{
			Sortable = true;
			Searchable = true;
			Visible = true;
			Order = int.MaxValue;
		}

		public bool Searchable { get; set; }
		public bool Sortable { get; set; }
		public int Order { get; set; }
		public string DisplayName { get; set; }
		public Type DisplayNameResourceType { get; set; }
		public SortDirection SortDirection { get; set; }
		public string MRenderFunction { get; set; }
		public String CssClass { get; set; }
		public String CssClassHeader { get; set; }
		public string Width { get; set; }

		public bool Visible { get; set; }

		public override void ApplyTo(ColDef colDef, PropertyInfo pi)
		{
			colDef.DisplayName = this.ToDisplayName() ?? colDef.Name;
			colDef.Sortable = Sortable;
			colDef.Visible = Visible;
			colDef.Searchable = Searchable;
			colDef.SortDirection = SortDirection;
			colDef.MRenderFunction = MRenderFunction;
			colDef.CssClass = CssClass;
			colDef.CssClassHeader = CssClassHeader;
			colDef.CustomAttributes = pi.GetCustomAttributes().ToArray();
			colDef.Width = Width;
		}
	}
}