using System.Collections.Generic;
using DotMatrix.Common.Award;

namespace DotMatrix.Common.Users
{
	public class UserProfileViewModel
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		public int Pixels { get; set; }
		public int Clicks { get; set; }
		public int Awards { get; set; }
		public List<AwardUserListItemModel> AwardList { get; set; }
	}
}
