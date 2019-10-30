using System.ComponentModel.DataAnnotations;

namespace AAAService
{
    public class DbText
    {
        [Key]
        public string FeedId { get; set; }
        public string ContentText { get; set; }

        public DbText() { }

        public DbText(string feedId, string contentText)
        {
            FeedId = feedId;
            ContentText = contentText;
        }
    }

    public class DbLink
    {
        [Key]
        public string FeedId { get; set; }
        public string ContentLinks { get; set; }

        public DbLink() { }

        public DbLink(string feedId, string contentLinks)
        {
            FeedId = feedId;
            ContentLinks = contentLinks;
        }
    }

    public class DbImage
    {
        [Key]
        public string FeedId { get; set; }
        public string ContentImages { get; set; }

        public DbImage() { }

        public DbImage(string feedId, string contentImages)
        {
            FeedId = feedId;
            ContentImages = contentImages;
        }
    }
}