using Insurance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;   

namespace Insurance.Controllers
{
    public class HomeController : Controller
    {


        public ActionResult RegisterCustomer()
        {
            // whey they are sigined in and come here route to review screen.!
            if(Session["CustomerID"] != null) { return RedirectToAction("CustomerReview"); }

            return View();
        }

        [HttpPost]
        public ActionResult RegisterCustomer(Customer customer)
        {
            if (ModelState.IsValid == false)
            {
                return View(); // show errors
            }

            ViewBag.Message = "";
            if (customer.CustomerPassword.Length < 6)
            {
               // ViewBag.Message = "Password should be at least six charaters.";
            }

           
            CustomerDBConnect context = new CustomerDBConnect();
            var message = CustomerEdits(context, customer);
           

            if (message != "")
            {
                ViewBag.Message = message;
                return View();
            }

            if(!Security.cryPromtionCode(customer.PromotionCode))
            {
                ViewBag.Message = "Unauthorized access - promotion code required.";
                return View();
            }

            customer.Encrypted = Security.cry(customer.CustomerPassword, 1);
            customer.CustomerPassword = "enc";

            context.Customers.Add(customer);
            context.SaveChanges();

            // store for assign policy screen.
            Session["CustomerID"] = customer.CustomerID;

            return View("RegisterSuccess");
        }

        public string CustomerEdits(CustomerDBConnect context, Customer customer)
        {
            var dup = from cust in context.Customers
                      where cust.CustomerID == customer.CustomerID
                      select cust;
            var message = "";
            foreach (var cust in dup)
            {
                message = "Customer ID already used.";
            }
            if (message != "") return message;

            var email = from cust in context.Customers
                        where cust.Email == customer.Email
                        select cust;
            message = "";
            foreach (var cust in email)
            {
                message = "Email already used.";
            }
            if (message != "") return message;

            return ""; // no errors
        }

        public ActionResult RegisterSuccess()
        {
            return View();
        }

        public ActionResult AssignPolicy()
        {
            return View();
        }
        [HttpGet]
        public ActionResult AssignPolicy(int? id)
        {
            if (id == null)
            {
                return View();
            }

            //find customer
            var inputCustID = Session["CustomerID"].ToString();
            CustomerDBConnect customerDBConnect = new CustomerDBConnect();
            var cust = from c in customerDBConnect.Customers
                       where c.CustomerID == inputCustID
                       select c;
            foreach (var item in cust)
            {
                item.PoicyID = (int)id;
            }
            customerDBConnect.SaveChanges();
            return RedirectToAction("CustomerReview");

        }

        public ActionResult CustomerReviewSignOn()
        {
            // whey they are sigined in and come here route to review screen.!
            if (Session["CustomerID"] != null) { return RedirectToAction("CustomerReview"); }

            return View();
        }
        [HttpPost]
        public ActionResult CustomerReviewSignOn(CustomerReviewSignOn s)
        {
            if (ModelState.IsValid == false)
            {
                return View();
            }

            CustomerDBConnect context = new CustomerDBConnect();
            var cus = from cust in context.Customers
                      where cust.CustomerID == s.CustomerID
                      select cust;

            var found = false;
            var pass = "";
            foreach (var cust in cus)
            {
                found = true;
                var passe = cust.Encrypted; ;
                pass = Security.cry(passe,-1);
            }

            if (!found)
            {
                ViewBag.Message = "Customer ID not found.";
                return View();
            }

            if (pass != s.Password)
            {
                ViewBag.Message = "Incorrect password.";
                return View();
            }

            Session["CustomerID"] = s.CustomerID;


            if (s.Action == "Personal Info")
            {
                // register and this should pass custid in temp data for policy assignment.
                TempData["CustomerID"] = s.CustomerID;
                return RedirectToAction("CustomerReview");
            }

            if (s.Action == "Select New Policy")
            {
                return RedirectToAction("AssignPolicy");
            }


            return View();
        }

        public ActionResult CustomerReview()
        {
            Customer customer = new Customer();

            CustomerDBConnect customerDBConnect = new CustomerDBConnect();

            var customerID = Session["CustomerID"].ToString();
            var lookup = from c in customerDBConnect.Customers
                         where c.CustomerID == customerID
                         select c;

            // first time populate with existing values

            foreach (var c in lookup)
            {
                ViewBag.screenCustomerID = c.CustomerID;
                customer.CustomerID = c.CustomerID;
                customer.CustomerPassword = Security.cry(c.Encrypted, -1); 
                customer.FirstName = c.FirstName;
                customer.LastName = c.LastName;
                customer.Address1 = c.Address1;
                customer.Address2 = c.Address2;
                customer.City = c.City;
                customer.State = c.State;
                customer.Zip = c.Zip;
                customer.Email = c.Email;
                customer.Phone = c.Phone;

            }


            return View(customer);
        }

        [HttpPost]
        public ActionResult CustomerReview(Customer customer)
        {
            if (ModelState.IsValid == false)
            {
                return View(); // show errors
            }

            ViewBag.Message = "";
            if (customer.CustomerPassword.Length < 6)
            {
                //ViewBag.Message = "Password should be at least six charaters.";
            }

            CustomerDBConnect customerDBConnect = new CustomerDBConnect();
            var cust = from c in customerDBConnect.Customers
                       where c.CustomerID == customer.CustomerID
                       select c;
            foreach (var item in cust)
            {
                item.FirstName = customer.FirstName;
                item.LastName = customer.LastName;
                item.Encrypted = Security.cry(customer.CustomerPassword, 1);
                item.CustomerPassword = "enr";
                item.Email = customer.Email;
                item.Address1 = customer.Address1;
                item.Address2 = customer.Address2;
                item.City = customer.City;
                item.State = customer.State;
                item.Zip = customer.Zip;
                item.Phone = customer.Phone;
            }
            customerDBConnect.SaveChanges();

            ViewBag.Message = "Update Successful.";

            return View();
        }

        public void ShowExceptionScreen(object sender, UnhandledExceptionEventArgs args)
        {
            // invokes diagnostic screen
            System.Exception ex = (System.Exception)args.ExceptionObject;
            ExceptionView("exception has occurred : " + ex.Message.ToString());

        }
        public ActionResult ExceptionView(string Message)
        {
            ViewBag.Message = Message;
            return View("ExceptionScreen");
        }

        public ActionResult Index()
        {
            {
                Session["splash"] = "index";

                Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CustomerDBConnect>());
                Database.SetInitializer(new DropCreateDatabaseIfModelChanges<PolicyDBConnect>()); // each time for test.
                Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ServiceDBConnect>());
                Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ClaimDBConnect>());
                SetUpPolicies();
                SetUpServices(); 
                AppDomain.CurrentDomain.UnhandledException += ShowExceptionScreen;
                Session["Splash"] = "Index"; // set splash page



            }
            return View();
        }
         
        public ActionResult Formal()
        {
            Session["Splash"] = "Formal"; // set splash page
            return View();
        }
         

        public void SetUpPolicies()
        {
            //  not best practice but for demo purposes.
            PolicyDBConnect context = new PolicyDBConnect();
            Policy[] array = new Policy[]
            {
                new Policy { PolicyID = 1, Copay = 200, Deductible = 500, PolicyName = "Standard", Rate = 250, PolicyType = "Medical" },
                new Policy { PolicyID = 2, Copay = 20, Deductible = 100, PolicyName = "Excellent", Rate = 150, PolicyType = "Medical" }

            };
            context.Policies.AddRange(array);
            context.SaveChanges();
        }

        public void SetUpServices()
        {
            //  not best practice but for demo purposes.
            ServiceDBConnect context = new ServiceDBConnect();
            Service[] array = new Service[]
            {
                new Service { Id = 1,  PolicyID = 1, ServiceName = "Medical Exam" },
                new Service { Id = 2,  PolicyID = 1, ServiceName = "X Ray"},

                new Service { Id = 1,  PolicyID = 2, ServiceName = "Medical Exam" },
                new Service { Id = 2,  PolicyID = 2, ServiceName = "X Ray"},
                new Service { Id = 3,  PolicyID = 2, ServiceName = "Check Up" },
                new Service { Id = 4,  PolicyID = 2, ServiceName = "Some Procedure"}


            };
            context.Services.AddRange(array);
            context.SaveChanges();
        }




        public ActionResult About()
        {
            ViewBag.Message = "Sample Insurance Application for Demo purposes.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult AddPolicy()
        {
             

            return View();
        }

        [HttpPost]
        public ActionResult AddPolicy(Policy policy)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            PolicyDBConnect context = new PolicyDBConnect();
            context.Policies.Add(policy);
            context.SaveChanges();
            ViewBag.Message = "Policy " + policy.PolicyName + " added.";

            return View();
        }

        public ActionResult PolicySearch()
        {
            // this screen passes a poilcy name first part or if left blank finds all pcys.
            // matches policy name
            return View();
        }

        [HttpPost]
        public ActionResult PolicySearch(PolicySearch policysearch)
        {
            if (!ModelState.IsValid)
            {

                return View();
            }
            PolicyDBConnect context = new PolicyDBConnect();
            bool all = (policysearch.PolicyName == null) ? true : false;
            if (all)
            {
                var pol = from p in context.Policies
                          select p;
                policysearch.policies = pol.ToList();
            }
            else
            {
                var pol = from p in context.Policies
                          where p.PolicyName.StartsWith(policysearch.PolicyName)
                          select p;
                policysearch.policies = pol.ToList();
            }

            return View(policysearch);
        }

        public ActionResult CustomerSearch()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CustomerSearch(CustomerSearch customerSearch)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            FindMatchingCustomers(customerSearch);
            return View(customerSearch);
        }

        public void FindMatchingCustomers(CustomerSearch s)
        {
            var first = s.PartialFirstName;
            var last = s.PartialLastName;
            CustomerDBConnect context = new CustomerDBConnect();
            List<string> list = new List<string>();

            if (first == null && last == null)
            {
                var match = from c in context.Customers
                            orderby c.LastName, c.FirstName
                            select c;
                s.customers = match.ToList();
            }

            if (first != null && last == null)
            {
                var match = from c in context.Customers
                            orderby c.LastName, c.FirstName
                            where c.FirstName.StartsWith(first)
                            select c;
                s.customers = match.ToList();

            }
            if (first == null && last != null)
            {
                var match = from c in context.Customers
                            orderby c.LastName, c.FirstName
                            where c.LastName.StartsWith(last)
                            select c;
                s.customers = match.ToList();
            }
            if (first != null && last != null)
            {
                var match = from c in context.Customers
                            orderby c.LastName, c.FirstName
                            where c.FirstName.StartsWith(first) &&
                                  c.LastName.StartsWith(last)
                            select c;
                s.customers = match.ToList();

            }
            return;

        }

        public ActionResult AdminSignIn()
        {
            var SignedIn = Session["adminSignedIn"];
            if (SignedIn != null)
            {
                ViewBag.Message = "(you are still signed in - please select an action.)";
            }
            return View();
        }

        [HttpPost]
        public ActionResult AdminSignIn(AdminSignIn adminsignin)
        {
            Session["CustomerID"] = null;

            if (ModelState.IsValid == false)
            {
                return View();
            }
            // this is not a good practice for passwords but this is a demo site!
            // use identity provider or best practices method currently known.
            var validate = false;
            var admin = "";
            var pw = "";
            Security.crya(out admin, out pw);
            if (adminsignin.AdminId == admin && adminsignin.Password == pw)
            {
                validate = true;
            }
           
            var SignedIn = Session["adminSignedIn"];
            if (SignedIn != null)
            {
                ViewBag.Message = "(you are still signed in - please select an action.)";
                validate = true;
            }
            if (!validate)
            {
                ViewBag.Message = "Invalid adm user id or password";
                return View();
            }

            // retain if admin returns to sign in screen keep signed in. 
            Session["adminSignedIn"] = "SignedIn";

            var Action = adminsignin.Action;

            if (Action == "Customer Search")
            {

                return RedirectToAction("CustomerSearch");
            }

            if (Action == "Policy Search")
            {
                return RedirectToAction("PolicySearch");
            }

            if (Action == "Policy Add")
            {
                return RedirectToAction("AddPolicy");
            }

            if (Action == "Reset Password")
            {
                return RedirectToAction("ResetPassword");
            }


            return View();
        }


        public List<SelectListItem> UtilityPolicySearch()
        {
            // produces item list of policies in system
            // used in assign policy screen and policy search screens.

            List<SelectListItem> a = new List<SelectListItem>();

            PolicyDBConnect context = new PolicyDBConnect();
            var list = from p in context.Policies
                       select p;
            foreach (var p in list)
            {
                SelectListItem b = new SelectListItem();
                b.Text = p.PolicyName;
                b.Value = p.PolicyID.ToString();
                a.Add(b);
            }
            return a;
        }

        [HttpGet]
        public ActionResult FileClaim(int? id)
        {
            // nav link check if signed in if not route to signon.
            var check = Session["CustomerID"];
            if(check == null)
            {
                RedirectToAction("CustomerReviewSignOn");
                return View();
            }

            // link to FileClaim Screen from other screens.
            if (id == null)
            {
                Claim claim = new Claim();
                claim.DateService = DateTime.Now;
                claim.DetailNote = "";
                claim.Clinic = "";
                claim.Doctor = "";
                
                // populate service list
                claim.serviceItems = PopulateServices(); 
                // redirect
                if (claim.serviceItems.Count == 0)
                {
                    ViewBag.Message = "A policy needs to be assigned before filing claims!";
                    RedirectToAction("AssignPolicy");
                }

                return View(claim);
            }
            return View();
        }
        [HttpPost]
        public ActionResult FileClaim(Claim claim)
        {
            // nav link check if signed in if not route to signon.
            var check = Session["CustomerID"];
            if (check == null)
            {
                return RedirectToAction("CustomerReviewSignOn");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }
            // edit date 
            ViewBag.Message = "";
            DateTime valueDate;
            bool validDate = DateTime.TryParse(claim.DateService.ToString(), out valueDate);
            if(!validDate)
            {
                ViewBag.Message = "Date is not valid or Missing";
                return View();
            }




            claim.CustomerID = Session["CustomerID"].ToString();
            ClaimDBConnect claimDBConnect = new ClaimDBConnect();
            claimDBConnect.Claims.Add(claim);
            claimDBConnect.SaveChanges();

            // process file claim update.
            ViewBag.Message = "Claim Filed Successfully";
            return RedirectToAction("CustomerReview");
        }

        public List<SelectListItem> PopulateServices()
        {
            // find policy id in customer database and look up services in the service database.
            var custID = Session["CustomerID"].ToString();
            CustomerDBConnect customerDBConnect = new CustomerDBConnect();
            var pol = from c in customerDBConnect.Customers
                      where custID == c.CustomerID
                      select c;
            int policyID = 0;
            foreach (var p in pol)
            {
                policyID = p.PoicyID;
            }

            if(policyID == 0)
            {
                return null; // no policy set 
            }

            List<SelectListItem> items = new List<SelectListItem>();

            ServiceDBConnect serviceDBConnect = new ServiceDBConnect();
            var serviceList = from service in serviceDBConnect.Services
                              where policyID == service.PolicyID
                              select service;
            foreach (var s in serviceList)
            {
                SelectListItem item = new SelectListItem();
                item.Value = s.ServiceName;
                item.Text = s.ServiceName;
                items.Add(item);
            }
             

            return items;
        }
        public ActionResult ShowClaimHistory()
        {
            // nav link check if signed in if not route to signon.
            var check = Session["CustomerID"];
            if (check == null)
            {
               return RedirectToAction("CustomerReviewSignOn");
            }
            return View();
        }

        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPassword resetPassword)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            if(!Security.pwReset(resetPassword.Password))
            {
                ViewBag.Message = "Unathorized attempt to reset password.";
                return View();
            }

            // validate customer id supplied.

            CustomerDBConnect customerDBConnect = new CustomerDBConnect();
            var cust = from c in customerDBConnect.Customers
                       where c.CustomerID == resetPassword.CustomerID
                       select c;
            bool valid = false;
            foreach(var c in cust)
            {
                valid = true;
            }
            if(!valid)
            {
                ViewBag.Message = "Customer ID not found.";
                return View();
            }

            // update the password 
            foreach (var item in cust)
            { 
                item.Encrypted = Security.cry( Security.GetTemp(), 1);
                item.CustomerPassword = "rst";
            }
            customerDBConnect.SaveChanges();

            ViewBag.Message = "Password Reset";
            resetPassword.Password = "";
            resetPassword.CustomerID = "";

            return View();
        }

    } 
}