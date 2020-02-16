using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;

using DotMatrix.Common.DataContext;
using DotMatrix.Entity;

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

		public DbSet<EmailTemplate> EmailTemplate { get; set; }
		public DbSet<EmailOutbox> EmailOutbox { get; set; }

		public DbSet<Game> Games { get; set; }
		public DbSet<Team> Teams { get; set; }
		public DbSet<Pixel> Pixel { get; set; }
		public DbSet<PixelHistory> PixelHistory { get; set; }

		public DbSet<Click> Click { get; set; }
		public DbSet<Prize> Prize { get; set; }

		public DbSet<Award> Award { get; set; }
		public DbSet<AwardHistory> AwardHistory { get; set; }

		public DbSet<PaymentAddress> PaymentAddress { get; set; }
		public DbSet<PaymentMethod> PaymentMethod { get; set; }
		public DbSet<PaymentUserMethod> PaymentUserMethod { get; set; }
		public DbSet<PaymentReceipt> PaymentReceipt { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			modelBuilder.Conventions.Add(new DecimalPropertyConvention(38, 8));

			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

			modelBuilder.Entity<User>().ToTable("Users");
			modelBuilder.Entity<User>().HasRequired(x => x.Team).WithMany(x => x.Users);

			modelBuilder.Entity<PixelHistory>().HasRequired(x => x.User);
			modelBuilder.Entity<PixelHistory>().HasRequired(x => x.Pixel).WithMany(x => x.History);

			modelBuilder.Entity<PaymentReceipt>().HasRequired(x => x.User).WithMany(x => x.Deposits);

			modelBuilder.Entity<AwardHistory>().HasOptional(x => x.Game);
			modelBuilder.Entity<AwardHistory>().HasRequired(x => x.User).WithMany(x => x.Awards);
		}
	}
}
