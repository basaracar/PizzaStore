using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaStore.Models
{
    public class Pizza
    {
        public int Id { get; set; }

        [Display(Name = "Pizza Adı")]
        [Required(ErrorMessage = "Pizza Adı Gerekli")]
        public string Name { get; set; }

        [Display(Name = "Pizza Açıklaması")]
        [Required(ErrorMessage = "Pizza Tanımlaması Gerekli")]
        public string Description { get; set; }
        
        [Display(Name = "Pizza Fotoğrafı")]
        public string? Photo { get; set; }
        [Required(ErrorMessage = "Pizza Fiyatı Gerekli")]
        [Display(Name = "Pizza Fiyatı")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:C}")]
        public decimal Price { get; set; }

        public Category? Category { get; set; }
        public int CategoryId { get; set; }

    }
}
