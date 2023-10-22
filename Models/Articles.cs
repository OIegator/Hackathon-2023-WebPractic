namespace Hackathon.Models
{// id, заголовок, текст, автор, время, ссылка
    public class Articles
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public string? Article { get; set; }
        public string? Author { get; set; }
        public DateTime? publication_time { get; set; }
        public string? Link { get; set; }
        public string? Style { get; set; }

    }
}
