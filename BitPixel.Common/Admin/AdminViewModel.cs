using System.Collections.Generic;
using BitPixel.Common.Award;
using BitPixel.Common.Game;
using BitPixel.Common.Payment;
using BitPixel.Common.Prize;
using BitPixel.Common.Users;

namespace BitPixel.Common.Admin
{
	public class AdminGameViewModel
	{
		public List<GameModel> Games { get; set; }
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
