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
    public class Policy
    {



        public int Id { get; set; }

        public int PolicyID { get; set; }

        [Required,
        RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = " Policy Name must only contains letters or numbers.")]
        public string PolicyName { get; set; }
        [Required,
        Range(0, double.MaxValue , ErrorMessage = "Please enter valid rate")]
        public double Rate { get; set; }
        [Required,
        Range(0, double.MaxValue, ErrorMessage = "Please enter valid copay")]
        public double Copay { get; set; }
        [Required,
        Range(0, double.MaxValue, ErrorMessage = "Please enter valid deductible")]
        public double Deductible { get; set; }
        [Required,
         RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = " Policy type must only contains letters.")]
        public string PolicyType  { get; set; } 
        // for screen assign selection
        public string Select { get; set; }
    }

    

    public class PolicyDBConnect : DbContext
    {
        public PolicyDBConnect()  
        {
        }

        public DbSet<Policy> Policies { get; set; }
    }


    public sealed class Configuration2 : DbMigrationsConfiguration<PolicyDBConnect>
    {
        public Configuration2()
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