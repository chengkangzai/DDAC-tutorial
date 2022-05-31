using System;

namespace NewNewTry.Models
{
    public class Payment
    {
        public int ID { get; set; }
        public string CustomerName { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
