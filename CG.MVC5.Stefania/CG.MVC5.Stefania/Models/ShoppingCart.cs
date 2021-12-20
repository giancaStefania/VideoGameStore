using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CG.MVC5.Stefania.Models
{
    public class ShoppingCart
    {
        public List<ProductE> Products { set; get; }
        public double Total { set; get; }

        /*public double GetTotal()
        {
            double result = 0;
            foreach(ProductE product in Products)
            {
                result = result + product.Price;
            }
        }*/
    }
}