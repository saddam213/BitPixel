using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Entity
{
	public class Pixel
	{
		[Key]
		public int Id { get; set; }
		public string PixelKey { get; set; }
		public string UserId { get; set; }

		public int X { get; set; }
		public int Y { get; set; }
		public byte R { get; set; }
		public byte G { get; set; }
		public byte B { get; set; }

		public DateTime LastUpdate { get; set; }

		public virtual ICollection<PixelHistory> History { get; set; }

	}
}
