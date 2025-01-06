using System;

namespace Core.Entities
{
    public class Article : Material
    {
        public DateTime PublishDate { get; set; }

        public string Source { get; set; }
    }
}