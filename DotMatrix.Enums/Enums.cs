namespace DotMatrix.Enums
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

	public enum PixelClickType : byte
	{
		GetPixel = 0,
		AddPixel = 1
	}

	public enum PrizeType : byte
	{
		Points = 0,
		Crypto = 1
	}

	public enum PrizeStatus : byte
	{
		Unclaimed = 0,
		Processing = 1,
		Claimed = 2
	}

	public enum AwardType : short
	{
		Registration = 0
	}

	public enum AwardTriggerType : byte
	{
		OneUser = 0,
		EachUserOnce = 1,
		EachUserMulti = 2
	}

	public enum AwardStatus : byte
	{
		Active = 0,
		Disabled = 10
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
}
