using System;
using BitPixel.Datatables.Models;

namespace BitPixel.Datatables
{
	public abstract class DataTablesAttributeBase : Attribute
	{
		public abstract void ApplyTo(ColDef colDef, System.Reflection.PropertyInfo pi);
	}
}