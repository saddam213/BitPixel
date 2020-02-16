using System;
using System.Reflection.Emit;

namespace DotMatrix.Datatables.DynamicLinq
{
	public class DynamicProperty
	{
		readonly string name;
		readonly Type type;

		public DynamicProperty(string name, Type type, CustomAttributeBuilder attributeBuilder = null)
		{
			if (name == null) throw new ArgumentNullException("name");
			if (type == null) throw new ArgumentNullException("type");
			this.name = name;
			this.type = type;
			CustomAttributeBuilder = attributeBuilder;

		}

		public string Name
		{
			get { return name; }
		}

		public Type Type
		{
			get { return type; }
		}

		public CustomAttributeBuilder CustomAttributeBuilder
		{
			get;
			private set;
		}

	}
}