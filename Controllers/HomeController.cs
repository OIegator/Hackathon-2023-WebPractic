using Hackathon.Data;
using Hackathon.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using UnidecodeSharpCore;

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
        public IActionResult Publish(string? author, string? title, string? subtitle, string? text)
        {
			if (string.IsNullOrEmpty(author) || string.IsNullOrEmpty(title) ||  string.IsNullOrEmpty(text)) {
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
           // newArticle.Id = articles.Max(i=>i.Id+1);
            newArticle.Author = author;
			newArticle.Title = title;
			newArticle.Subtitle = subtitle;
			newArticle.Article = text;
			newArticle.publication_time = DateTime.Now;
			newArticle.Link = GenerateLink(title, DateTime.Now);
            db.Articles.Add(newArticle);
            db.SaveChanges();
            
            Console.WriteLine("Успешный успех");
            return Ok();
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}