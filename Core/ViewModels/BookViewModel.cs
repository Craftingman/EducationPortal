namespace Core.ViewModels
{
    public class BookViewModel : MaterialViewModel
    {
        public string Authors { get; set; }
        
        public int Pages { get; set; }
        
        public string Format { get; set; }
        
        public int PublishYear { get; set; }
    }
}