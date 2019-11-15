using System.Collections.Generic;

namespace Gallery.Models
{
    public class Album
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public IList<Photo> Photos { get; set; }

        public Album(int id, string title, IList<Photo> photos)
        {
            Id = id;
            Title = title;
            Photos = photos;
        }

    }
}
