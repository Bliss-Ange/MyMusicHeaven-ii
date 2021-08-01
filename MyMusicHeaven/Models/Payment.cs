using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMusicHeaven.Models
{
    public class Payment
    {
        public int ID { get; set; }
        public string CustomerName { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal TotalPayment { get; set; }
    }
}
