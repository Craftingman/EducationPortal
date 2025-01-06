using System;

namespace Core.ViewModels
{
    public class ArticleViewModel : MaterialViewModel
    {
        public DateTime PublishDate { get; set; }

        public string Source { get; set; }
    }
}