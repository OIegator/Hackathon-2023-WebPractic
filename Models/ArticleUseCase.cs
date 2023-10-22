using System.Text;
using UnidecodeSharpCore;

namespace Hackathon.Models
{

	// use case для разделения логики
	public class ArticleUseCase
    {
		// создание правильной ссылки из имени и даты
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
				}
				else if (c == ' ')
				{
					result.Append('-');
				}
			}
			return result.ToString();
		}

	}
}
