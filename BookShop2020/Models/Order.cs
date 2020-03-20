using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BookShop2020.Models
{
    public class Order
    {
        [HiddenInput(DisplayValue = false)]// ID покупки
        public int Id { get; set; }

        [Required]
        [Range(1, 50000, ErrorMessage = "Недопустимая сумма")]
        [Display(Name = "Общая стоимость")]
        public decimal TotalPrice { get; set; }

        public DateTime Date { get; set; } // дата покупки
        public IEnumerable<Item> Items { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите фамилию")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Длина строки должна быть от 2 до 50 символов")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите свое имя")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Длина строки должна быть от 2 до 50 символов")]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Display(Name = "Адрес")]
        public string Address { get; set; }

        [Display(Name = "Способ доставки")]
        public string DeliveryMethod { get; set; }

        public string Status { get; set; }

    }

    public class Item
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        [HiddenInput(DisplayValue = false)]
        public int OrderId { get; set; }
    
        [HiddenInput(DisplayValue = false)]
        public int BookId { get; set; }
        
        [Required]
        [Range(1, 1000, ErrorMessage = "Недопустимое количество")]
        [Display(Name = "Количество")]
        public int Quantity { get; set; }

        public Book TheBook { get; set; }
    }


}