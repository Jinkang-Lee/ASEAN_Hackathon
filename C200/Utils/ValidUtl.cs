using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Reflection;

public static class ValidUtl
{
   public static bool CheckIfEmpty(params string[] list)
   {
      foreach (string o in list)
      {
         if (o == null || o.Trim().Equals(""))
         {
            return true;
         }
      }
      return false;
   }

   // Extension Method to check whether the contents of a string is an integer
   public static bool IsInteger(this String x)
   {
      return Int32.TryParse(x, out int i);
   }

   // Extension Method to check whether the contents of a string is a double
   public static bool IsNumeric(this String x)
   {
      return Double.TryParse(x, out double i);
   }

   // Extension Method to check whether the contents of a string 
   // is a date specified by the "format" 
   public static bool IsDate(this String x, string format)
   {
      // E.g. format = "yyyy-MM-dd"
      DateTime tempDT;
      return (DateTime.TryParseExact(x, format,
                           CultureInfo.InvariantCulture,
                           DateTimeStyles.None,
                           out tempDT));

   }

   // Extension Method to convert the contents of a string 
   // to a date specified by the "format" 
   public static DateTime ToDate(this String x, string format)
   {
      DateTime tempDT = DateTime.MinValue;
      DateTime.TryParseExact(x, format,
                           CultureInfo.InvariantCulture,
                           DateTimeStyles.None,
                           out tempDT);
      return tempDT;
   }

}
