using System.ComponentModel.DataAnnotations;

namespace BookShop2020.Models
{
    public class BookViewModel
    {
        public Book TheBook { get; set; }
        [Display(Name = "Отзывы")]
        public int ReviewNumber { get; set; }
    }

    public class ReviewViewModel
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }
    }
}