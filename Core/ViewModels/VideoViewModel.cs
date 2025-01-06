using System;

namespace Core.ViewModels
{
    public class VideoViewModel : MaterialViewModel
    {
        public TimeSpan Duration { get; set; }
        
        public string Resolution { get; set; }
    }
}