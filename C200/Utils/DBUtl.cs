using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Reflection;

// This version is for Lesson 4, 5, 6, 7
public static class DBUtl
{
   public static string DB_CONNECTION;
   public static string DB_Message;

   static DBUtl()
   {
      IConfiguration config =
         new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
      string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
      if (env.Equals("Development"))
         DB_CONNECTION = config.GetConnectionString("DefaultConnection");
      else if (env.Equals("Production"))
         DB_CONNECTION = config.GetConnectionString("ProductionConnection");
   }

   public static DataTable GetTable(string sql)
   {
      DataTable dt = new DataTable();
      using (SqlConnection dbConn = new SqlConnection(DB_CONNECTION))
      using (SqlDataAdapter dAdptr = new SqlDataAdapter(sql, dbConn))
      {
         try
         {
            dAdptr.Fill(dt);
            return dt;
         }

         catch (System.Exception ex)
         {
            DB_Message = ex.Message;
            return null;
         }
      }
   }

   public static int ExecSQL(string sql)
   {
      int rowsAffected = 0;
      using (SqlConnection dbConn = new SqlConnection(DB_CONNECTION))
      using (SqlCommand dbCmd = dbConn.CreateCommand())
      {
         try
         {
            dbConn.Open();
            dbCmd.CommandText = sql;
            rowsAffected = dbCmd.ExecuteNonQuery();
         }

         catch (System.Exception ex)
         {
            DB_Message = ex.Message;
            rowsAffected = -1;
         }
      }
      return rowsAffected;
   }

   public static string EscQuote(this string line)
   {
      return line?.Replace("'", "''");
   }


}
