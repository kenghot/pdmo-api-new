using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace mof.DataModels.Models
{
    public partial class MOFContext  : DbContext
    {
        public string GetConnectionString()
        {
 
            var db = base.Database.GetDbConnection();
            return db.ConnectionString;
        }

        [DbFunction("GetMasterAgreement", "dbo")]
        public static string GetMasterAgreement(long ProjectPlanId)
        {
            throw new NotImplementedException();
        }
    }
}
