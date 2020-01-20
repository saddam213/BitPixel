using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotMatrix.Common.DataContext;
using DotMatrix.Common.Award;
using System.Data.Entity;
using DotMatrix.Enums;
using System.Data.Entity.Infrastructure;
using Dapper;

namespace DotMatrix.Core.Award
{
	public class AwardWriter : IAwardWriter
	{
		public IDataContextFactory DataContextFactory { get; set; }

		public async Task<AddUserAwardResult> AddAward(AddUserAwardModel model)
		{
			using (var context = DataContextFactory.CreateContext())
			{
				try
				{
					var award = await context.Award
						.Where(x => x.Type == model.Type)
						.FirstOrDefaultAsync();
					if (award == null)
						return new AddUserAwardResult { Success = false, Message = "AwardType not found" };

					var version = GetAwardVersion(award.TriggerType, model.UserId, model.Version);
					if (await context.AwardHistory.AnyAsync(x => x.AwardId == award.Id && x.UserId == model.UserId && x.Version == version))
						return new AddUserAwardResult { Success = false, Message = "Award already exists" };

					context.AwardHistory.Add(new Entity.AwardHistory
					{
						AwardId = award.Id,
						Points = award.Points,
						UserId = model.UserId,
						Version = version,
						VersionData = model.Version,
						Timestamp = DateTime.UtcNow
					});

					await context.SaveChangesAsync();
					await context.Database.Connection.ExecuteAsync(StoredProcedure.AuditPoints, new { UserId = model.UserId }, commandType: System.Data.CommandType.StoredProcedure);
				}
				catch (DbUpdateException e)
				{
					return new AddUserAwardResult { Success = false, Message = "Unable to insert duplicate award." };
				}

				return new AddUserAwardResult { Success = true, Message = "Successfully added award." };
			}
		}

		private static string GetAwardVersion(AwardTriggerType awardTriggerType, int userId, string version)
		{
			switch (awardTriggerType)
			{
				case Enums.AwardTriggerType.OneUser:
					return "-1";
				case Enums.AwardTriggerType.EachUserOnce:
					return $"{userId}";
				case Enums.AwardTriggerType.EachUserMulti:
					return $"{userId}-{version}";
				default:
					break;
			}
			return DateTime.UtcNow.ToString();
		}
	}
}
