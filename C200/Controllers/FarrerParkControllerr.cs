using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace C200.Controllers
{
    public class FarrerParkController : Controller
    {
        public IActionResult HomeNoAcc()
        {
            return View();
        }

        public IActionResult HomeAcc()
        {
            return View();
        }
        public IActionResult AccountRegistration()
        {
            return View();
        }
        public IActionResult AccountRegistrationPost()
        {

            IFormCollection form = HttpContext.Request.Form;
            string retrivedName = form["Name"].ToString().Trim();
            string retrivedNRIC = form["NRIC"].ToString().Trim();
            string retrivedDOB = form["DOB"].ToString();


            //EXAMPLE 2020-11-17
            /*if (retrivedDOB != null)
            {
                ViewData["Message"] = retrivedDOB;
                return View("AccountRegistration");
            }*/


            //Checks if all fields are filled in
            if (ValidUtl.CheckIfEmpty(retrivedName, retrivedNRIC, retrivedDOB) == true)
            {
                ViewData["Message"] = "All fields must be filled in";
                ViewData["MsgType"] = "warning";
                return View("AccountRegistration");
            }

            //Checks if name field contain a digit
            if (retrivedName.Any(char.IsDigit) == true)
            {
                ViewData["Message"] = "Name entered must not contain a number";
                ViewData["MsgType"] = "warning";
                return View("AccountRegistration");
            }

            //Check for format of NRIC entered
            if ((retrivedNRIC.Substring(0, 1).ToLower().Equals("t")
                || retrivedNRIC.Substring(0, 1).ToLower().Equals("s")
                || retrivedNRIC.Substring(0, 1).ToLower().Equals("g")
                || retrivedNRIC.Substring(0, 1).ToLower().Equals("f"))
                && (retrivedNRIC.Substring(8, 1).IsInteger() == false)
                && ((retrivedNRIC.Substring(1, 7).IsInteger() == true) && (retrivedNRIC.Substring(1, 7).Length == 7))
                && (retrivedNRIC.Length == 9))
            {
                //Check validity of NRIC using CheckSum exact accurate method instead of string patterns
                int sum = 0;
                int remainder = 0;
                string firstCharNRIC = "";
                string lastCharNRIC = "";
                string expectedLastCharNRIC = "";

                firstCharNRIC = retrivedNRIC.Substring(0, 1).ToLower();
                lastCharNRIC = retrivedNRIC.Substring(8, 1).ToLower();

                //Weight of NRIC characters (from first to last) = 2 7 6 5 4 3 2
                sum = (Int32.Parse(retrivedNRIC.Substring(1, 1)) * 2)
                    + (Int32.Parse(retrivedNRIC.Substring(2, 1)) * 7)
                    + (Int32.Parse(retrivedNRIC.Substring(3, 1)) * 6)
                    + (Int32.Parse(retrivedNRIC.Substring(4, 1)) * 5)
                    + (Int32.Parse(retrivedNRIC.Substring(5, 1)) * 4)
                    + (Int32.Parse(retrivedNRIC.Substring(6, 1)) * 3)
                    + (Int32.Parse(retrivedNRIC.Substring(7, 1)) * 2);

                //S or T: 0=J, 1=Z, 2=I, 3=H, 4=G, 5=F, 6=E, 7=D, 8=C, 9=B, 10=A
                //F or G: 0 = X, 1 = W, 2 = U, 3 = T, 4 = R, 5 = Q, 6 = P, 7 = N, 8 = M, 9 = L, 10 = K
                
                if (firstCharNRIC.Equals("t") || firstCharNRIC.Equals("g"))
                {
                    sum = sum + 4;
                }

                remainder = sum % 11;

                if (firstCharNRIC.Equals("s") || firstCharNRIC.Equals("t"))
                {
                    if (remainder == 0)
                    {
                        expectedLastCharNRIC = "J";
                    }
                    else if (remainder == 1)
                    {
                        expectedLastCharNRIC = "Z";
                    }
                    else if (remainder == 2)
                    {
                        expectedLastCharNRIC = "I";
                    }
                    else if (remainder == 3)
                    {
                        expectedLastCharNRIC = "H";
                    }
                    else if (remainder == 4)
                    {
                        expectedLastCharNRIC = "G";
                    }
                    else if (remainder == 5)
                    {
                        expectedLastCharNRIC = "F";
                    }
                    else if (remainder == 6)
                    {
                        expectedLastCharNRIC = "E";
                    }
                    else if (remainder == 7)
                    {
                        expectedLastCharNRIC = "D";
                    }
                    else if (remainder == 8)
                    {
                        expectedLastCharNRIC = "C";
                    }
                    else if (remainder == 9)
                    {
                        expectedLastCharNRIC = "B";
                    }
                    else if (remainder == 10)
                    {
                        expectedLastCharNRIC = "A";
                    }
                }

                if (firstCharNRIC.Equals("f") || firstCharNRIC.Equals("g"))
                {
                    if (remainder == 0)
                    {
                        expectedLastCharNRIC = "X";
                    }
                    else if (remainder == 1)
                    {
                        expectedLastCharNRIC = "W";
                    }
                    else if (remainder == 2)
                    {
                        expectedLastCharNRIC = "U";
                    }
                    else if (remainder == 3)
                    {
                        expectedLastCharNRIC = "T";
                    }
                    else if (remainder == 4)
                    {
                        expectedLastCharNRIC = "R";
                    }
                    else if (remainder == 5)
                    {
                        expectedLastCharNRIC = "Q";
                    }
                    else if (remainder == 6)
                    {
                        expectedLastCharNRIC = "P";
                    }
                    else if (remainder == 7)
                    {
                        expectedLastCharNRIC = "N";
                    }
                    else if (remainder == 8)
                    {
                        expectedLastCharNRIC = "M";
                    }
                    else if (remainder == 9)
                    {
                        expectedLastCharNRIC = "L";
                    }
                    else if (remainder == 10)
                    {
                        expectedLastCharNRIC = "K";
                    }
                }

                if (expectedLastCharNRIC.ToLower() != lastCharNRIC || expectedLastCharNRIC == "")
                {
                    ViewData["Message"] = "NRTIC entered is not valid, using wrong Start/End character or number keyed in is wrong";
                    ViewData["MsgType"] = "warning";
                    return View("AccountRegistration");
                }
            }
            else
            {
                ViewData["Message"] = "NRIC entered does not follow the NRIC format and is wrong";
                ViewData["MsgType"] = "warning";
                return View("AccountRegistration");
            }

            //Check for age of user based on date entered
            /* **Note that check for valid date entered not required as HTML code provided for user input interface
             does not allow for invalid inputs, through the usage of a calendar, and only not filling up will be an 
            option which was already covered previosuly** */


            //Breaking up DOB string vale retrived into a string list, so as to allow for Date to be re created as a DateTime variable later
            string[] retrivedDOBList = retrivedDOB.Split('-');

            bool isAllowed = false;
            DateTime DateNow = DateTime.Now;
            DateTime DateDOBVariable = new DateTime(retrivedDOB[0], retrivedDOB[1], retrivedDOB[2]);

            if (DateDOBVariable.Year < DateNow.Year == true)
            {
                if ((DateNow - DateDOBVariable).TotalDays / 365 >= 12.0)
                {
                    isAllowed = true;
                }
                else
                {
                    ViewData["Message"] = "Users must be aged 12 or above ";
                    ViewData["MsgType"] = "warning";
                    return View("AccountRegistration");
                }
            }
            else
            {
                ViewData["Message"] = "Invalid DOB Selected, DOB cannot be a day in the future";
                ViewData["MsgType"] = "warning";
                return View("AccountRegistration");
            }







            return View();
        }

        public IActionResult LoginPage()
        {
            return View();
        }

        public IActionResult RegisterPage()
        {
            return View();
        }

        public IActionResult AccountLogin()
        {
            return View();
        }

        public IActionResult Submit()
        {
            return View();
        }

        public IActionResult VistiorRegistration()
        {
            return View();
        }

        public IActionResult Visitor1()
        {
            return View("Visitor1");
        }


        public IActionResult UserAcc()
        {
            return View();
        }

        public IActionResult PatientBooking()
        {
            return View();
        }


        public IActionResult VisitorGroup()
        {
            return View();
        }
    }
}
