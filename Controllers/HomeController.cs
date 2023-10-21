using Hackathon.Data;
using Hackathon.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Hackathon.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
        private readonly DBContext db;


        public HomeController(ILogger<HomeController> logger, DBContext _db)
		{
			db = _db;
			_logger = logger;
		}

		public IActionResult Index()
		{
            Articles article = new Articles();
			return View(article);
		}

		public IActionResult Privacy()
		{
			return View();
		}

        [HttpPost("/publish")]
        public IActionResult Publish(string? author, string? title, string? subtitle, string? text)
        {
			if (string.IsNullOrEmpty(author) || string.IsNullOrEmpty(title) || string.IsNullOrEmpty(subtitle) || string.IsNullOrEmpty(text)) {
				//todo всплывающее окно
				return NotFound();
			}
			var articles = db.Articles;
			foreach (var article in articles)
			{
				Console.WriteLine(article.Article);
			}
			Console.WriteLine(author);
			Console.WriteLine(title);
			Console.WriteLine(subtitle);
			Console.WriteLine(text);
			var newArticle = new Articles();
            newArticle.Id = articles.Max(i=>i.Id+1);
            newArticle.Author = author;
			newArticle.Title = title;
			newArticle.Subtitle = subtitle;
			newArticle.Article = text;
			newArticle.publication_time = DateTime.Now;
          //  db.Articles.Add(newArticle);
           // db.SaveChanges();
            Console.WriteLine("Успешный успех");
            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}