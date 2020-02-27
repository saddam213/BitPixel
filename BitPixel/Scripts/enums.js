const Enums = {
	PixelType: Object.freeze({
		Empty: 0,
		User: 1,
		Fixed: 2
	}),
	AwardType: Object.freeze({
		Bronze: 0,
		Silver: 1,
		Gold: 2,
		Special: 10
	}),

	GetName: (enumType, enumKey) => {
		return Object.keys(enumType)[enumKey]
	},

	GetValue: (enumType, enumName) => {
		return enumType[enumName];
	}
};