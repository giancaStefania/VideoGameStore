using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CG.MVC5.Stefania.Models
{
    public class Category
    {
        public int Id { set; get; }
        [Required]
        public string Name { set; get; }
    }
}