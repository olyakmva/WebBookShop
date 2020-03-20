using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using System.Collections.Generic;

namespace BookShop2020.Models
{
    [Table("Categories")]
    public class Category
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; } // ID 

        [Required(ErrorMessage = "Пожалуйста введите название жанра")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
        [Display(Name = "Жанр")]
        public string Name { get; set; } // название 
        public IEnumerable<Book> Books { get; set; }
    }
}