using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CG.MVC5.Stefania.Models
{
    public class ProductE
    {
        public int Id { set; get; }
        [Required]
        public string Name { set; get; }
        [Required]
        public string Description { set; get; }
        [Required]
        [Range(0.01, float.MaxValue, ErrorMessage = "Price must be > 0 and an valid float number")]
        public string Price { set; get; }
        public HttpPostedFileBase Image1_file { set; get; }
        public HttpPostedFileBase Image2_file { set; get; }
        public HttpPostedFileBase Image3_file { set; get; }
        public string Image1_string { set; get; }
        public string Image2_string { set; get; }
        public string Image3_string { set; get; }
        [Required]
        public string Category { set; get; }
        [Required]
        public string Stock { set; get; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be > 0 and an integer value")]
        public int Quantity { set; get; }
        public string Platform { set; get; }
        public bool Validate { set; get; }

    }
}