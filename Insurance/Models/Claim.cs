using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Insurance.Models
{
    public class Claim
    {
        [key]
        public int Id { get; set; }
        // hidden passed to screen . no edits.
        public string CustomerID { get; set; }
        // drop down
        [Required]
        public string Service { get; set; }
        [Required]  // add edit for date ..... or model attribute TODO.
        public DateTime DateService { get; set; } 
        [RegularExpression("^[#a-zA-Z0-9\\s]*$",ErrorMessage ="Detail Note must be letter/number only, # and spaces.")]
        public string DetailNote { get; set; }
        // drop down no edits
        [Required]
        public string Location { get; set; }
        [Required,
            RegularExpression(@"^\s|[a-zA-Z0-9.#\s]*$" ,ErrorMessage="Refer Clinic/Doc must be letter, number, with . # or blank.")]
        public string Clinic { get; set; }
        [Required,
            RegularExpression(@"^\s|[a-zA-Z0-9.\s]*$" ,ErrorMessage="Doctor must be letter/number with . or space")]
        public string Doctor { get; set; }
        [Required,
         RegularExpression("^[a-zA-Z0-9]*$",ErrorMessage ="First Name must be letters or numbers only")]
        public string First { get; set; }
        [Required,
           RegularExpression("^[a-zA-Z]*$" , ErrorMessage ="Last Name must be letters or numbers only")]
        public string Last { get; set; }
        // no edits on diag,proc they are drop downs
        [Required,
        RegularExpression(@"^none|[0-9]*$")]
        public string Diag1 { get; set; }
        [RegularExpression(@"^none|\s|[0-9]*$")]
        public string Diag2 { get; set; }
        [Required,
        RegularExpression(@"^none|[0-9]*$")]
        public string Proc1 { get; set; }
        [RegularExpression(@"^none|\s|[0-9]*$")]
        public string Proc2 { get; set; }
        [RegularExpression(@"^none|\s|[0-9]*$")]
        public string Proc3 { get; set; }

        
        [RegularExpression(@"^\s|[a-zA-Z0-9.\s]*$",ErrorMessage ="Referral from clinic/doc must be letter/number/period")]
        public string ReferralFromClinicOrDoc { get; set; }
        // drop down no edits 
        public string PaymentAction { get; set; }

        public List<SelectListItem> serviceItems;

    }




    public class ClaimDBConnect : DbContext
    {

        public ClaimDBConnect()  
        {
        }

        public DbSet<Claim> Claims { get; set; }
    }


    public sealed class Configuration1 : DbMigrationsConfiguration<ClaimDBConnect>
    {
        public Configuration1()
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