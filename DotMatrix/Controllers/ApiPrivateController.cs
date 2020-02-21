//using System.Threading.Tasks;
//using System.Web.Http;

//using DotMatrix.Api;
//using DotMatrix.Common.Api;
//using DotMatrix.Common.Pixel;
//using DotMatrix.Enums;

//using Microsoft.AspNet.Identity;

//namespace DotMatrix.Controllers
//{
//	[ApiAuthentication]
//	public class ApiPrivateController : ApiController
//	{
//		public IPixelWriter PixelWriter { get; set; }
//		public IPixelReader PixelReader { get; set; }

//		[HttpPost]
//		public async Task<ApiResponse> AddPixel(AddPixelRequest model)
//		{
//			var result = await PixelWriter.AddPixel(User.Identity.GetUserId<int>(), model);
//			if (!result.Success)
//				return new ApiResponse { Success = false, Message = result.Message };

//			await PixelHub.UpdateFrontEndPixel(new PixelHubAddPixelRequest
//			{
//				X = model.X,
//				Y = model.Y,
//				Color = model.Color,
//				Type = PixelType.User,

//				PixelId = result.PixelId,
//				UserId = result.UserId,
//				UserName = result.UserName,
//				UserPoints = result.UserPoints,
//				NewPoints = result.NewPoints,

//				IsApi = true,
//			});
//			return new ApiResponse { Success = true, Message = $"Added new pixel at X:{model.X} Y:{model.Y}" };
//		}

//		[HttpPost]
//		public async Task<ApiResponse> GetPixel(GetPixelRequest model)
//		{
//			if (!Constant.IsValidLocation(model.X, model.Y))
//				return new ApiResponse { Success = false, Message = "Invalid input data." };

//			return new ApiResponse
//			{
//				Success = true,
//				Data = await PixelReader.GetPixel(model.X, model.Y)
//			};
//		}
//	}
//}
