namespace BitPixel.Enums
{
	public enum PixelType : byte
	{
		Empty = 0,
		User = 1,
		Fixed = 2
	}

	public enum PaymentMethodType : byte
	{
		None = 0,
		Crypto = 1
	}
	
	public enum PaymentMethodStatus : byte
	{
		Ok = 0,
		Suspended = 1,
		Deleted = 10
	}
	public enum PaymentReceiptStatus : byte
	{
		Pending = 0,
		Processing = 1,
		Complete = 2
	}

	public enum PaymentUserMethodStatus : byte
	{
		Active = 0,
		Deleted = 10
	}

	public enum ClickType : byte
	{
		Click = 0,
		Pixel = 1,
		None = 10
	}

	public enum PrizeType : byte
	{
		Points = 0,
		Crypto = 1
	}

	public enum PrizeStatus : byte
	{
		Unclaimed = 0,
		Pending = 1,
		Processing = 2,
		Claimed = 10
	}

	public enum AwardType : short
	{
		None = 0,
		Registration = 1,
		Avatar = 2,

		ImageBestPicture = 100,
		VideoPestAnimation = 200,

		ClickDaily = 1000,
		Click100 = 1001,
		Click1000 = 1002,
		Click10000 = 1003,
		Click100000 = 1004,
		Click1000000 = 1005,
		ClickFourCorners = 1006,
		ClickVerticalLine = 1007,
		ClickHorizontalLine = 1008,
		ClickBorderLines = 1009,

		PixelDaily = 2000,
		Pixel100 = 2001,
		Pixel1000 = 2002,
		Pixel10000 = 2003,
		Pixel100000 = 2004,
		Pixel1000000 = 2005,

		PixelFourCorners = 2006,
		PixelVerticalLine = 2007,
		PixelHorizontalLine = 2008,
		PixelBorderLines = 2009,
		PixelLastPixel = 2010,

		PixelOverwrite = 2100,
		PixelOverwrite100 = 2101,
		PixelOverwrite1000 = 2102,
		PixelOverwrite10000 = 2103,
		PixelOverwrite100000 = 2104,
		PixelOverwrite1000000 = 2105,

		Number420 = 3000,
		Number69 = 3001,
		Number911 = 3002,

		ColorRGB = 4000,
		ColorBlind = 4001,
		ColorRainbow = 4002,
		Color10 = 4003,
		Color100 = 4004,
		Color1000 = 4005,
		
	}

	public enum AwardTriggerType : byte
	{
		Once = 0,
		OncePerUser = 1,
		MultiPerUser = 2,
		OncePerGame = 3,
		OncePerUserPerGame = 4,
		MultiPerUserPerGame = 5
	}

	public enum AwardStatus : byte
	{
		Active = 0,
		Disabled = 10
	}

	public enum AwardLevel : byte
	{
		Bronze = 0,
		Silver = 1,
		Gold = 2,
		Special = 10
	}

	public enum EmailStatus : byte
	{
		Pending = 0,
		Failed = 1,
		Processed = 2
	}

	public enum EmailTemplateType : byte
	{
		Registration = 0,
		PasswordReset = 1,
	}


	public enum GameType : byte
	{
		TreasureHunt = 0,
		TeamBattle = 1
	}

	public enum GameStatus : byte
	{
		NotStarted = 0,
		Started = 1,
		Paused = 2,
		Finished = 3,
		Deleted = 10
	}

	public enum GamePlatform : byte
	{
		Desktop = 0,
		Mobile = 1
	}

	public enum GameEndType : byte
	{
		LastPixel = 0,
		Timestamp = 1
	}
}
