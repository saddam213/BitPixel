using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using DotMatrix.Common.Results;

namespace DotMatrix.Helpers
{
	public static class ValidationHelpers
	{
		public static bool IsWriterResultValid(this ModelStateDictionary modelState, IWriterResult writerResult)
		{
			bool valid = modelState.IsValid && writerResult.Success;
			if (!string.IsNullOrEmpty(writerResult.Message))
			{
				modelState.AddModelError(writerResult.Success ? "Success" : "Error", writerResult.Message);
			}
			return valid;
		}

		public static MvcHtmlString ModelValidationSummary<TModel>(this HtmlHelper<TModel> helper)
		{
			if (!helper.ViewData.ModelState.Any())
				return MvcHtmlString.Empty;

			var successMessage = helper.ViewData.ModelState.FirstOrDefault(x => x.Key == "Success").Value;
			if (successMessage != null)
			{
				var message = successMessage.Errors?.FirstOrDefault();
				if (message != null)
					return MvcHtmlString.Create(BuildAlert("success", message.ErrorMessage));
			}

			var errorMessage = helper.ViewData.ModelState.FirstOrDefault(x => x.Key == "Error").Value
											?? helper.ViewData.ModelState.FirstOrDefault().Value;
			if (errorMessage != null)
			{
				var message = errorMessage.Errors?.FirstOrDefault();
				if (message != null)
					return MvcHtmlString.Create(BuildAlert("danger", message.ErrorMessage));
			}

			return MvcHtmlString.Empty;
		}



		private static string BuildAlert(string type, string message)
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("<div class='alert-summary alert alert-{0} text-center' style='display:none'>", type));
			stringBuilder.AppendLine(string.Format("<p>{0}</p>", message));
			stringBuilder.AppendLine("<script>");
			stringBuilder.AppendLine("$(function(){ $('.alert-summary').fadeTo(15000, 500).slideUp(500, function () {$('.alert-summary').hide();	}); });");
			stringBuilder.AppendLine("</script>");
			stringBuilder.AppendLine("</div>");
			return stringBuilder.ToString();
		}

		public const int ImageMinimumBytes = 512;

		public static bool IsValidImage(this HttpPostedFileBase postedFile)
		{
			if (postedFile == null)
				return false;

			//-------------------------------------------
			//  Check the image mime types
			//-------------------------------------------
			if (!string.Equals(postedFile.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) &&
					!string.Equals(postedFile.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) &&
					!string.Equals(postedFile.ContentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) &&
					!string.Equals(postedFile.ContentType, "image/gif", StringComparison.OrdinalIgnoreCase) &&
					!string.Equals(postedFile.ContentType, "image/x-png", StringComparison.OrdinalIgnoreCase) &&
					!string.Equals(postedFile.ContentType, "image/png", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}

			//-------------------------------------------
			//  Check the image extension
			//-------------------------------------------
			var postedFileExtension = Path.GetExtension(postedFile.FileName);
			if (!string.Equals(postedFileExtension, ".jpg", StringComparison.OrdinalIgnoreCase)
					&& !string.Equals(postedFileExtension, ".png", StringComparison.OrdinalIgnoreCase)
					&& !string.Equals(postedFileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
					&& !string.Equals(postedFileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}

			//-------------------------------------------
			//  Attempt to read the file and check the first bytes
			//-------------------------------------------
			try
			{
				if (!postedFile.InputStream.CanRead)
				{
					return false;
				}
				//------------------------------------------
				//   Check whether the image size exceeding the limit or not
				//------------------------------------------ 
				if (postedFile.ContentLength < ImageMinimumBytes)
				{
					return false;
				}

				byte[] buffer = new byte[ImageMinimumBytes];
				postedFile.InputStream.Read(buffer, 0, ImageMinimumBytes);
				string content = System.Text.Encoding.UTF8.GetString(buffer);
				if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
						RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
				{
					return false;
				}
			}
			catch (Exception)
			{
				return false;
			}

			//-------------------------------------------
			//  Try to instantiate new Bitmap, if .NET will throw exception
			//  we can assume that it's not a valid image
			//-------------------------------------------

			try
			{
				using (var bitmap = new Bitmap(postedFile.InputStream))
				{
				}
			}
			catch (Exception)
			{
				return false;
			}
			finally
			{
				postedFile.InputStream.Position = 0;
			}

			return true;
		}
	}
}