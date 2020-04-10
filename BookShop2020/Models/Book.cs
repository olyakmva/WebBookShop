using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace BookShop2020.Models
{
    [Table("Books")]
    public class Book
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; } // ID книги

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
        [Display(Name = "Автор")]
        public string Author { get; set; } // автор книги

        [Required(ErrorMessage = "Пожалуйста введите название")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
        [Display(Name = "Название")]
        public string Name { get; set; } // название книги
                
        
        [Required]
        [Range(0, 20000, ErrorMessage = "Недопустимая цена")]
        [Display(Name = "Цена")]
        public decimal Price { get; set; } // цена
        [Required]
        [Range(1700, 2020, ErrorMessage = "Недопустимый год")]
        [Display(Name = "Год издания")]
        public int Year { get; set; } 

        [HiddenInput(DisplayValue = false)]
        public int? CategoryId { get; set; }

        [Display(Name = "Жанр")]
        public Category Genre { get; set; }
        [HiddenInput(DisplayValue = false)]
        public string ImageUrl { get; set; }

        [Required]
        [Range(0, 20000, ErrorMessage = "Недопустимое количество")]
        [Display(Name = "Количество")]
        public int Number { get; set; }
    }
}