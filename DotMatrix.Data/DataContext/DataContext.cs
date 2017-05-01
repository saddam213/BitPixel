using DotMatrix.Common.DataContext;
using DotMatrix.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Data.DataContext
{
	public class DataContext : DbContext, IDataContext
	{
		public DataContext()
			: base("DefaultConnection")
		{
			Database.Log = (e) => Debug.WriteLine(e);
		}

		public DbSet<User> Users { get; set; }
		public DbSet<Pixel> Pixel { get; set; }
		public DbSet<PixelHistory> PixelHistory { get; set; }
		public DbSet<Deposit> Deposit { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Conventions.Add(new DecimalPropertyConvention(38, 8));

			modelBuilder.Entity<User>().ToTable("AspNetUsers");

			modelBuilder.Entity<PixelHistory>().HasRequired(x => x.User);
			modelBuilder.Entity<PixelHistory>().HasRequired(x => x.Pixel).WithMany(x => x.History);

			modelBuilder.Entity<Deposit>().HasRequired(x => x.User).WithMany(x => x.Deposits);
		}
	}
}
