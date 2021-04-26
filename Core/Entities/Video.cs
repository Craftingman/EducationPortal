using System;

namespace Core.Entities
{
    public class Video : Material
    {
        public TimeSpan Duration { get; set; }
        
        public string Resolution { get; set; }
    }
}