namespace DotMatrix.Common.DataContext
{
	public static class StoredProcedure
	{
		public const string User_AuditPoints = "[dbo].[User_AuditPoints]";
		public const string Game_AddClick = "[dbo].[Game_AddClick]";
		public const string Game_AddPixel = "[dbo].[Game_AddPixel]";

		public const string Game_GetPixelRectangle = "[dbo].[Game_GetPixelRectangle]";
		public const string Payment_AssignAddress = "[dbo].[Payment_AssignAddress]";

		public const string Cache_Game_GetInitial = "[dbo].[Cache_Game_GetInitial]";
		public const string Cache_Game_GetUpdates = "[dbo].[Cache_Game_GetUpdates]";

		public const string WalletService_UpdateWithdraw = "[dbo].[WalletService_UpdateWithdraw]";
		public const string WalletService_GetWithdrawals = "[dbo].[WalletService_GetWithdrawals]";

		public const string Award_AddAwardHistory = "[dbo].[Award_AddAwardHistory]";
	}
}
