using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace BookShop2020.Models
{
    [Table("Cart")]
    public class CartItem
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        [HiddenInput(DisplayValue = false)]
        public string CartId { get; set; }
        [HiddenInput(DisplayValue = false)]
        public int BookId { get; set; }
        [Required]
        [Range(1, 10000, ErrorMessage = "Некорректное значение")]
        [Display(Name = "Количество")]
        public int Quantity { get; set; }
        public Book SelectBook { get; set; }
    }
}