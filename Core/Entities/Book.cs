using System.Collections.Generic;

namespace Core.Entities
{
    public class Book : Material
    {
        public string Authors { get; set; }
        
        public int Pages { get; set; }
        
        public string Format { get; set; }
        
        public int PublishYear { get; set; }
    }
}