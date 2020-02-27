using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Base.Extensions
{
	public static class StringExtensions
	{
		public static bool IsNullOrEmpty(this string value)
		{
			return string.IsNullOrEmpty(value);
		}

		public static bool IsDigits(this string value)
		{
			if (string.IsNullOrEmpty(value))
				return false;

			foreach (var c in value)
				if (c < '0' || c > '9')
					return false;

			return true;
		}

		public static string Truncate(this string value, int maxLength)
		{
			if (string.IsNullOrEmpty(value))
				return value;

			return value.Substring(0, Math.Min(value.Length, maxLength));
		}

		public static string AddOrdinal(this int num)
		{
			if (num <= 0)
				return num.ToString();

			switch (num % 100)
			{
				case 11:
				case 12:
				case 13:
					return num + "th";
			}

			switch (num % 10)
			{
				case 1:
					return num + "st";
				case 2:
					return num + "nd";
				case 3:
					return num + "rd";
				default:
					return num + "th";
			}

		}
	}
}
