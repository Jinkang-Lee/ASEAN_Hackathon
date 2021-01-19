using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;



namespace C200.Controllers
{

    public class NewFarrerParkController : Controller
    {


        public IActionResult Index()
        {
            return View();
        }



        public IActionResult HomePage()
        {
            return View();
        }

        public IActionResult Visitation1()
        {
            return View("Visitation1");
        }

        //BED AND WARD INFO
        public IActionResult Visitation1Post()
        {

            IFormCollection form = HttpContext.Request.Form;
            
             string patientWard = form["WardNumber"].ToString().Trim();
             string patientBed = form["BedNumber"].ToString().Trim();
            

            //Checks if all fields are filled in
            if (ValidUtl.CheckIfEmpty(patientWard, patientBed) == true)
             {
                 ViewData["Message"] = "All fields must be filled in";
                 ViewData["MsgType"] = "warning";

                 return View("Visitation1");
             }



            //CHECK IF BED AND WARD EXIST
            String fullsql = String.Format("SELECT * FROM Patient WHERE ward_num = '{0}' AND bed_num = '{1}'", patientWard, patientBed);
            DataTable ds = DBUtl.GetTable(fullsql);
             if (ds.Rows.Count == 1)
             {
                return View("Visitation2");
             }
             else
             {
                 ViewData["Message"] = "Invalid Bed or Ward!";
                 ViewData["MsgType"] = "danger";
                 return View("Visitation1");
             }

            
        }

        

        public IActionResult Visitation2()
        {
            return View("Visitation2");
        }

        Random rand1 = new Random();

        //PATIENT AND VISITOR INFO
        public IActionResult Visitation2Post()
        {
            IFormCollection form = HttpContext.Request.Form;
            string patientName = form["PName"].ToString().Trim();
            string VisitorName = form["VName"].ToString().Trim();
            string VisitorNRIC = form["NRIC"].ToString().Trim();
            string VisitorMobile = form["Mobile"].ToString().Trim();

            //Checks if all fields are filled in
            if (ValidUtl.CheckIfEmpty(patientName, VisitorName, VisitorNRIC, VisitorMobile) == true)
            {
                ViewData["Message"] = "All fields must be filled in";
                ViewData["MsgType"] = "warning";

                return View("Visitation2");
            }

            //Checks if name field contain a digit
            if (VisitorName.Any(char.IsDigit) == true && patientName.Any(char.IsDigit) == true)
            {
                ViewData["Message"] = "Name entered must not contain a number";
                ViewData["MsgType"] = "warning";

                return View("Visitation2");
            }

            //CHECK IF PATIENT EXIST
            String fullsql = String.Format("SELECT * FROM Patient WHERE full_name = '{0}'", patientName);
            DataTable ds = DBUtl.GetTable(fullsql);
            if (ds.Rows.Count != 1)
            {
                ViewData["Message"] = "Patient Not Found!";
                ViewData["MsgType"] = "danger";
                return View("Visitation2");
            }


            //Random 6 digit pin number
            Random rand1 = new Random();
            int pin_num = rand1.Next(000001, 999999);

            //INSERT visitor into visitor database
            string sql = @"INSERT INTO Visitor(NRIC, full_name, phone, pin_num) VALUES('{0}', '{1}', '{2}', '{3}')";
            string insert = String.Format(sql, VisitorNRIC, VisitorName, VisitorMobile, pin_num);
            int count = DBUtl.ExecSQL(insert);

            //INSERT visit id and pin number from Visitor table to visitation table
            string sql2 = @"INSERT INTO visitation(visit_id, pin_number) 
                            SELECT visit_id, pin_num FROM Visitor WHERE full_name= '{0}'";
            string insert2 = String.Format(sql2, VisitorName);
            int count2 = DBUtl.ExecSQL(insert2);


            //UPDATE visitation table with patient_id taken from Patient table
            string sql3 = @"UPDATE visitation SET visitation.patient_id = Patient.patient_id FROM Patient WHERE full_name = '{0}'";

            string update = string.Format(sql3, patientName);
            int count3 = DBUtl.ExecSQL(update);


            if (count == 1 && count2 == 1 && count3 == 1)
            {

                return View("Visitation3");
            }


            else
            {
                ViewData["Message"] = DBUtl.DB_Message;
                ViewData["MsgType"] = "danger";
                return View("Visitation2");
            }
        }

            
        
        
        public IActionResult EndingPage()
        {
            return View("EndingPage");
        }


        public IActionResult Visitation3()
        {
            return View("Visitation3");
        }


        //DECLARATION FORM
        public IActionResult Visitation3Post()
        {       

            IFormCollection form = HttpContext.Request.Form;
            string Q1 = form["Q1"].ToString().Trim();
            string Q2 = form["Q2"].ToString().Trim();
            string Q3 = form["Q3"].ToString().Trim();

            //Checks if all fields are filled in
            if (ValidUtl.CheckIfEmpty(Q1, Q2, Q3) == true)
            {
                ViewData["Message"] = "All fields must be filled in";
                ViewData["MsgType"] = "warning";

                return View("Visitation3");
            }

            if(Q1 == "Yes" || Q2 == "Yes" || Q3 == "Yes")
            {
                ViewData["Message"] = "You are no allowed to proceed as you selected 'Yes' for one of the field";
                ViewData["MsgType"] = "warning";

                return View("Visitation3");
            }

            return View("EndingPage");
        }

       

   




        public IActionResult GantryEnter()
        {
            return View("GantryEnter");
        }



        public IActionResult GantryEnterPost()
        {
            IFormCollection form = HttpContext.Request.Form;
            string Pin_Num = form["Pin"].ToString().Trim();

            //Checks if all fields are filled in
            if (ValidUtl.CheckIfEmpty(Pin_Num) == true)
            {
                ViewData["Message"] = "Please enter a pin number";
                ViewData["MsgType"] = "warning";

                return View("GantryEnter");
            }

            if(Pin_Num.Length != 6)
            {
                ViewData["Message"] = "Length of Pin Number should be 6";
                ViewData["MsgType"] = "warning";

                return View("GantryEnter");
            }

            

            //CHECK IF Pin Number is valid
            String checkpinsql = String.Format("SELECT * FROM visitation WHERE pin_number = '{0}'", Pin_Num);
            DataTable ds = DBUtl.GetTable(checkpinsql);
            if (ds.Rows.Count != 1)
            {
                ViewData["Message"] = "Invalid Pin Number!";
                ViewData["MsgType"] = "danger";
                return View("GantryEnter");
            }



            DateTime Time_Now = DateTime.Now;

            
            string sql = @"UPDATE visitation SET visitation.time_in = '{0}' WHERE visitation.pin_number = '{1}'";
            string update = String.Format(sql, Time_Now, Pin_Num);

            int count = DBUtl.ExecSQL(update);

            if (count == 1)
            {
                //Write code to update expected amount of people in ward
                ViewData["Message"] = "SUCCESS!";
                ViewData["MsgType"] = "success";
                return View("GantryEnter");
            }

            return View("GantryEnter");

        }


        public IActionResult GantryExit()
        {
            return View("GantryExit");
        }


        public IActionResult GantryExitPost()
        {
            IFormCollection form = HttpContext.Request.Form;
            string Pin_Num = form["Pin"].ToString().Trim();

            //Checks if all fields are filled in
            if (ValidUtl.CheckIfEmpty(Pin_Num) == true)
            {
                ViewData["Message"] = "Please enter a pin number";
                ViewData["MsgType"] = "warning";

                return View("GantryExit");
            }

            if (Pin_Num.Length != 6)
            {
                ViewData["Message"] = "Length of Pin Number should be 6";
                ViewData["MsgType"] = "warning";

                return View("GantryExit");
            }



            //CHECK IF Pin Number is valid
            String checkpinsql = String.Format("SELECT * FROM visitation WHERE pin_number = '{0}'", Pin_Num);
            DataTable ds = DBUtl.GetTable(checkpinsql);
            if (ds.Rows.Count != 1)
            {
                ViewData["Message"] = "Invalid Pin Number!";
                ViewData["MsgType"] = "danger";
                return View("GantryExit");
            }

            DateTime Time_Now = DateTime.Now;

            //!!!!!!!!!!HAVE TO FIGURE THIS OUT!!!!!!!!!!!!!!!!
            string sql = @"UPDATE visitation SET visitation.time_out = '{0}' WHERE visitation.pin_number = '{1}'";
            string update = String.Format(sql, Time_Now, Pin_Num);

            int count = DBUtl.ExecSQL(update);

            if (count == 1)
            {
                //Write code to update expected amount of people in ward
                ViewData["Message"] = "SUCCESS!";
                ViewData["MsgType"] = "success";
                return View("GantryExit");
            }


            return View("GantryExit");


        }
    }
}
 