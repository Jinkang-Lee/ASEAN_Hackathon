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
            String fullsql = String.Format("SELECT * FROM Patient WHERE ward = '{0}' AND bed = '{1}'", patientWard, patientBed);
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
            String fullsql = String.Format("SELECT * FROM Patient WHERE name = '{0}'", patientName);
            DataTable ds = DBUtl.GetTable(fullsql);
            if (ds.Rows.Count != 1)
            {
                ViewData["Message"] = "Patient Not Found!";
                ViewData["MsgType"] = "danger";
                return View("Visitation2");
            }



            //Check for format of NRIC entered
            if ((VisitorNRIC.Substring(0, 1).ToLower().Equals("t")
                || VisitorNRIC.Substring(0, 1).ToLower().Equals("s")
                || VisitorNRIC.Substring(0, 1).ToLower().Equals("g")
                || VisitorNRIC.Substring(0, 1).ToLower().Equals("f"))
                && (VisitorNRIC.Substring(8, 1).IsInteger() == false)
                && ((VisitorNRIC.Substring(1, 7).IsInteger() == true) && (VisitorNRIC.Substring(1, 7).Length == 7))
                && (VisitorNRIC.Length == 9))

            {
                //Check validity of NRIC using CheckSum exact accurate method instead of string patterns
                int sum = 0;
                int remainder = 0;
                string firstCharNRIC = "";
                string lastCharNRIC = "";
                string expectedLastCharNRIC = "";

                firstCharNRIC = VisitorNRIC.Substring(0, 1).ToLower();
                lastCharNRIC = VisitorNRIC.Substring(8, 1).ToLower();

                //Weight of NRIC characters (from first to last) = 2 7 6 5 4 3 2
                sum = (Int32.Parse(VisitorNRIC.Substring(1, 1)) * 2)
                    + (Int32.Parse(VisitorNRIC.Substring(2, 1)) * 7)
                    + (Int32.Parse(VisitorNRIC.Substring(3, 1)) * 6)
                    + (Int32.Parse(VisitorNRIC.Substring(4, 1)) * 5)
                    + (Int32.Parse(VisitorNRIC.Substring(5, 1)) * 4)
                    + (Int32.Parse(VisitorNRIC.Substring(6, 1)) * 3)
                    + (Int32.Parse(VisitorNRIC.Substring(7, 1)) * 2);

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
                    return View("Visitation2");
                }
            }
            else
            {
                ViewData["Message"] = "NRIC entered does not follow the NRIC format and is wrong";
                ViewData["MsgType"] = "warning";
                return View("Visitation2");
            }





            //Random 6 digit pin number
            Random rand1 = new Random();
            int pin_num = rand1.Next(000001, 999999);








            ////CHECK IF VISITOR ALREADY EXIST IN DATABASE
            //String checksql = String.Format("SELECT nric FROM Visitor WHERE nric = '{0}'", VisitorNRIC);
            //DataTable check = DBUtl.GetTable(checksql);
            //if(check.Rows.Count ==1)
            //{
            //    //IF VISITOR EXIST IN DATABASE
            //    //INSERT visitor_id into Visitation table
            //    string sql2 = @"INSERT INTO Visitation(visitor_id) 
            //                    SELECT visitor_id FROM Visitor WHERE name = '{0}'";

            //    string insert1 = string.Format(sql2, VisitorName);
            //    int count2 = DBUtl.ExecSQL(insert1);

            //    //UPDATE visitation table with patient_id taken from Patient table
            //    string sql3 = @"UPDATE Visitation SET Visitation.patient_id = Patient.patient_id FROM Patient WHERE name = '{0}'";

            //    string update = string.Format(sql3, patientName);
            //    int count3 = DBUtl.ExecSQL(update);


            //    //UPDATE Visitation table with pin number
            //    string sql4 = @"UPDATE Visitation SET pin_number = {0}";
            //    string update2 = string.Format(sql4, pin_num);
            //    int count4 = DBUtl.ExecSQL(update2);

            //    if (count2 == 1 && count3 == 1 && count4 == 1)
            //    {

            //        return View("Visitation3");
            //    }


            //    else
            //    {
            //        ViewData["Message"] = DBUtl.DB_Message;
            //        ViewData["MsgType"] = "danger";
            //        return View("Visitation2");
            //    }
            //}



            //else
            //{
            //    //IF VISITOR DOES NOT EXIST IN DATABASE

            //    //INSERT visitor into visitor database
            //    string sql = @"INSERT INTO Visitor(nric, name, phone) VALUES('{0}', '{1}', '{2}')";
            //    string insert = String.Format(sql, VisitorNRIC, VisitorName, VisitorMobile);
            //    int count = DBUtl.ExecSQL(insert);

            //    //INSERT visitor_id into Visitation table
            //    string sql2 = @"INSERT INTO Visitation(visitor_id) 
            //                    SELECT visitor_id FROM Visitor WHERE name = '{0}'";

            //    string insert1 = string.Format(sql2, VisitorName);
            //    int count2 = DBUtl.ExecSQL(insert1);

            //    //UPDATE visitation table with patient_id taken from Patient table
            //    string sql3 = @"UPDATE Visitation SET Visitation.patient_id = Patient.patient_id FROM Patient WHERE name = '{0}'";

            //    string update = string.Format(sql3, patientName);
            //    int count3 = DBUtl.ExecSQL(update);


            //    //UPDATE Visitation table with pin number
            //    string sql4 = @"UPDATE Visitation SET pin_number = {0}";
            //    string update2 = string.Format(sql4, pin_num);
            //    int count4 = DBUtl.ExecSQL(update2);

            //    if (count == 1 && count2 == 1 && count3 == 1 && count4 == 1)
            //    {

            //        return View("Visitation3");
            //    }


            //    else
            //    {
            //        ViewData["Message"] = DBUtl.DB_Message;
            //        ViewData["MsgType"] = "danger";
            //        return View("Visitation2");
            //    }
            //}




            //INSERT visitor into visitor database
            //string sql = @"INSERT INTO Visitor(nric, name, phone) VALUES('{0}', '{1}', '{2}')";
            //string insert = String.Format(sql, VisitorNRIC, VisitorName, VisitorMobile);
            //int count = DBUtl.ExecSQL(insert);




            ////INSERT visitor_id into Visitation table
            //string sql2 = @"INSERT INTO Visitation(visitor_id) 
            //                    SELECT visitor_id FROM Visitor WHERE name = '{0}'";

            //string insert1 = string.Format(sql2, VisitorName);
            //int count2 = DBUtl.ExecSQL(insert1);


            ////UPDATE visitation table with patient_id taken from Patient table
            //string sql3 = @"UPDATE Visitation SET Visitation.patient_id = Patient.patient_id FROM Patient WHERE name = '{0}'";

            //string update = string.Format(sql3, patientName);
            //int count3 = DBUtl.ExecSQL(update);

            //string sqltest1 = @"SELECT Patient.patient_id FROM Patient WHERE name = '{0}'";
            //string test1 = string.Format(sqltest1, patientName);

            //string sqltest2 = @"SELECT Visitor.visitor_id FROM Visitor WHERE name = '{0}'";
            //string test2 = string.Format(sqltest2, VisitorName);

            //UPDATE Visitation table with pin number
            //string sql4 = @"UPDATE Visitation SET pin_number = '{0}' WHERE patient_id = '{1}' AND visitor_id = '{2}'";

            //string update2 = string.Format(sql4, pin_num, test1, test2);
            //int count4 = DBUtl.ExecSQL(update2);




            //if (count == 1 && count2 == 1 && count3 == 1 && count4 == 1)
            //{

            //    return View("Visitation3");
            //}


            //else
            //{
            //    ViewData["Message"] = DBUtl.DB_Message;
            //    ViewData["MsgType"] = "danger";
            //    return View("Visitation2");
            //}




            string selectVisitorIC = @"SELECT nric FROM Visitor WHERE Visitor.nric = '{0}' ";
            string fullSelectVisitorIC = string.Format(selectVisitorIC, VisitorNRIC);
            DataTable DSVisitorNRIC = DBUtl.GetTable(fullSelectVisitorIC);

            if (DSVisitorNRIC.Rows.Count != 1)
            {
                //INSERT visitor into visitor database
                string sql = @"INSERT INTO Visitor(nric, name, phone) VALUES('{0}', '{1}', '{2}')";
                string insert = String.Format(sql, VisitorNRIC, VisitorName, VisitorMobile);
                int count = DBUtl.ExecSQL(insert);
            }

            //Retrive VisitorID 

            string selectVisitorIDSQL = @"SELECT visitor_id FROM Visitor WHERE Visitor.name = '{0}'";
            string fullSelectVisitorIDSQL = string.Format(selectVisitorIDSQL, VisitorName);

            DataTable DSVisitorID = DBUtl.GetTable(fullSelectVisitorIDSQL);
            string retrivedVisitorID = DSVisitorID.Rows[0][0].ToString();

            //Retrive PatientID 
            string selectPatientIDSQL = @"SELECT patient_id FROM Patient WHERE Patient.name = '{0}'";
            string fullSelectPatientIDSQL = string.Format(selectPatientIDSQL, patientName);

            DataTable DSPatientID = DBUtl.GetTable(fullSelectPatientIDSQL);
            string retrivedPatientID = DSPatientID.Rows[0][0].ToString();

            //Retrive RuleID
            string fullSelectRuleIDSQL = @"SELECT rule_id FROM Rules ORDER BY rule_id DESC";

            DataTable DSRuleID = DBUtl.GetTable(fullSelectRuleIDSQL);
            string retrivedRuleID = DSRuleID.Rows[0][0].ToString();


            //Insert Visitation Records into the Visitation Table of the Database
            string insertVisitationSQL = @"INSERT INTO Visitation(patient_id, visitor_id, rule_id, pin_number) VALUES({0} , {1} , {2} , '{3}')";
            string fullInsertVisitationSQL = string.Format(insertVisitationSQL, retrivedPatientID, retrivedVisitorID, retrivedRuleID, pin_num);

            int rowsAffected = DBUtl.ExecSQL(fullInsertVisitationSQL);
            if (rowsAffected == 1)
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
            String checkpinsql = String.Format("SELECT * FROM Visitation WHERE pin_number = '{0}'", Pin_Num);
            DataTable ds = DBUtl.GetTable(checkpinsql);
            if (ds.Rows.Count != 1)
            {
                ViewData["Message"] = "Invalid Pin Number!";
                ViewData["MsgType"] = "danger";
                return View("GantryEnter");
            }



            DateTime Time_Now = DateTime.Now;

            
            string sql = @"UPDATE Visitation SET Visitation.time_in = '{0}' WHERE Visitation.pin_number = '{1}'";
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
            String checkpinsql = String.Format("SELECT * FROM Visitation WHERE pin_number = '{0}'", Pin_Num);
            DataTable ds = DBUtl.GetTable(checkpinsql);
            if (ds.Rows.Count != 1)
            {
                ViewData["Message"] = "Invalid Pin Number!";
                ViewData["MsgType"] = "danger";
                return View("GantryExit");
            }

            DateTime Time_Now = DateTime.Now;


            string sql = @"UPDATE Visitation SET Visitation.time_out = '{0}' WHERE Visitation.pin_number = '{1}'";
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
 