using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc; 


namespace Insurance.Models
{

    public static class PolicyLookupClass 
    {        
        
        public static Policy PolicyLookup(string CustomerID)
        {
            CustomerDBConnect customerDBConnect = new CustomerDBConnect();

            int policyID = 0;
            var clist = from c in customerDBConnect.Customers
                        where c.CustomerID == CustomerID
                        select c;
            foreach(var c in clist)
            {
                policyID = c.PoicyID;
            }

            if(policyID == 0)
            {
                return null;
            }

            PolicyDBConnect policyDBConnect = new PolicyDBConnect();

            var list = from p in policyDBConnect.Policies
                                 where p.PolicyID == policyID
                                 orderby p.PolicyID
                                 select p;

            Policy policy = null;
            foreach(var p in list)
            {
                policy = p;
            }

            return policy;

            
        }
    }
    
}