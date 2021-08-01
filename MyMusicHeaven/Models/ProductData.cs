using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyMusicHeaven.Data; //so that can find the context class to check the table connection

namespace MyMusicHeaven.Models
{
    public class ProductData //to help on hardcode your data direct to your db when first load the program
    {
        public static void Initialize(IServiceProvider serviceProvider) 
        {
            using(var context = new MyMusicHeavenNewContext(serviceProvider.GetRequiredService<DbContextOptions<MyMusicHeavenNewContext>>()))
            {
                // Look for any product. 
                if (context.Product.Any() && context.Payment.Any()) //check whether the table is empty or not
                {
                    return;
                }

                if (!context.Product.Any()) { //if table empty then add in the data
                    context.Product.AddRange( //hard code date to the table
                        new Product
                        {
                            ProductName = "BTS Disc",
                            StockInDate = DateTime.Parse("2021-06-15"),
                            ProductPrice = 10.99M,
                            Category = "Disc",
                            Rating = "5"
                        },
                        new Product
                        {
                            ProductName = "Twice T-Shirt",
                            StockInDate = DateTime.Parse("2021-06-15"),
                            ProductPrice = 70.00M,
                            Category = "T-Shirt",
                            Rating = "4"
                        }
                    );
                }

                if (!context.Payment.Any())
                { //if table empty then add in the data
                    context.Payment.AddRange( //hard code date to the table
                        new Payment
                        {
                            CustomerName = "Suga",
                            PaymentDate = DateTime.Parse("2021-06-16"),
                            TotalPayment = 102.30M
                        }
                    );
                }


                context.SaveChanges();


            }
            
                
            
            
        }

    }
}
