using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mof.DataModels.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
namespace mof.DataModels.T4_Templates
{
    public class Class
    {
        public void Test ()
        {
            IEnumerable<string> s;
            //var cn = "Data Source=(LocalDb)\\MSSQLLocalDB;database=IdentityServer4Admin;trusted_connection=yes;";
            //DbContextOptionsBuilder opt = new DbContextOptionsBuilder();
            // opt.UseSqlServer("Data Source=(LocalDb)\\MSSQLLocalDB;database=IdentityServer4Admin;trusted_connection=yes;");
            var db = new MOFContext();
            var g = from gs in db.CeLovgroup select gs;

            //         foreach (CeLovgroup tmp in g)
            //         { foreach (CeLov lov in tmp.CeLov) { tmp.Lovcode}

            //     public static class tmp .Replace(" ","_") 
            //     {
            //         public const string _LOVGroupCode = "<#= tmp.LOVGroupCode #>";  
            //  foreach (KRules.DBModels.CE_LOV lov in tmp.CE_LOVs)
            //             {
            //	  public static string  lov.LOVValue.Replace(" ","_")  { get { return "<#= lov.LOVCode #>" ; } }  
            // } 
            //     }
            //}  
        }
    }
}
