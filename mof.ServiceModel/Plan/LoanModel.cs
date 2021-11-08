using mof.ServiceModels.Common.Generic;
using mof.ServiceModels.Project;
using System;
using System.Collections.Generic;
using System.Text;

namespace mof.ServiceModels.Plan
{
    public class PlanProjectSource
    {
        ///// <summary>
        ///// รหัส โครงการของแผน
        ///// </summary>
        public long PlanProjectID { get; set; }
        public List<LoanPeriod> LoanPeriods { get; set; }
        public string CoordinatorName { get; set; }
        public string CoordinatorPosition { get; set; }
        public string CoordinatorTel { get; set; }
        public string CoordinatorEmail { get; set; }
        public List<ProjectResolutionModel> Resolutions { get; set; } = new List<ProjectResolutionModel>();
        public List<ProjectResolutionModel> Files { get; set; } = new List<ProjectResolutionModel>();
    }

    public class LoanPeriod
    {
        /// <summary>
        /// Y = ปี , M = Month
        /// </summary>
        public string PeriodType { get; set; }
        /// <summary>
        /// ปี หรือ เดือน เป็นตัวเลข เช่น เปํนปี 2562 หรือ เดือน 1 (มกราคม)
        /// </summary>
        public int PeriodValue { get; set; }
        /// <summary>
        /// รายละเอียดแหล่งเงินกู้
        /// </summary>
        public List<LoanSource> LoanSources { get; set; } = new List<LoanSource>();
    }
    /// <summary>
    /// แหล่งเงินกู้
    /// </summary>
    public class LoanSource
    {
        /// <summary>
        /// ไม่ได้ใช้
        /// </summary>
        public long? LoanSourceID { get; set; }
        /// <summary>
        /// L = ในประเทศ , F = ต่างประเทศ , O = อื่นๆ, A = ทั้งหมด
        /// </summary>
        public string SourceType { get; set; }
        /// <summary>
        /// จำนวนเงินที่กู้
        /// </summary>
        public decimal LoanAmount { get; set; }
        /// <summary>
        /// สกุลเงิน
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// คิดเป็น (บาท) display only
        /// </summary>
        public decimal THBAmount { get; set; }

        public BasicData SourceLoan { get; set; } = new BasicData();
        public decimal? Jan { get; set; } = 0;
        public decimal? Feb { get; set; } = 0;
        public decimal? Mar { get; set; } = 0;
        public decimal? Apr { get; set; } = 0;
        public decimal? May { get; set; } = 0;
        public decimal? Jun { get; set; } = 0;
        public decimal? Jul { get; set; } = 0;
        public decimal? Aug { get; set; } = 0;
        public decimal? Sep { get; set; } = 0;
        public decimal? Oct { get; set; } = 0;
        public decimal? Nov { get; set; } = 0;
        public decimal? Dec { get; set; } = 0;
    }
    /// <summary>
    /// ลักษณะการกู้
    /// </summary>
    public class LoanType
    {       
        /// <summary>
        /// D = กู้ตรง  G=กู้โดยขอค้ำจากกระทรวงการคลัง  MF=กู้ต่อจากกระทรวงการคลัง(ของรัฐวิสาหกิจ)/กู้มาเพื่อให้กู้ต่อ(ของรัฐบาล)
        /// </summary>
        public string SelectedType { get; set; }
        /// <summary>
        /// จำนวนเงินที่กู้
        /// </summary>
        public decimal LoanAmount { get; set; }
        /// <summary>
        /// สกุลเงิน
        /// </summary>
        public string Currency { get; set; }
    }

}
