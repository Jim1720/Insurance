using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Insurance.Models
{
    public class AdminSignIn
    {
        
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Invalid Admin ID")]
        public string AdminId { get; set; }
        
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Invalid Password")]
        public string Password { get; set; }
        public string Action { get; set; }
    }
}