using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Insurance.Models
{
    public class CustomerSearch
    {
         
        [RegularExpression(@"^\s|[a-zA-Z0-9]*$", ErrorMessage = "Partial first must only contains letters numbers or blank.")]
        public string PartialFirstName { get; set; } 
        [RegularExpression(@"^\s|[a-zA-Z0-9]*$", ErrorMessage = "Partial last must only contains letters numbers  or blank.")]
        public string PartialLastName { get; set; }
        public List<Customer> customers { get; set; }
    }
}