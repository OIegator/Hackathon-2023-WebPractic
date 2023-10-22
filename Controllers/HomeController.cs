using Hackathon.Data;
using Hackathon.Models;
using Microsoft.AspNetCore.Mvc;
using IronBarCode;
using System.Drawing;
using System.Drawing.Imaging;

namespace Hackathon.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly DBContext db;
		private readonly ArticleUseCase articleUseCase;

        public HomeController(ILogger<HomeController> logger, DBContext _db)
		{
			db = _db;
			_logger = logger;
			articleUseCase= new ArticleUseCase();
		}

		// основной экран, для уменьшение количества кода и простоты судейства было решено не разбивать на 2 отдельных экрана почти идентичный по функционалу код
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

		// публикация статьи из заполненных полей
        [HttpPost("/publish")]
        public IActionResult Publish(string? author, string? title, string? subtitle, string? text, string? style)
        {
			if (string.IsNullOrEmpty(author) || string.IsNullOrEmpty(title) ||  string.IsNullOrEmpty(text) || string.IsNullOrEmpty(style)) {
				return NotFound();
			}
		
			var newArticle = new Articles();
            newArticle.Author = author;
			newArticle.Title = title;
			newArticle.Subtitle = subtitle;
			newArticle.Article = text;
			newArticle.publication_time = DateTime.Now;
			newArticle.Style = style;
			newArticle.Link = articleUseCase.GenerateLink(title, DateTime.Now);
            db.Articles.Add(newArticle);
            db.SaveChanges();

            return CreateArticleUrl(newArticle.Link);
        }
		// показ найденной статьи
        public ActionResult Show(string articleUrl)
        {
            var articles = db.Articles;
            Articles? neededArticle = articles.LastOrDefault(a=> a.Link == articleUrl);
			if (neededArticle == null) {
				return NotFound();
			}

            return View("Index", neededArticle);
        }

		// Создание ссылки новой статьи
		public IActionResult CreateArticleUrl(string link)
		{
			string articleUrl = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value + "/article=" + link;
			return Ok(new {url = articleUrl });
		}

		// СОЗДАНИЕ ИЗОБРАЖЕНИЯ QR
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

	}
}