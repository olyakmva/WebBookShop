using System.ComponentModel.DataAnnotations;

namespace BookShop2020.Models
{
    public class GenreSoldViewModel
    {
        [Display(Name = "Название")]
        public string Name { get; set; } // название категории
        [Display(Name = "Количество")]
        public int Quantity { get; set; }
        [Display(Name = "Общая стоимость")]
        public int TotalPrice { get; set; }
    }
}