namespace Gallery.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }

        public Photo(int id, string title, string url, string thumbnailUrl)
        {
            Id = id;
            Title = title;
            Url = url;
            ThumbnailUrl = thumbnailUrl;
        }
    }
}
