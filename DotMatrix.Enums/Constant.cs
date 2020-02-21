using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DotMatrix.Enums
{
	public static class Constant
	{
		public const int SystemUserId = 1;
		public const int DefaultGameId = 1;
		//public const int ClicksPerSecond = 3;
		public const int ClicksPerDay = 200000;

		public const int PixelPoints = 1;
		//public const int Width = 800;
	//	public const int Height = 600;

		public static bool IsValidLocation(int x, int y, int width, int height)
		{
			return (x >= 0 && x < width) && (y >= 0 && y < height);
		}

		public static bool IsValidColor(string hexColor)
		{
			return Regex.IsMatch(hexColor, "#[0-9a-fA-F]{6}");
		}

		public static bool IsValidPixel(int x, int y, int width, int height, string hexColor)
		{
			return IsValidLocation(x, y, width, height) && IsValidColor(hexColor);
		}
	}

}
