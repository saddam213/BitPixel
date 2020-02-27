using System;
using System.Data.Entity;
using System.Threading.Tasks;

using BitPixel.Entity;

namespace BitPixel.Common.DataContext
{
	public interface IDataContext : IDisposable
	{
		Database Database { get; }
		int SaveChanges();
		Task<int> SaveChangesAsync();

		DbSet<User> Users { get; set; }

		DbSet<EmailTemplate> EmailTemplate { get; set; }
		DbSet<EmailOutbox> EmailOutbox { get; set; }

		DbSet<Entity.Game> Games { get; set; }
		DbSet<Entity.Team> Team { get; set; }
		DbSet<TeamMember> TeamMember { get; set; }

		DbSet<Entity.Pixel> Pixel { get; set; }
		DbSet<PixelHistory> PixelHistory { get; set; }

		DbSet<Entity.Click> Click { get; set; }
		DbSet<Entity.Prize> Prize { get; set; }

		DbSet<Entity.Award> Award { get; set; }
		DbSet<Entity.AwardHistory> AwardHistory { get; set; }

		DbSet<PaymentAddress> PaymentAddress { get; set; }
		DbSet<PaymentMethod> PaymentMethod { get; set; }
		DbSet<PaymentUserMethod> PaymentUserMethod { get; set; }
		DbSet<PaymentReceipt> PaymentReceipt { get; set; }
	}
}
