using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Insurance.Models
{
    public class ResetPassword
    {
        [Required,
        RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = " Customer ID must only contains letters or numbers - required.")]
        public string CustomerID { get; set; }
        [Required,
        RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = " Password must only contains letters or numbers - required.")]
        public string Password { get; set; }
    }
}