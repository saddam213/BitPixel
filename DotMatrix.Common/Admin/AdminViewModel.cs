using System.Collections.Generic;
using DotMatrix.Common.Award;
using DotMatrix.Common.Game;
using DotMatrix.Common.Payment;
using DotMatrix.Common.Prize;
using DotMatrix.Common.Users;

namespace DotMatrix.Common.Admin
{
	public class AdminViewModel
	{
	}

	public class AdminGameViewModel
	{
		public List<GameModel> Games { get; set; }
		public List<PrizeItemModel> Prizes { get; set; }
	}

	public class AdminUserViewModel
	{
		public List<UserModel> Users { get; set; }
	}

	public class AdminAwardViewModel
	{
		public List<AwardModel> Awards { get; set; }
	}
}
