using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using DotMatrix.Common.Award;
using DotMatrix.Common.DataContext;
using DotMatrix.Common.Results;
using DotMatrix.Enums;

namespace DotMatrix.Core.Award
{
	public class AwardWriter : IAwardWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<IWriterResult> UpdateAward(UpdateAwardModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				var award = await context.Award.FirstOrDefaultAsync(x => x.Id == model.Id);
				if (award == null)
					return new WriterResult(false, "Award not found");

				award.Name = model.Name;
				award.Description = model.Description;
				award.Icon = model.Icon;
				award.Level = model.Level;
				award.Status = model.Status;
				award.Rank = model.Rank;
				award.Points = model.Points;
				await context.SaveChangesAsync();
				return new WriterResult(true);
			}
		}

		public async Task<IWriterResult> AddUserAward(AddUserAwardModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				try
				{
					var award = await context.Award
						.Where(x => x.Type == model.Type)
						.FirstOrDefaultAsync();
					if (award == null)
						return new WriterResult(false, "AwardType not found");

					var version = GetAwardVersion(award.TriggerType, model.GameId, model.UserId, model.Version);
					if (string.IsNullOrEmpty(version))
						return new WriterResult(false, "Invalid request");

					if (await context.AwardHistory.AnyAsync(x => x.AwardId == award.Id && x.UserId == model.UserId && x.Version == version))
						return new WriterResult(false, "Award already exists");

					context.AwardHistory.Add(new Entity.AwardHistory
					{
						AwardId = award.Id,
						GameId = model.GameId,
						Points = award.Points,
						UserId = model.UserId,
						Version = version,
						VersionData = model.Version,
						Timestamp = DateTime.UtcNow
					});

					await context.SaveChangesAsync();
					await context.Database.Connection.ExecuteAsync(StoredProcedure.User_AuditPoints, new { UserId = model.UserId }, commandType: System.Data.CommandType.StoredProcedure);
				}
				catch (DbUpdateException e)
				{
					return new WriterResult(false, "Unable to insert duplicate award.");
				}

				return new WriterResult(true, "Successfully added award.");
			}
		}

		private static string GetAwardVersion(AwardTriggerType awardTriggerType, int? gameId, int userId, string version)
		{
			switch (awardTriggerType)
			{
				case Enums.AwardTriggerType.Once:
					return $"-";
				case Enums.AwardTriggerType.OncePerUser:
					return $"User:{userId}";
				case Enums.AwardTriggerType.MultiPerUser:
					return $"User:{userId}|{version}";
				case Enums.AwardTriggerType.OncePerGame:
					return $"Game:{gameId.Value}";
				case Enums.AwardTriggerType.OncePerUserPerGame:
					return $"Game:{gameId.Value}|User:{userId}";
				case Enums.AwardTriggerType.MultiPerUserPerGame:
					return $"Game:{gameId.Value}|User:{userId}|Version:{version}";
				default:
					break;
			}
			return null;
		}
	}
}
