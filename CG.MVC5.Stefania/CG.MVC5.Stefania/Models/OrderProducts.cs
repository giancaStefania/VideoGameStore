using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CG.MVC5.Stefania.Models
{
    public class OrderProducts
    {
        public int Id { set; get; }
        public DateTime Date { set; get; }
        public string OrderState { set; get; }
        public List<ProductE> Products { set; get; }

        public double TotalPrice()
        {
            double result = 0;
            foreach(ProductE product in Products)
            {
                if(product.Validate)
                    result = result + ((double.Parse(product.Price) * product.Quantity));
            }
            return result;
        }
        public string GetOnlyDate()
        {
            string [] date = Date.ToString().Split(' ');
            return date[0];
        }
    }
}