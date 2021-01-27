using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Data;

namespace C200_Project_WorkSpace.Controllers
{
    public class FarrerParkAdminController : Controller
    {
        public IActionResult Home()
        {
            return View("AdminHomePage");
        }
        
        public IActionResult ModifyVisitationRules()
        {
            return View("AdminModifyVisitation");
        }

        public IActionResult ModifyVisitationRulesPost()
        {
            IFormCollection form = HttpContext.Request.Form;

            string maxNumVisitorEachDay = form["MaxVisitorEachDay"].ToString().Trim();
            string maxNumVisitorEachTime = form["MaxVisitorEachTime"].ToString();
            string maxNumVisitorEachTimeICU = form["MaxVisitorEachTimeICU"].ToString();
            string afternnonStartTime = form["VisitationAfternoonStartTime"];
            string afternnonEndTime = form["VisitationAfternoonEndTime"];
            string eveningStartTime = form["VisitationEveningStartTime"];
            string eveningEndTime = form["VisitationEveningEndTime"];
            string healthDeclarationNeeded = form["HealthDeclarationChecks"].ToString();
            string ageRequiredToVisit = form["AgeRequirement"].ToString();

            ViewData["MsgType"] = "success";
            ViewData["Message"] = afternnonStartTime;

            string insertSQL = @"INSERT INTO Rules 
                               VALUES({0} , {1} , {2} , 
                               '{3:hh:mm}' , '{4:hh:mm}' , '{5:hh:mm}' , '{6:hh:mm}' , 
                               {7} , {8})";
            
            int rowsAffected = DBUtl.ExecSQL(insertSQL, maxNumVisitorEachDay, maxNumVisitorEachTime, maxNumVisitorEachTimeICU, 
                               afternnonStartTime, afternnonEndTime, eveningStartTime, eveningEndTime, 
                               healthDeclarationNeeded, ageRequiredToVisit);

            if(rowsAffected == 1)
            {
                ViewData["MsgType"] = "success";
                ViewData["Message"] = "YAYYYYY";
            }
            else
            {
                ViewData["MsgType"] = "danger";
                ViewData["Message"] = DBUtl.DB_Message;
            }
            
            return View("AdminModifyVisitation");

        }
    }
}
