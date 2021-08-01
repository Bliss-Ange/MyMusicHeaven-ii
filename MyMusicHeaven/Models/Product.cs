using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMusicHeaven.Models
{
    public class Product //make table structure, table name - Product
    {
        //here define the column we need //also use it for making the form structure

        public int ID { get; set; }
        public string ProductName { get; set; }
        public DateTime StockInDate { get; set; }
        public string Category { get; set; }
        public decimal ProductPrice { get; set; }
        public string Rating { get; set; }
        public byte[] ProductPicture { get; set; }
    }
}
