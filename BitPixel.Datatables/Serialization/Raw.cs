﻿namespace BitPixel.Datatables.Serialization
{
	public class Raw
	{
		readonly string _value;
		public Raw(string value)
		{
			_value = value;
		}
		public override string ToString()
		{
			return _value;
		}
	}
}