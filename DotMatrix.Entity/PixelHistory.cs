using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Entity
{
	public class PixelHistory
	{
		[Key]
		public int Id { get; set; }

		public int PixelId { get; set; }
		public string UserId { get; set; }
		public string Color { get; set; }
		public decimal Price { get; set; }

		public DateTime Timestamp { get; set; }

		[ForeignKey("PixelId")]
		public virtual Pixel Pixel { get; set; }

		[ForeignKey("UserId")]
		public virtual User User { get; set; }
	}
}
