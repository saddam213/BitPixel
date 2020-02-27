using System;
using System.Collections;
using System.Collections.Generic;

namespace BitPixel.Base
{
	public static class ObjectExtensions
	{
	
		public static string GetBytesReadable(this long value)
		{
			// Get absolute value
			long absolute_i = (value < 0 ? -value : value);
			// Determine the suffix and readable value
			string suffix;
			double readable;
			if (absolute_i >= 0x1000000000000000) // Exabyte
			{
				suffix = "EB";
				readable = (value >> 50);
			}
			else if (absolute_i >= 0x4000000000000) // Petabyte
			{
				suffix = "PB";
				readable = (value >> 40);
			}
			else if (absolute_i >= 0x10000000000) // Terabyte
			{
				suffix = "TB";
				readable = (value >> 30);
			}
			else if (absolute_i >= 0x40000000) // Gigabyte
			{
				suffix = "GB";
				readable = (value >> 20);
			}
			else if (absolute_i >= 0x100000) // Megabyte
			{
				suffix = "MB";
				readable = (value >> 10);
			}
			else if (absolute_i >= 0x400) // Kilobyte
			{
				suffix = "KB";
				readable = value;
			}
			else
			{
				return value.ToString("0 B"); // Byte
			}
			// Divide by 1024 to get fractional value
			readable = (readable / 1024);
			// Return formatted number with suffix
			return readable.ToString("0.### ") + suffix;
		}

		public static byte[] ToByteArray(this ulong value)
		{
			var size = 8;
			var result = new byte[size];
			for (var i = 0; i < size; i++)
			{
				var bitOffset = (size - (i + 1)) * 8;
				result[i] = (byte)((value & ((ulong)0xFF << bitOffset)) >> bitOffset);
			}
			return result;
		}

		public static ulong ToUInt64(this byte[] data)
		{
			var requiredSize = 8;
			if (data.Length != requiredSize)
				throw new ArgumentException($"The byte-array \"{nameof(data)}\" must be exactly {requiredSize} bytes.");

			var result = 0ul;
			for (var i = 0; i < requiredSize; i++)
			{
				result |= ((ulong)data[i] << ((requiredSize - (i + 1)) * 8));
			}
			return result;
		}

		public static bool IsGreaterThan(this byte[] value, byte[] other)
		{
			return value.CompareTo(other) > 0;
		}

		public static bool IsLessThan(this byte[] value, byte[] other)
		{
			return value.CompareTo(other) < 0;
		}

		public static bool IsGreaterOrEqual(this byte[] value, byte[] other)
		{
			return value.CompareTo(other) > 0 || value.CompareTo(other) == 0;
		}

		public static bool IsLessOrEqual(this byte[] value, byte[] other)
		{
			return value.CompareTo(other) < 0 || value.CompareTo(other) == 0;
		}

		public static int CompareTo(this byte[] value, byte[] other)
		{
			if (value == null && other == null)
				return 0;
			else if (value == null)
				return -1;
			else if (other == null)
				return 1;
			return ((IStructuralComparable)value).CompareTo(other, Comparer<byte>.Default);
		}

		public static bool IsGreaterThan<T>(this T value, T other) where T : IComparable
		{
			return value.CompareTo(other) > 0;
		}

		public static bool IsGreaterOrEqual<T>(this T value, T other) where T : IComparable
		{
			return value.CompareTo(other) > 0 || value.CompareTo(other) == 0;
		}

		public static bool IsLessThan<T>(this T value, T other) where T : IComparable
		{
			return value.CompareTo(other) < 0;
		}

		public static bool IsLessOrEqual<T>(this T value, T other) where T : IComparable
		{
			return value.CompareTo(other) < 0 || value.CompareTo(other) == 0;
		}
	}
}
