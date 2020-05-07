using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace BookShop2020.Models
{
    [Table("Clients")]
    public class Client
    {
        [HiddenInput(DisplayValue = false)]
        public string Id { get; set; } // ID покупателя
        
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Длина строки должна быть от 2 до 50 символов")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; } 

        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Display(Name = "Адрес")]
        public string Address { get; set; }

        [Required]
        [HiddenInput(DisplayValue = false)]
        public decimal TotalOrdersCost { get; set; } 

        [Required]
        [HiddenInput(DisplayValue = false)]
        public int OrdersNumber { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int ReviewsNumber { get; set; }

        [Required]
        [HiddenInput(DisplayValue = false)]
        public int CurrentDiscount { get; set; }
    }
}