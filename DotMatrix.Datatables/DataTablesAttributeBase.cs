using System;
using DotMatrix.Datatables.Models;

namespace DotMatrix.Datatables
{
	public abstract class DataTablesAttributeBase : Attribute
	{
		public abstract void ApplyTo(ColDef colDef, System.Reflection.PropertyInfo pi);
	}
}