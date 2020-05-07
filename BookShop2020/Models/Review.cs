using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BookShop2020.Models
{
    public class Review
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; } // ID 
        [HiddenInput(DisplayValue = false)]
        public int BookId { get; set; } 
        [HiddenInput(DisplayValue = false)]
        public string ClientId { get; set; }  
        [Required]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Длина отзыва должна быть от 10 до 2000 символов")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Текст отзыва")]
        public string Text { get; set; } 
    }
}