using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewNewTry.Models
{
    public class Order
    {
        [Key]
        public string OrderId { get; set; }
        // [ForeignKey("User")]
        public string CustomerName { get; set; }
        public int PurchaseItemNumber { get; set; }
        public decimal PurchaseTotalAmount { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
}
