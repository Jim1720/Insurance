using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Insurance.Models
{
    public class CustomerReviewSignOn
    {

        [Required,
         RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = " Customer ID must only contains letters or numbers and is required.")]
        public string CustomerID { get; set; }

        [Required,
         RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = " Password must only contains letters or numbers and is required.")]
        public string Password { get; set; }

        [Required,
         RegularExpression("^[\\sa-zA-Z]*$", ErrorMessage = " Action must only contains letters or numbers or spaces and is required.")]
        public string Action { get; set; }
    }
}