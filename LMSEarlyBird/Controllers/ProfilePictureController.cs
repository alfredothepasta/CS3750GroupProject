using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using LMSEarlyBird.Data;
using LMSEarlyBird.Models;
using Microsoft.AspNetCore.Identity;
using LMSEarlyBird.Interfaces;
using LMSEarlyBird.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Drawing;
using Microsoft.AspNetCore.Hosting.Server;

namespace LMSEarlyBird.Controllers
{
	public class ProfilePictureController : Controller
	{
		private readonly IWebHostEnvironment _webHost;

		public ProfilePictureController(IWebHostEnvironment webHost)
		{
			_webHost = webHost;
		}

		/// <summary>
		/// Returns the profile picture page
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult ProfilePicture()
		{
            return View();
		}

        /// <summary>
        /// Saves the profile picture uploaded via POST
        /// </summary>
        /// <param name="imgFile"></param>
        /// <returns></returns>
        [HttpPost]
		public async Task<IActionResult> ProfilePicture(IFormFile imgFile)
		{
			// Check if the file is an image
			if(imgFile == null) {
                ViewData["Message"] = "Please select an image file";
                return View();
            }
			string imgext = Path.GetExtension(imgFile.FileName);
			if (imgext == ".jpg" || imgext == ".png" || imgext == ".JPG" || imgext == ".PNG")
			{

                // Get the path to save the image
                var fileName = User.GetUserId() + "ProfilePicture" + Path.GetExtension(imgFile.FileName);
				var saveimg = Path.Combine(_webHost.WebRootPath, "images/ProfilePictures", fileName);


                // Delete existing image with the same file name

                // Get a list of files matching the pattern
                string[] matchingFiles = Directory.GetFiles("wwwroot/images/ProfilePictures/", $"{User.GetUserId()}ProfilePicture.*");

                // Delete each matching file
                foreach (string file in matchingFiles)
                {
					System.IO.File.Delete(file);
                }

                using (var uploading = new FileStream(saveimg, FileMode.Create))
				{
					await imgFile.CopyToAsync(uploading);
					ViewData["Message"] = "The Selected File " + imgFile.FileName + " saved successfully.";
				}
			}
			else
			{
				ViewData["Message"] = "Only .jpg and .png files are allowed";
			}
			return View();
		}
	}
}
