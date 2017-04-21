using DotMatrix.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Common.DataContext
{
	public interface IDataContext : IDisposable
	{
		Database Database { get; }
		int SaveChanges();
		Task<int> SaveChangesAsync();

		DbSet<User> Users { get; set; }
		DbSet<Entity.Pixel> Pixel { get; set; }
		DbSet<PixelHistory> PixelHistory { get; set; }
	}
}
