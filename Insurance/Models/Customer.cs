using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace Insurance.Models
{
    public class Customer
    {


        [key]
        public int ID;

        [Required,
         RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = " First Name must only contains letters or numbers and is required.")]
        public string FirstName { get; set; }

        [Required,
         RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Last Name must contains letters and is required")]
        public string LastName { get; set; }
        [Required,
            EmailAddress]
        public string Email { get; set; }
        [Required,
            Phone]
        public string Phone { get; set; }

        [Required,
         RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = " Customer ID must only contains letters or numbers - required.")]
        public string CustomerID { get; set; }
        [Required,
         RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = " password must only contains letters or numbers - required.")]
        public string CustomerPassword { get; set; }
        public string Encrypted { get; set;  }

        [Required,
          RegularExpression("^[a-zA-Z0-9#.\\s]*$", ErrorMessage = " address1 must only contains letters or numbers . # - required.")]
        public string Address1 { get; set; }
        [RegularExpression("^\\s|[a-zA-Z0-9#.\\s]*$", ErrorMessage = "address2 must contains letters or numbers . #  if entered")]
        public string Address2 { get; set; }
        [Required,
         RegularExpression("^[a-zA-Z]*$", ErrorMessage = "City must contains letters and is required")]
        public string City { get; set; }
        // drop down
        public string State { get; set; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "US zip must only contains numbers = required.")]
        public string Zip { get; set; }
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Invliad Promotion Code.")]
        public string PromotionCode { get; set; }

        public int PoicyID { get; set; }  


    }


    public class CustomerDBConnect : DbContext
    {

        public CustomerDBConnect()  
        {
        }

        public DbSet<Customer> Customers { get; set; }
    }

    public sealed class Configuration : DbMigrationsConfiguration<CustomerDBConnect>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;

            // Very important! Gives me enough time to wait for Azure
            // to initialize (Create -> Migrate -> Seed) the database.
            // Usually Azure needs 1-2 minutes so the default value of
            // 30 seconds is not big enough!
            CommandTimeout = 830;
        }
 
    }
}