﻿using System;
using System.Threading.Tasks;
using DotMatrix.Common.Game;
using DotMatrix.Common.Pixel;
using DotMatrix.Enums;
using DotMatrix.QueueService.Common;

namespace DotMatrix.Core.Pixel
{
	public class PixelWriter : IPixelWriter
	{
		public IGameReader GameReader { get; set; }
		public IQueueHubClient QueueHubClient { get; set; }

		public async Task<AddClickResponse> AddClick(int userId, AddClickRequest model)
		{
			try
			{
				var game = await GameReader.GetGame(model.GameId);
				if (game == null)
					return new AddClickResponse { Success = false, Message = "Game not found" };

				if (!Constant.IsValidLocation(model.X, model.Y, game.Width, game.Height))
					return new AddClickResponse { Success = false, Message = "Pixel X,Y must be within range 0-999" };

				var result = await QueueHubClient.SubmitClick(new SubmitClickRequest
				{
					IsApi = false,
					UserId = userId,
					X = model.X,
					Y = model.Y,
					GameId = game.Id
				});

				return new AddClickResponse
				{
					Success = result.Success,
					Message = result.Message,
					UserPoints = result.UserPoints,

					GameId = game.Id,
					GameName = game.Name
				};
			}
			catch (Exception)
			{
				return new AddClickResponse { Success = false, Message = "Failed to add new click, unknown error" };
			}
		}

		public async Task<AddPixelResponse> AddPixel(int userId, AddPixelRequest model)
		{
			try
			{
				var game = await GameReader.GetGame(model.GameId);
				if (game == null)
					return new AddPixelResponse { Success = false, Message = "Game not found" };

				if (!Constant.IsValidColor(model.Color))
					return new AddPixelResponse { Success = false, Message = "Failed to add new pixel, Pixel color must be valid hex color code #000000-#FFFFFF" };

				if (!Constant.IsValidLocation(model.X, model.Y, game.Width, game.Height))
					return new AddPixelResponse { Success = false, Message = "Failed to add new pixel, Pixel X,Y must be within range 0-999" };

				if (Constant.PixelPoints > model.MaxPoints)
					return new AddPixelResponse { Success = false, Message = "Failed to add new pixel, Points are greater than MaxPoints" };

				var result = await QueueHubClient.SubmitPixel(new SubmitPixelRequest
				{
					IsApi = false,
					UserId = userId,

					X = model.X,
					Y = model.Y,
					Color = model.Color,
					Type = PixelType.User,
					Points = Constant.PixelPoints,
					MaxPoints = model.MaxPoints,
					GameId = game.Id
				});

				return new AddPixelResponse
				{
					Success = result.Success,
					Message = result.Message,

					PixelId = result.PixelId,
					NewPoints = result.NewPoints,

					TeamId = result.TeamId,
					TeamName = result.TeamName,
					UserId = result.UserId,
					UserName = result.UserName,
					UserPoints = result.UserPoints,

					GameId = game.Id,
					GameName = game.Name,

					IsPrizeWinner = result.IsPrizeWinner,
					PrizeId = result.PrizeId,
					PrizeName = result.PrizeName,
					PrizePoints = result.PrizePoints,
					PrizeDescription = result.PrizeDescription
				};
			}
			catch (Exception)
			{
				return new AddPixelResponse { Success = false, Message = "Failed to add new pixel, unknown error" };
			}
		}
	}
}
