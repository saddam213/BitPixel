﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotMatrix.Common.Deposits
{
	public interface IDepositReader
	{
		Task<List<DepositModel>> GetDeposits(string userId);
	}
}
