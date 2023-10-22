﻿using Hackathon.Data;
using Hackathon.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using UnidecodeSharpCore;
using IronBarCode;
using System.Drawing;
using System.Drawing.Imaging;

namespace Hackathon.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IWebHostEnvironment webHostEnvironment;
		private readonly DBContext db;


        public HomeController(ILogger<HomeController> logger, DBContext _db, IWebHostEnvironment _webHostEnvironment)
		{
			db = _db;
			_logger = logger;
			webHostEnvironment = _webHostEnvironment;
		}

		public IActionResult Index(Articles? article)
		{
            
			if (article == null)
			{
				Articles article1 = new Articles();
                return View(article1);
            }
			else {
                return View(article);
            }
         
			
		}

		public IActionResult Privacy()
		{
			return View();
		}

        [HttpPost("/publish")]
        public IActionResult Publish(string? author, string? title, string? subtitle, string? text, string? style)
        {
			if (string.IsNullOrEmpty(author) || string.IsNullOrEmpty(title) ||  string.IsNullOrEmpty(text) || string.IsNullOrEmpty(style)) {
				return NotFound();
			}
		
			var newArticle = new Articles();
           // newArticle.Id = articles.Max(i=>i.Id+1);
            newArticle.Author = author;
			newArticle.Title = title;
			newArticle.Subtitle = subtitle;
			newArticle.Article = text;
			newArticle.publication_time = DateTime.Now;
			newArticle.Style = style;
			newArticle.Link = GenerateLink(title, DateTime.Now);
            db.Articles.Add(newArticle);
            db.SaveChanges();

            return CreateQRCode(newArticle.Link);
        }
        public ActionResult Show(string articleUrl)
        {
            var articles = db.Articles;
            Articles? neededArticle = articles.FirstOrDefault(a=> a.Link == articleUrl);
			if (neededArticle == null) {
				//todo
				return NotFound();
			}

            return View("Index", neededArticle);
        }

        public string GenerateLink(string title, DateTime date)
        {
            // Удаление пробелов и транслитерация символов
            string sanitizedTitle = TransliterateAndSanitize(title);

            // Форматирование даты в виде "месяц-день"
            string formattedDate = date.ToString("MM-dd");

            // Склейка частей URL
            string url = $"{sanitizedTitle}-{formattedDate}";

            return url;
        }

		// транслит строки, изменение регистра, замена пробелов
        private string TransliterateAndSanitize(string input)
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in input.ToLower().Unidecode())
            {
                if (char.IsLetterOrDigit(c))
                {
                    result.Append(c);
                } else if (c == ' ')
                {
                    result.Append('-');
                }
            }
            return result.ToString();
        }


		public IActionResult CreateQRCode(string link)
		{
			string articleUrl = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value + "/article=" + link;
			return Json(new {url = articleUrl });
		}

		public ActionResult GenerateQRCode(string text)
		{
			GeneratedBarcode barcode = QRCodeWriter.CreateQrCode(text, 200);

			barcode.SetMargins(10);
			Bitmap bitmap = barcode.ToBitmap();
			barcode.ChangeBarCodeColor(IronSoftware.Drawing.Color.Black);
			using (MemoryStream stream = new MemoryStream())
			{
				bitmap.Save(stream, ImageFormat.Png);
				return File(stream.ToArray(), "image/png");
			}
		}

			//todo решить что делать
			[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View();
		}
	}
}