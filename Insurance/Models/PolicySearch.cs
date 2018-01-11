using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Insurance.Models
{
    public class PolicySearch
    {
      
      [RegularExpression(@"^\s|[a-zA-Z0-9]*$", ErrorMessage = "Policy  must only contains letters numbers.")]
        public string PolicyName { get; set; }
        public List<Policy> policies { get; set; }
    }
}