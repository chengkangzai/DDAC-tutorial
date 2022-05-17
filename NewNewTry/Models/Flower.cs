using System;
using System.ComponentModel.DataAnnotations;

namespace NewNewTry.Models
{
    public class Flower
    {
        public int ID { get; set; }
        [Display(Name = "Flower Name")]
        public string FlowerName { get; set; }
        public decimal FlowerPrice { get; set; }
        public string FlowerType { get; set; }
        public DateTime FlowerSentToShop { get; set; }
    }
}
