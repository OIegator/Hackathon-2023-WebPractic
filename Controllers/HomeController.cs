using Hackathon.Data;
using Hackathon.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using UnidecodeSharpCore;
using IronBarCode;


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
                Console.WriteLine(" fdsg " + article.Article);
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
				//todo всплывающее окно
				return NotFound();
			}
		
			Console.WriteLine(author);
			Console.WriteLine(title);
			Console.WriteLine(subtitle);
			Console.WriteLine(text);
			Console.WriteLine(style);
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
            
            Console.WriteLine("Успешный успех");
            return CreateQRCode(newArticle.Link);
        }
        public ActionResult Show(string articleUrl)
        {

            Console.WriteLine($"articleUrl: {articleUrl}");

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

			Console.WriteLine(articleUrl);
			string imageUrl;
			try
			{
				GeneratedBarcode barcode = QRCodeWriter.CreateQrCode(articleUrl, 200);
		
				barcode.SetMargins(10);
				barcode.ChangeBarCodeColor(IronSoftware.Drawing.Color.Black);
				string path = Path.Combine(webHostEnvironment.WebRootPath, "GeneratedQRCode");
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				string filePath = Path.Combine(webHostEnvironment.WebRootPath, "GeneratedQRCode/qrcode.png");
				barcode.SaveAsPng(filePath);
				string fileName = Path.GetFileName(filePath);
				imageUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + "/GeneratedQRCode/" + fileName;
				ViewBag.QrCodeUri = imageUrl;
				Console.WriteLine(imageUrl);
			}
			catch (Exception)
			{
				//todo что-то делать
				return NotFound();
			}
			return Json(new {imgUrl = imageUrl, url = articleUrl });
		}



		//todo решить что делать
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View();
		}
	}
}