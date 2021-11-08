
using System;
using System.Collections.Generic;
using System.Linq; 
using System.Text;
using System.Threading.Tasks;

namespace mof.ServiceModels.Constants

{
    public enum eLOVExtend
    {
        PDMOLAWREG
    }
    public static class ConstantValue
    {

		     public static Dictionary<string, string> MIME = new Dictionary<string, string>() {
            {  "csv","text/csv"  },
            { "doc","application/msword"},
            {"docx","application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            {"gif","image/gif" },
            {"htm","text/html" }, 
            {"html","text/html" }, 
            {"jpeg","image/jpeg" },

            {"jpg","image/jpeg" },
            {"png","image/png" },
            {"pdf","application/pdf" },
            {"ppt","application/vnd.ms-powerpoint" },
            {"pptx","application/vnd.ms-powerpoint" },
            {"rar","application/x-rar-compressed" },
            {"txt","text/plain" },
            {"xls","application/vnd.ms-excel" },
            {"xlsx","application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            {"zip","application/zip" },
			 
        };
    }
    public static  class LOVGroup 
    {
	 
        public static class Agreement_Transaction_Type 
        {  
		    
            public const string _LOVGroupCode = "AGTYPE";  
			
        }
		 
        public static class Agreement_Group 
        {  
		    
            public const string _LOVGroupCode = "ARGGroup";  
			 public static string หนี้เงินกู้ที่กระทรวงการคลังกู้มาชำระหนี้เงินกู้ที่กระทรวงการคลังค้ำประกัน__ชำระหนี้แทน_ { get { return "D02" ; } }  
			 public static string หนี้เงินกู้เพื่อชดเชยการขาดดุลงบประมาณ_เมื่อรายจ่ายสูงกว่ารายได้_และการบริหารหนี้__1__ตั๋วเงินคงคลัง__2__พันธบัตร_ตราสารหนี้อื่นๆ { get { return "C01" ; } }  
			 public static string หนี้เงินกู้ที่กระทรวงการคลังกู้มาชำระหนี้เงินกู้ที่กระทรวงการคลังค้ำประกัน__ชำระหนี้แทน_new { get { return "C02" ; } }  
			 public static string หนี้เงินกู้มาเพื่อให้กู้ต่อ { get { return "C03" ; } }  
			 public static string หนี้เงินกู้เพื่อชดใช้ความเสียหายให้_FIDF1 { get { return "C04" ; } }  
			 public static string หนี้เงินกู้เพื่อชดใช้ความเสียหายให้_FIDF_3 { get { return "C05" ; } }  
			 public static string หนี้เงินกู้เพื่อฟื้นฟูและเสริมสร้างความมั่นคงทางเศรษฐกิจ { get { return "C08" ; } }  
			 public static string เงินกู้กรณีรายจ่ายสูงกว่ารายได้ { get { return "A13" ; } }  
			 public static string เงินกู้ตาม_พรก__COVID__เงินบาท_ { get { return "A12" ; } }  
			 public static string เงินกู้ชดเชยการขาดดุลงบประมาณ__เหลื่อมปี_ { get { return "A11" ; } }  
			 public static string การกู้เงินจากแหล่งเงินกู้ในประเทศในรูปสกุลเงินต่างประเทศ { get { return "A10" ; } }  
			 public static string เงินกู้เพื่อบริหารหนี้ตาม_พรก__กองทุนส่งเสริมการประกันภัยพิบัติ_2555 { get { return "A09" ; } }  
			 public static string เงินกู้เพื่อบริหารหนี้ตาม_พรก__ให้อำนาจ_กค__กู้เงินเพื่อการวางระบบบริหารจัดการน้ำและสร้างอนาคต_2555 { get { return "A08" ; } }  
			 public static string เงินกู้ตาม_พรก__ให้อำนาจ_กค__กู้เงินเพื่อฟื้นฟูและเสริมสร้างความมั่นคงทางเศรษฐกิจ_2552 { get { return "A07" ; } }  
			 public static string เงินกู้เพื่อชดใช้ความเสียหายให้_FIDF3 { get { return "A06" ; } }  
			 public static string การให้กู้ต่อ { get { return "A05" ; } }  
			 public static string เงินกู้เพื่อกองทุนบริหารเงินกู้เพื่อปรับโครงสร้างหนี้สาธารณะและพัฒนาตลาดตราสารหนี้ในประเทศ { get { return "A04" ; } }  
			 public static string เงินกู้ในประเทศทดแทนเงินกู้ต่างประเทศ { get { return "A03" ; } }  
			 public static string เงินกู้เพื่อบริหารดุลเงินสด_เงินกู้เพื่อบริหารสภาพคล่องของเงินคงคลัง { get { return "A02" ; } }  
			 public static string เงินกู้เพื่อชดเชยการขาดดุลงบประมาณ { get { return "A01" ; } }  
			 public static string เงินกู้ตาม_พรก__COVID__เงินตราต่างประเทศ_ { get { return "B03" ; } }  
			 public static string รัฐบาลกู้มาให้กู้ต่อ { get { return "B02" ; } }  
			 public static string เงินกู้ตามแผนการก่อหนี้จากต่างประเทศ { get { return "B01" ; } }  
			 public static string เงินยืม__กระทรวงการคลังเป็นผู้ยืมเงิน_ { get { return "G01" ; } }  
			 public static string เงิน_Prefund_ที่ให้กองทุนฯ_ไปบริหารจัดการ { get { return "F02" ; } }  
			 public static string เงินให้ยืม__กระทรวงการคลังให้หน่วยงานอื่นยืมเงิน_ { get { return "F01" ; } }  
			 public static string เงินกู้เพื่อบริหารหนี้ตาม_พรก__COVID__เงินตราต่างประเทศ_ { get { return "D04" ; } }  
			 public static string เงินกู้เพื่อบริหารหนี้เงินกู้ต่อต่างประเทศ { get { return "D03" ; } }  
			 public static string เงินกู้เพื่อบริหารหนี้เงินกู้ต่างประเทศของรัฐบาล { get { return "D01" ; } }  
			 public static string หนี้เงินกู้เพื่อแก้ไขปัญหา_เยียวยา_และฟื้นฟูเศรษฐกิจและสังคมที่ได้รับผลกระทบจากการระบาดของโรคติดเชื้อไวรัสโคโรนา_2019 { get { return "C12" ; } }  
			 public static string เงินกู้เพื่อบริหารหนี้เงินกู้ในประเทศทดแทนเงินกู้ต่างประเทศ { get { return "C11" ; } }  
			 public static string เงินกู้เพื่อบริหารหนี้ตาม_พรก__กองทุนส่งเสริมการประกันภัยพิบัติ_2555_new { get { return "C10" ; } }  
			 public static string เงินกู้เพื่อบริหารหนี้ตาม_พรก__ให้อำนาจ_กค__กู้เงินเพื่อการวางระบบบริหารจัดการน้ำและสร้างอนาคต_2555_new { get { return "C09" ; } }  
			
        }
		 
        public static class อัตราดอกเบี้ย 
        {  
		    
            public const string _LOVGroupCode = "contract_interest_type";  
			 public static string คงที่__Fix_rate_ { get { return "1" ; } }  
			 public static string ลอยตัว__Floating_Rate_ { get { return "2" ; } }  
			
        }
		 
        public static class ประเภท_อปท_ 
        {  
		    
            public const string _LOVGroupCode = "contract_org_type";  
			 public static string เทศบาลนคร { get { return "2" ; } }  
			 public static string เทศบาลเมือง { get { return "3" ; } }  
			 public static string เทศบาลตำบล { get { return "4" ; } }  
			 public static string องค์การบริหารส่วนจังหวัด { get { return "1" ; } }  
			 public static string อปท__รูปแบบพิเศษ { get { return "6" ; } }  
			 public static string องค์การบริหารส่วนตำบล { get { return "5" ; } }  
			
        }
		 
        public static class ประเภทโครงการเงินกู้ 
        {  
		    
            public const string _LOVGroupCode = "contract_project_type";  
			 public static string _1__ครุภัณฑ์_อุปกรณ์สำนักงาน_ยานพาหนะ_เช่น_รถขยะ_รถดับเพลิง_ฯลฯ__ { get { return "1" ; } }  
			 public static string _2__ศูนย์พัฒนาเด็กเล็ก_ศูนย์การเรียนรู้_อาคารสำนักงาน_โรงเรียน { get { return "2" ; } }  
			 public static string _3__ห้องสมุด_สนามกีฬา_สวนสาธารณะ { get { return "3" ; } }  
			 public static string _4__ระบบสาธารณูปโภค_ไฟฟ้า_ประปา_ถนน__ { get { return "4" ; } }  
			 public static string _5__ตลาดสด_โรงฆ่าสัตว์ { get { return "5" ; } }  
			 public static string _6__อื่นๆ { get { return "6" ; } }  
			
        }
		 
        public static class วัตภุประสงค์ของเงินกู้ 
        {  
		    
            public const string _LOVGroupCode = "contract_purpose";  
			 public static string _1__โครงการลงทุน { get { return "1" ; } }  
			 public static string _2__เงินทุนหมุนเวียนสถานธนานุบาล { get { return "2" ; } }  
			 public static string _3__ปรับโครงสร้างหนี้เงินกู้ { get { return "3" ; } }  
			
        }
		 
        public static class แหล่งเงินกู้ 
        {  
		    
            public const string _LOVGroupCode = "contract_source";  
			 public static string เงินทุนส่งเสริมกิจการเทศบาล__ก_ส_ท__ { get { return "1" ; } }  
			 public static string กองทุนพัฒนาเมืองในภูมิภาค__กพม__ { get { return "2" ; } }  
			 public static string ธนาคารเพื่อการเกษตรและสหกรณ์การเกษตร { get { return "3" ; } }  
			 public static string ธนาคารกรุงไทย_จำกัด__มหาชน_ { get { return "4" ; } }  
			
        }
		 
        public static class การนับหนี้ 
        {  
		    
            public const string _LOVGroupCode = "DCAL";  
			 public static string หนี้ทั้งหมดนับเป็นหนี้สาธารณะและหนี้ภาครัฐ { get { return "01" ; } }  
			 public static string หนี้ทั้งหมดไม่นับเป็นหนี้สาธารณะ_แต่เป็นหนี้ภาครัฐ { get { return "02" ; } }  
			 public static string หนี้ที่กระทรวงการคลังค้ำประกันนับเป็นหนี้ภาครัฐและหนี้สาธารณะ_หนี้ที่กระทรวงการคลังไม่ได้ค้ำประกันไม่นับเป็นหนี้สาธารณะ_แต่เป็นหนี้ภาครัฐ { get { return "03" ; } }  
			 public static string หนี้ที่กู้ต่อจากกระทรวงการคลังนับเป็นหนี้ภาครัฐและหนี้สาธารณะ_หนี้ที่กู้จากแหล่งเงินกู้อื่นไม่นับเป็นหนี้สาธารณะ_แต่เป็นหนี้ภาครัฐ { get { return "04" ; } }  
			 public static string __กรณี_อปท__กู้เงินต่อจากกระทรวงการคลัง_นับเป็น_หนี้สาธารณะ__และเป็น__หนี้ภาครัฐ____กรณี_อปท__กู้เงินจากแหล่งเงินกู้อื่น_ไม่นับเป็น__หนี้สาธารณะ__แต่ถือเป็น__หนี้ภาครัฐ_ { get { return "05" ; } }  
			 public static string ปัจจุบัน_อบต__ยังไม่มีอำนาจกู้เงิน_เนื่องจากกระทรวงมหาดไทยยังไม่มีการออกระเบียบเพื่อรองรับการกู้เงินของ_อบต__ทั้งนี้_ตามมาตรา_83_พระราชบัญญัติสภาตำบลและองค์การบริหารส่วนตำบลพ_ศ__2537 { get { return "06" ; } }  
			
        }
		 
        public static class Existing_Debt_Objective 
        {  
		    
            public const string _LOVGroupCode = "EDOBJ";  
			 public static string หนี้เงินกู้มาเพื่อให้กู้ต่อ { get { return "018" ; } }  
			 public static string หนี้เงินกู้บาททดแทนการกู้เงินตราต่างประเทศ__มาตรา_23__ { get { return "017" ; } }  
			 public static string หนี้เงินกู้ที่กระทรวงการคลังกู้มาชำระหนี้เงินกู้ที่กระทรวงการคลังค้ำประกัน__ชำระหนี้แทน_ { get { return "016" ; } }  
			 public static string หนี้เงินกู้ตาม_พรก__กองทุนส่งเสริมการประกันภัยพิบัติ_พ_ศ__2555 { get { return "015" ; } }  
			 public static string หนี้เงินกู้เพื่อบริหารหนี้ตาม_พรก__ให้อำนาจกระทรวงการคลังกู้เงินเพื่อวางระบบบริหารจัดการน้ำและสร้างอนาคตประเทศ_พ_ศ__2555 { get { return "013" ; } }  
			 public static string หนี้เงินกู้เพื่อบริหารหนี้เงินกู้ตามโครงการช่วยเพิ่มเงินกองทุนชั้นที่_2__Tier_2_ { get { return "012" ; } }  
			 public static string หนี้เงินกู้เพื่อบริหารหนี้เงินกู้ตามโครงการช่วยเพิ่มเงินกองทุนชั้นที่_1__Tier_1_ { get { return "011" ; } }  
			 public static string หนี้เงินกู้เพื่อชดใช้ความเสียหายให้_FIDF_3 { get { return "010" ; } }  
			 public static string หนี้เงินกู้เพื่อชดใช้ความเสียหายให้_FIDF_1 { get { return "009" ; } }  
			 public static string หนี้เงินกู้เพื่อฟื้นฟูและเสริมสร้างความมั่นคงทางเศรษฐกิจ { get { return "008" ; } }  
			 public static string หนี้เงินกู้เพื่อแก้ไขปัญหา_เยียวยา_และฟื้นฟูเศรษฐกิจและสังคมที่ได้รับผลกระทบจากการระบาดของโรคติดเชื้อไวรัสโคโรนา_2019 { get { return "006" ; } }  
			 public static string เงินกู้เพื่อบริหารหนี้เงินกู้__นอกกรอบ_ { get { return "005" ; } }  
			 public static string เงินกู้เพื่อบริหารหนี้เงินกู้__ไม่บรรจุแผน_ { get { return "004" ; } }  
			 public static string เงินกู้เพื่อบริหารหนี้เงินกู้__ในกรอบ_ { get { return "003" ; } }  
			 public static string เงินกู้เพื่อบริหารหนี้เงินกู้ { get { return "002" ; } }  
			 public static string หนี้เงินกู้เพื่อชดเชยการขาดดุลงบประมาณ_เมื่อรายจ่ายสูงกว่ารายได้_และการบริหารหนี้_ { get { return "001" ; } }  
			
        }
		 
        public static class Existing_Debt_Plan_Type 
        {  
		    
            public const string _LOVGroupCode = "EDPT";  
			 public static string ปรับโครขากระทรวงการคลังนำเงินเข้า___ปล่อยเงินออก { get { return "G" ; } }  
			 public static string ปรับโครขากระทรวงการคลังนำเงินเข้า___ปล่อยเงินออก_new { get { return "F" ; } }  
			 public static string ปรับโครงสร้างหนี้_ต่างประเทศ { get { return "D" ; } }  
			 public static string ปรับโครงสร้างหนี้_ในประเทศ { get { return "C" ; } }  
			
        }
		 
        public static class พรบ__วินัยการเงินการคลัง 
        {  
		    
            public const string _LOVGroupCode = "FDA";  
			 public static string ส่วนราชการ { get { return "F1" ; } }  
			 public static string รัฐวิสาหกิจประเภท__1_ { get { return "F2" ; } }  
			 public static string รัฐวิสาหกิจประเภท__2_ { get { return "F3" ; } }  
			 public static string รัฐวิสาหกิจประเภท__3_ { get { return "F4" ; } }  
			 public static string หน่วยงานของรฐสภา_ศาล_ฯลฯ { get { return "F5" ; } }  
			 public static string องค์การมหาชน { get { return "F6" ; } }  
			 public static string ทุนหมุนเวียนที่มีฐานะเป็นนิติบุคคล { get { return "F7" ; } }  
			 public static string องค์การปกครองส่วนท้องถิ่น { get { return "F8" ; } }  
			 public static string หน่วยงานอื่นของรัฐตามที่กฎหมายกำหนด { get { return "F9" ; } }  
			
        }
		 
        public static class สาขา 
        {  
		    
            public const string _LOVGroupCode = "FIELD";  
			 public static string สาขาขนส่ง { get { return "01" ; } }  
			 public static string สาขาพลังงาน { get { return "02" ; } }  
			 public static string สาขาสาธารณูปการ { get { return "03" ; } }  
			 public static string สาขาสื่อสาร { get { return "04" ; } }  
			 public static string สาขาอุตสาหกรรทและพาณิชยากรรม { get { return "05" ; } }  
			 public static string สาขาเกษตร { get { return "06" ; } }  
			 public static string สาขาทรัพยากรธรรมชาติ { get { return "07" ; } }  
			 public static string สาขาสังคมและเทคโนโลยี { get { return "08" ; } }  
			 public static string สาขาสถาบันการเงิน { get { return "09" ; } }  
			
        }
		 
        public static class Financial_Report 
        {  
		    
            public const string _LOVGroupCode = "FNREP";  
			 public static string BAL01_สินทรัพย์ { get { return "BAL01" ; } }  
			 public static string BAL0101_สินทรัพย์หมุนเวียน { get { return "BAL0101" ; } }  
			 public static string BAL0102_สินทรัพยถาวร { get { return "BAL0102" ; } }  
			 public static string BAL02_หนี้สินและส่วนของทุน { get { return "BAL02" ; } }  
			 public static string BAL0201_หนี้สิน { get { return "BAL0201" ; } }  
			 public static string BAL020101_หนี้สินหมุนเวียน { get { return "BAL020101" ; } }  
			 public static string BAL020102_หนี้สินระยะยาว { get { return "BAL020102" ; } }  
			 public static string BAL0202_ส่วนของทุน { get { return "BAL0202" ; } }  
			 public static string PLS01_รายได้ { get { return "PLS01" ; } }  
			 public static string PLS02_ค่าใช้จ่าย { get { return "PLS02" ; } }  
			 public static string PLS03_กำไรขาดทุน_จากการดำเนินงาน { get { return "PLS03" ; } }  
			 public static string PLS04_กำไรขาดทุน_ก่อนหักดอกเบี้ยภาษี_ค่าเสือม_และตัดจำหน่าย__EBITDA_ { get { return "PLS04" ; } }  
			 public static string PLS05_กำไรขาดทุน_สุทธิ { get { return "PLS05" ; } }  
			 public static string CF01_กระแสเงินจากการดำเนินงาน { get { return "CF01" ; } }  
			 public static string CF02_กระแสเงินสดจากการลงทุน { get { return "CF02" ; } }  
			 public static string CF03_กระแสเงินสดจากการจัดหาเงิน { get { return "CF03" ; } }  
			 public static string CF0401_เงินสดสุทธฺรับมาก__น้อย__กว่าเงินสดจ่าย { get { return "CF0401" ; } }  
			 public static string CF0402_เงินสดคงเหลือต้นงวด { get { return "CF0402" ; } }  
			 public static string CF0403_เงินสดคงเหลือปลาดงวด { get { return "CF0403" ; } }  
			 public static string DAL01_ยอดหนี้คงค้าง__ประมาณการภาระหนี้ทั้งหมด___ในประเทศ_ { get { return "DAL01" ; } }  
			 public static string DAL02_การเบิกจ่าย__ประมาณการภาระหนี้ทั้งหมด___ในประเทศ_ { get { return "DAL02" ; } }  
			 public static string DAL0301_การชำระคืน_เงินต้น__ประมาณการภาระหนี้ทั้งหมด___ในประเทศ_ { get { return "DAL0301" ; } }  
			 public static string DAL0302_การชำระคืน__ดอกเบี้ยค่าธรรมเนียม__ประมาณการภาระหนี้ทั้งหมด___ในประเทศ_ { get { return "DAL0302" ; } }  
			 public static string DAL04_กระแสเงินไหลเข้าสุทธิ__ประมาณการภาระหนี้ทั้งหมด___ในประเทศ_ { get { return "DAL04" ; } }  
			 public static string DAF01_ยอดหนี้คงค้าง__ประมาณการภาระหนี้ทั้งหมด___ต่างประเทศ_ { get { return "DAF01" ; } }  
			 public static string DAF02_การเบิกจ่าย__ประมาณการภาระหนี้ทั้งหมด___ต่างประเทศ_ { get { return "DAF02" ; } }  
			 public static string DAF0301_การชำระคืน_เงินต้น__ประมาณการภาระหนี้ทั้งหมด___ต่างประเทศ_ { get { return "DAF0301" ; } }  
			 public static string DAF0302_การชำระคืน__ดอกเบี้ยค่าธรรมเนียม__ประมาณการภาระหนี้ทั้งหมด___ต่างประเทศ_ { get { return "DAF0302" ; } }  
			 public static string DAF04_กระแสเงินไหลเข้าสุทธิ__ประมาณการภาระหนี้ทั้งหมด___ต่างประเทศ_ { get { return "DAF04" ; } }  
			 public static string GDL01_เงินต้น__ประมาณการภาระหนี้ที่รัฐบาลต้องรับภาระ___ในประเทศ_ { get { return "GDL01" ; } }  
			 public static string GDL02_ดอกเบี้ยค่าธรรมเนียม__ประมาณการภาระหนี้ที่รัฐบาลต้องรับภาระ___ในประเทศ_ { get { return "GDL02" ; } }  
			 public static string GDL03_ค่าธรรมเนียม__ประมาณการภาระหนี้ที่รัฐบาลต้องรับภาระ___ในประเทศ_ { get { return "GDL03" ; } }  
			 public static string GDF01_ยอดหนี้คงค้าง__ประมาณการภาระหนี้ที่รัฐต้องรับภาระ___ต่างประเทศ_ { get { return "GDF01" ; } }  
			 public static string GDF02_ดอกเบี้ยค่าธรรมเนียม__ประมาณการภาระหนี้ที่รัฐต้องรับภาระ___ต่างประเทศ_ { get { return "GDF02" ; } }  
			 public static string GDF03_ค่าธรรมเนียม__ประมาณการภาระหนี้ที่รัฐต้องรับภาระ___ต่างประเทศ_ { get { return "GDF03" ; } }  
			
        }
		 
        public static class Flow_Type 
        {  
		    
            public const string _LOVGroupCode = "Ftyp";  
			 public static string Ftype1100_Investment___Increase { get { return "1100" ; } }  
			 public static string Ftype1105_Borrowing___Increase { get { return "1105" ; } }  
			 public static string Ftype1110_Decrease { get { return "1110" ; } }  
			 public static string Ftype1120_Final_repayment { get { return "1120" ; } }  
			 public static string Ftype1130_Instalment_repayment { get { return "1130" ; } }  
			 public static string Ftype1150_ดอกเบี้ยทบต้น { get { return "1150" ; } }  
			 public static string Ftype11B0_รับเงินกู้_งบประมาณ_เข้าTR1 { get { return "11B0" ; } }  
			 public static string Ftype11B1_รับเงินกู้_นอกงบในTR1_ตั๋ว { get { return "11B1" ; } }  
			 public static string Ftype11B2_รับเงินกู้_นอกงบในTR1_ชดเชยฯ { get { return "11B2" ; } }  
			 public static string Ftype11B3_รับเงินกู้_new_issue_กม_พิเศษ { get { return "11B3" ; } }  
			 public static string Ftype11B4_รับเงินกู้_refinance_กม_พิเศษ { get { return "11B4" ; } }  
			 public static string Ftype11B5_รับเงินกู้_เข้าบัญชีไถ่ถอน { get { return "11B5" ; } }  
			 public static string Ftype11B6_รับเงินกู้_เข้าบัญชีบริหารหนี้ { get { return "11B6" ; } }  
			 public static string Ftype11B7_รับเงินกู้_เข้าบัญชีอื่นของสบน { get { return "11B7" ; } }  
			 public static string Ftype11B8_รับเงินกู้นอกงปม_เข้าบัญชี_CGD { get { return "11B8" ; } }  
			 public static string Ftype11B9_รับเงินกู้นอกงปม_เข้าบัญชีสรก_ { get { return "11B9" ; } }  
			 public static string Ftype11BS_รับเงินกู้_SOE_องค์กรอื่น { get { return "11BS" ; } }  
			 public static string Ftype11C0_รับเงินกู้นอกงปม_เข้าบัญชีสรก_ { get { return "11C0" ; } }  
			 public static string Ftype11C2_รับเงินกู้_new_issue_SP2 { get { return "11C2" ; } }  
			 public static string Ftype11C3_รับเงินกู้_new_issue_SP2 { get { return "11C3" ; } }  
			 public static string Ftype11C4_รับเงินกู้_new_issue_SP2 { get { return "11C4" ; } }  
			 public static string Ftype11C5_รับเงินกู้_new_issue_SP2 { get { return "11C5" ; } }  
			 public static string Ftype11C6_รับเงินกู้_new_issue_SP2 { get { return "11C6" ; } }  
			 public static string Ftype11C7_รับเงินกู้_new_issue_SP2 { get { return "11C7" ; } }  
			 public static string Ftype11C8_รับเงินกู้_new_issue_SP2 { get { return "11C8" ; } }  
			 public static string Ftype11C9_รับเงินกู้_new_issue_SP2 { get { return "11C9" ; } }  
			 public static string Ftype11D0_ไถ่ถอน_จากงบประมาณ_พร้อมดบ_ { get { return "11D0" ; } }  
			 public static string Ftype11D1_ไถ่ถอน_จากบัญชีไถ่ถอน_พร้อมดบ_ { get { return "11D1" ; } }  
			 public static string Ftype11D2_ไถ่ถอน_จากบัญชีบริหาร_พร้อมดบ_ { get { return "11D2" ; } }  
			 public static string Ftype11D3_ไถ่ถอน_จากบัญชีอื่น_พร้อมดบ_ { get { return "11D3" ; } }  
			 public static string Ftype11D4_ไถ่ถอน_จากบัญชีอื่น_พร้อมดบ_ { get { return "11D4" ; } }  
			 public static string Ftype11D8_ไถ่ถอน_จากบัญชีไถ่ถอน_พร้อมดบ_ { get { return "11D8" ; } }  
			 public static string Ftype11D9_ไถ่ถอน_จากบัญชีไถ่ถอน_พร้อมดบ_ { get { return "11D9" ; } }  
			 public static string Ftype11F0_จ่ายชำระหนี้_Final_Repay_ในงบ { get { return "11F0" ; } }  
			 public static string Ftype11F1_รัฐรับภาระเงินต้นให้SOE { get { return "11F1" ; } }  
			 public static string Ftype11F2_จ่ายคืน_รับคืน_เงินให้กู้ต่อ { get { return "11F2" ; } }  
			 public static string Ftype11F3_กู้_ให้กู้_เงินให้กู้ต่อ { get { return "11F3" ; } }  
			 public static string Ftype11F4_จ่ายชำระหนี้_Prepayment_นอกงบ { get { return "11F4" ; } }  
			 public static string Ftype11F5_จ่ายชำระหนี้_Final_Repay_ในงบ { get { return "11F5" ; } }  
			 public static string Ftype11F7_จ่ายคืน_รับคืน_เงินให้กู้ต่อ { get { return "11F7" ; } }  
			 public static string Ftype11FL_SOE_องค์กรอื่น_จ่ายFinal_Repay { get { return "11FL" ; } }  
			 public static string Ftype11FS_SOE_องค์กรอื่น_จ่ายFinal_Repay { get { return "11FS" ; } }  
			 public static string Ftype11I0_จ่ายชำระหนี้_Installment_ในงบ { get { return "11I0" ; } }  
			 public static string Ftype11I6_Installment_เงินให้กู้ต่อ { get { return "11I6" ; } }  
			 public static string Ftype11IL_Installment_เงินให้กู้ต่อ { get { return "11IL" ; } }  
			 public static string Ftype11IS_SOE_องค์กรอื่น_จ่ายInstallment { get { return "11IS" ; } }  
			 public static string Ftype11L0_เงินให้กู้ต่อ { get { return "11L0" ; } }  
			 public static string Ftype11L1_เงินให้กู้ต่อ_แบบไม่มีตัวเงิน { get { return "11L1" ; } }  
			 public static string Ftype11N0_จ่ายชำระหนี้แบบ_Annuity_ในงบ { get { return "11N0" ; } }  
			 public static string Ftype11N2_รัฐรับภาระจ่าย_Annuity_ให้_SOE { get { return "11N2" ; } }  
			 public static string Ftype11NS_SOE_จ่ายชำระหนี้แบบ_Annuity { get { return "11NS" ; } }  
			 public static string Ftype11P0_จ่ายชำระหนี้_Prepayment_ในงบ { get { return "11P0" ; } }  
			 public static string Ftype11P1_จ่ายชำระหนี้_Prepayment_นอกงบ { get { return "11P1" ; } }  
			 public static string Ftype11P2_จ่ายชำระหนี้_Prepayment_นอกงบ { get { return "11P2" ; } }  
			 public static string Ftype11P3_ไถ่ถอน_จากบัญชีไถ่ถอน_Refi { get { return "11P3" ; } }  
			 public static string Ftype11P4_ไถ่ถอน_นอกงบ_TR1_Refinance { get { return "11P4" ; } }  
			 public static string Ftype11P5_ไถ่ถอน_จากบัญชีงบประมาณ { get { return "11P5" ; } }  
			 public static string Ftype11P6_ไถ่ถอน_จากบัญชีไถ่ถอน_Refi { get { return "11P6" ; } }  
			 public static string Ftype11P7_ไถ่ถอน_นอกงบ_TR1_Refinance { get { return "11P7" ; } }  
			 public static string Ftype11P8_ไถ่ถอน_จากบัญชีงบประมาณ { get { return "11P8" ; } }  
			 public static string Ftype11P9_จ่ายชำระหนี้_Prepayment_ในงบ { get { return "11P9" ; } }  
			 public static string Ftype11PA_จ่ายชำระหนี้_Prepayment_ในงบ { get { return "11PA" ; } }  
			 public static string Ftype11PP_ชำระคืนSOE_องค์กรอื่นPrepaymnt { get { return "11PP" ; } }  
			 public static string Ftype11R0_ไถ่ถอน_จากบัญชีงบประมาณ { get { return "11R0" ; } }  
			 public static string Ftype11R1_ไถ่ถอน_จากบัญชีไถ่ถอน_Refi { get { return "11R1" ; } }  
			 public static string Ftype11R2_ไถ่ถอน_จากบัญชีไถ่ถอน_Rollover { get { return "11R2" ; } }  
			 public static string Ftype11R3_ไถ่ถอน_จากบัญชีบริหาร_Refi { get { return "11R3" ; } }  
			 public static string Ftype11R4_ไถ่ถอน_นอกงบ_TR1_Refinance { get { return "11R4" ; } }  
			 public static string Ftype11R5_ไถ่ถอน_จากบัญชีบริหาร_Rollover { get { return "11R5" ; } }  
			 public static string Ftype11R6_ไถ่ถอน_จากบัญชีบริหาร_Rifinanc { get { return "11R6" ; } }  
			 public static string Ftype11R7_ไถ่ถอน_จากบัญชีอื่น_Refinance { get { return "11R7" ; } }  
			 public static string Ftype11R8_ไถ่ถอน_จากบัญชีอื่น_Rollover { get { return "11R8" ; } }  
			 public static string Ftype11R9_ไถ่ถอน_จากบัญชีไถ่ถอน_กม_พิเศษ { get { return "11R9" ; } }  
			 public static string Ftype11RE_ไถ่ถอน_SOE_องค์กรอื่นRefinance { get { return "11RE" ; } }  
			 public static string Ftype11RO_ไถ่ถอน_SOE_องค์กรอื่น_RollOver { get { return "11RO" ; } }  
			 public static string Ftype11RS_ไถ่ถอน_ของ_SOE_องค์กรอื่น { get { return "11RS" ; } }  
			 public static string Ftype11S3_ไถ่ถอน_จากบัญชีบริหาร_Refi { get { return "11S3" ; } }  
			 public static string Ftype11S4_ไถ่ถอน_นอกงบ_TR1_Refinance { get { return "11S4" ; } }  
			 public static string Ftype11S5_ไถ่ถอน_จากบัญชีบริหาร_Rollover { get { return "11S5" ; } }  
			 public static string Ftype11S6_ไถ่ถอน_จากบัญชีบริหาร_Rifinanc { get { return "11S6" ; } }  
			 public static string Ftype11S7_ไถ่ถอน_นอกงบ_TR1_Refinance { get { return "11S7" ; } }  
			 public static string Ftype11T0_จ่ายคืน_รับคืน_เงินให้กู้ต่อ { get { return "11T0" ; } }  
			 public static string Ftype11T1_กู้ปรับโครงสร้างหนี้_ในปท { get { return "11T1" ; } }  
			 public static string Ftype11T2_จ่ายปรับโครงสร้างหนี้_ในปท_ { get { return "11T2" ; } }  
			 public static string Ftype11T3_กู้ปรับโครงสร้างหนี้_ตลาดทุน { get { return "11T3" ; } }  
			 public static string Ftype11T4_จ่ายปรับโครงสร้างหนี้_ตลาดทุน { get { return "11T4" ; } }  
			 public static string Ftype11T5_กู้_ให้กู้_เงินให้กู้ต่อ { get { return "11T5" ; } }  
			 public static string Ftype11T6_จ่ายคืน_รับคืน_เงินให้กู้ต่อ { get { return "11T6" ; } }  
			 public static string Ftype11T7_ชำระหนี้_ECP_ด้วยการโอนตรง { get { return "11T7" ; } }  
			 public static string Ftype11T8_กู้_ให้กู้_เงินให้กู้ต่อ { get { return "11T8" ; } }  
			 public static string Ftype11T9_จ่ายคืน_รับคืน_เงินให้กู้ต่อ { get { return "11T9" ; } }  
			 public static string Ftype11U0_กู้_ให้กู้_เงินให้กู้ต่อ { get { return "11U0" ; } }  
			 public static string Ftype11U1_จ่ายคืน_รับคืน_เงินให้กู้ต่อ { get { return "11U1" ; } }  
			 public static string Ftype11U2_กู้_ให้กู้_เงินให้กู้ต่อ { get { return "11U2" ; } }  
			 public static string Ftype11U3_จ่ายชำระหนี้_Final_Repay_ในงบ { get { return "11U3" ; } }  
			 public static string Ftype11U4_กู้_ให้กู้_เงินให้กู้ต่อ { get { return "11U4" ; } }  
			 public static string Ftype11U5_กู้_ให้กู้_เงินให้กู้ต่อ { get { return "11U5" ; } }  
			 public static string Ftype11U6_กู้_ให้กู้_เงินให้กู้ต่อ { get { return "11U6" ; } }  
			 public static string Ftype11U7_กู้_ให้กู้_เงินให้กู้ต่อ { get { return "11U7" ; } }  
			 public static string Ftype11U8_กู้_ให้กู้_เงินให้กู้ต่อ { get { return "11U8" ; } }  
			 public static string Ftype11V1_กู้_ให้กู้_เงินให้กู้ต่อ { get { return "11V1" ; } }  
			 public static string Ftype11V2_กู้_ให้กู้_เงินให้กู้ต่อ { get { return "11V2" ; } }  
			 public static string Ftype11V3_กู้_ให้กู้_เงินให้กู้ต่อ { get { return "11V3" ; } }  
			 public static string Ftype11V4_กู้_ให้กู้_เงินให้กู้ต่อ { get { return "11V4" ; } }  
			 public static string Ftype11V5_กู้_ให้กู้_เงินให้กู้ต่อ { get { return "11V5" ; } }  
			 public static string Ftype1204_Facility_charges__Not_utilized { get { return "1204" ; } }  
			 public static string Ftype1205_Facility_charges__Utilized { get { return "1205" ; } }  
			 public static string Ftype1206_Facility_charges__Overdrawn { get { return "1206" ; } }  
			 public static string Ftype1207_Facility_charges__Credit_line { get { return "1207" ; } }  
			 public static string Ftype12A1_ดอกเบี้ยจ่ายล่วงหน้าT_Billในงบ { get { return "12A1" ; } }  
			 public static string Ftype12A2_ดบ_จ่ายล่วงหน้า_T_Bill_นอกงบ { get { return "12A2" ; } }  
			 public static string Ftype12A3_ดอกเบี้ยจ่ายล่วงหน้า_R_Bill { get { return "12A3" ; } }  
			 public static string Ftype12A4_รัฐรับภาระเงินต้นให้SOE { get { return "12A4" ; } }  
			 public static string Ftype12A5_รัฐรับภาระเงินต้นให้SOE { get { return "12A5" ; } }  
			 public static string Ftype12B1_ดอกเบี้ยจ่าย_ในงบประมาณ { get { return "12B1" ; } }  
			 public static string Ftype12B2_ดอกเบี้ยจ่าย_ในงบประมาณ { get { return "12B2" ; } }  
			 public static string Ftype12B3_ดอกเบี้ยจ่าย_ในงบประมาณ { get { return "12B3" ; } }  
			 public static string Ftype12B4_ดอกเบี้ยจ่าย_ในงบประมาณ { get { return "12B4" ; } }  
			 public static string Ftype12B5_ดอกเบี้ยจ่าย_ในงบประมาณ { get { return "12B5" ; } }  
			 public static string Ftype12B6_Commitment_fee { get { return "12B6" ; } }  
			 public static string Ftype12B7_ดอกเบี้ยจ่าย_ในงบประมาณ { get { return "12B7" ; } }  
			 public static string Ftype12B8_Commitment_fee { get { return "12B8" ; } }  
			 public static string Ftype12B9_ดอกเบี้ยจ่าย_ในงบประมาณ { get { return "12B9" ; } }  
			 public static string Ftype12C2_ดอกเบี้ยจ่าย_ในงบประมาณ { get { return "12C2" ; } }  
			 public static string Ftype12I0_ดอกเบี้ยจ่าย_ในงบประมาณ { get { return "12I0" ; } }  
			 public static string Ftype12I1_ดอกเบี้ยจ่าย_กรณี_T_Bill_ในงบ { get { return "12I1" ; } }  
			 public static string Ftype12I2_ดอกเบี้ยจ่าย_กรณี_T_Bill_นอกงบ { get { return "12I2" ; } }  
			 public static string Ftype12I3_ดอกเบี้ยจ่าย_กรณี_R_Bill { get { return "12I3" ; } }  
			 public static string Ftype12I4_ดอกเบี้ยจ่าย_ECP { get { return "12I4" ; } }  
			 public static string Ftype12I5_ดอกเบี้ยจ่าย_รับล่วงหน้า_ในงบ { get { return "12I5" ; } }  
			 public static string Ftype12I6_ดอกเบี้ยจ่าย_รับล่วงหน้า_นอกงบ { get { return "12I6" ; } }  
			 public static string Ftype12I7_ดบ_จ่าย_รับล่วงหน้า_กม_พิเศษ { get { return "12I7" ; } }  
			 public static string Ftype12I8_ดบ_จ่าย_รับล่วงหน้า_ปคส_ใน_ปท_ { get { return "12I8" ; } }  
			 public static string Ftype12I9_ดบจ่ายรับล่วงหน้าnew_issueพรก_ { get { return "12I9" ; } }  
			 public static string Ftype12J1_ดอกเบี้ยจ่าย_รับล่วงหน้า_ในงบ { get { return "12J1" ; } }  
			 public static string Ftype12J2_ดอกเบี้ยจ่าย_รับล่วงหน้า_ในงบ { get { return "12J2" ; } }  
			 public static string Ftype12J3_ดอกเบี้ยจ่าย_รับล่วงหน้า_ในงบ { get { return "12J3" ; } }  
			 public static string Ftype12J4_ดอกเบี้ยจ่าย_รับล่วงหน้า_ในงบ { get { return "12J4" ; } }  
			 public static string Ftype12J5_ดอกเบี้ยจ่าย_รับล่วงหน้า_ในงบ { get { return "12J5" ; } }  
			 public static string Ftype12J6_ดอกเบี้ยจ่าย_รับล่วงหน้า_ในงบ { get { return "12J6" ; } }  
			 public static string Ftype12J7_ดอกเบี้ยจ่าย_รับล่วงหน้า_นอกงบ { get { return "12J7" ; } }  
			 public static string Ftype12L1_ดอกเบี้ยจ่ายไม่มีตัวเงินกู้ต่อ { get { return "12L1" ; } }  
			 public static string Ftype12L2_ดอกเบี้ยจ่าย_ECPเพื่อให้กู้ต่อ { get { return "12L2" ; } }  
			 public static string Ftype12L3_ดอกเบี้ยรับกู้ต่อ_ไม่มีตัวเงิน { get { return "12L3" ; } }  
			 public static string Ftype12L4_ดอกเบี้ยรับจาก_ECPไม่มีตัวเงิน { get { return "12L4" ; } }  
			 public static string Ftype12L5_ดอกเบี้ยรับจาก_ECPไม่มีตัวเงิน { get { return "12L5" ; } }  
			 public static string Ftype12L6_ดอกเบี้ยรับจาก_ECPไม่มีตัวเงิน { get { return "12L6" ; } }  
			 public static string Ftype12L8_ดอกเบี้ยรับจาก_ECPไม่มีตัวเงิน { get { return "12L8" ; } }  
			 public static string Ftype12L9_ดอกเบี้ยจ่ายไม่มีตัวเงินกู้ต่อ { get { return "12L9" ; } }  
			 public static string Ftype12M1_ดอกเบี้ยรับจาก_ECPไม่มีตัวเงิน { get { return "12M1" ; } }  
			 public static string Ftype12M2_ดอกเบี้ยรับจาก_ECPไม่มีตัวเงิน { get { return "12M2" ; } }  
			 public static string Ftype12M3_Commitment_fee { get { return "12M3" ; } }  
			 public static string Ftype12M4_Commitment_fee { get { return "12M4" ; } }  
			 public static string Ftype12M5_ดอกเบี้ยรับจาก_ECPไม่มีตัวเงิน { get { return "12M5" ; } }  
			 public static string Ftype12M6_Commitment_fee { get { return "12M6" ; } }  
			 public static string Ftype12M7_Commitment_fee { get { return "12M7" ; } }  
			 public static string Ftype12O1_ดอกเบี้ยจ่ายของกองทุนอื่น { get { return "12O1" ; } }  
			 public static string Ftype12R1_ดอกเบี้ยจ่ายไม่มีตัวเงินกู้ต่อ { get { return "12R1" ; } }  
			 public static string Ftype12R2_ดอกเบี้ยรับไม่มีตัวเงินกู้ต่อ { get { return "12R2" ; } }  
			 public static string Ftype12R3_ดอกเบี้ยรับมีตัวเงิน_กู้ต่อ { get { return "12R3" ; } }  
			 public static string Ftype12S0_ดอกเบี้ยจ่าย_SOE_องค์กรอื่น { get { return "12S0" ; } }  
			 public static string Ftype12S1_รัฐรับภาระดอกเบี้ยจ่าย_ของSOE { get { return "12S1" ; } }  
			 public static string Ftype12S2_SOE_ดอกเบี้ยจ่ายของตั๋ว_CP { get { return "12S2" ; } }  
			 public static string Ftype12S3_รัฐรับภาระดอกเบี้ยจ่าย_ของSOE { get { return "12S3" ; } }  
			 public static string Ftype13I0_ดอกเบี้ยจ่าย_รับล่วงหน้า_นอกงบ { get { return "13I0" ; } }  
			 public static string Ftype13I1_ดอกเบี้ยจ่าย_รับล่วงหน้า_นอกงบ { get { return "13I1" ; } }  
			 public static string Ftype13I2_ดบ_จ่าย_รับล่วงหน้า_กม_พิเศษ { get { return "13I2" ; } }  
			 public static string Ftype13I3_ดอกเบี้ยจ่าย_รับล่วงหน้า_นอกงบ { get { return "13I3" ; } }  
			 public static string Ftype13I4_ดอกเบี้ยจ่าย_รับล่วงหน้า_นอกงบ { get { return "13I4" ; } }  
			 public static string Ftype13I5_ดอกเบี้ยจ่าย_รับล่วงหน้า_นอกงบ { get { return "13I5" ; } }  
			 public static string Ftype13I6_ดอกเบี้ยจ่าย_รับล่วงหน้า_นอกงบ { get { return "13I6" ; } }  
			 public static string Ftype13I7_ดบ_จ่าย_รับล่วงหน้า_กม_พิเศษ { get { return "13I7" ; } }  
			 public static string Ftype13I8_ดบ_จ่าย_รับล่วงหน้า_กม_พิเศษ { get { return "13I8" ; } }  
			 public static string Ftype1401_Discount_รอตัดจ่าย_พรบ_บริหาร { get { return "1401" ; } }  
			 public static string Ftype1402_Discount_รอตัดจ่าย_พรบ_บริหาร { get { return "1402" ; } }  
			 public static string Ftype1403_Discount_รอตัดจ่าย_พรบ_บริหาร { get { return "1403" ; } }  
			 public static string Ftype1901_Charges { get { return "1901" ; } }  
			 public static string Ftype1902_Commission { get { return "1902" ; } }  
			 public static string Ftype1905_Withholding_tax_1__e_g__state_ { get { return "1905" ; } }  
			 public static string Ftype1906_Withholding_tax_2__e_g_region_ { get { return "1906" ; } }  
			 public static string Ftype2100_Interest_accrual { get { return "2100" ; } }  
			 public static string Ftype2102_Interest_accrual { get { return "2102" ; } }  
			 public static string Ftype2103_Interest_accrual_deferral__CP { get { return "2103" ; } }  
			 public static string Ftype2104_Interest_accrual_deferral__CP { get { return "2104" ; } }  
			 public static string Ftype2105_Int__capitalization__Accrual { get { return "2105" ; } }  
			 public static string Ftype2106_Other_Exp__Accrual { get { return "2106" ; } }  
			 public static string Ftype2107_Other_Exp__Accrual { get { return "2107" ; } }  
			 public static string Ftype2108_Commission { get { return "2108" ; } }  
			 public static string Ftype2109_Commission { get { return "2109" ; } }  
			 public static string Ftype2110_Commission { get { return "2110" ; } }  
			 public static string Ftype2111_Commission { get { return "2111" ; } }  
			 public static string Ftype2150_Interest_accrual__Reset { get { return "2150" ; } }  
			 public static string Ftype2152_Interest_accrual__Reset { get { return "2152" ; } }  
			 public static string Ftype2153_Int_accrual_deferral__CP_reset { get { return "2153" ; } }  
			 public static string Ftype2154_Int_accrual_deferral__CP_reset { get { return "2154" ; } }  
			 public static string Ftype2155_Int_capitalization__Accr_reset { get { return "2155" ; } }  
			 public static string Ftype2156_Other_Exp___Accr_reset { get { return "2156" ; } }  
			 public static string Ftype2157_Other_Exp___Accr_reset { get { return "2157" ; } }  
			 public static string Ftype2200_Interest_deferral { get { return "2200" ; } }  
			 public static string Ftype2203_Interest_deferral { get { return "2203" ; } }  
			 public static string Ftype2205_Int__capitalization__Deferral { get { return "2205" ; } }  
			 public static string Ftype2210_Interest_deferral { get { return "2210" ; } }  
			 public static string Ftype2211_Interest_deferral { get { return "2211" ; } }  
			 public static string Ftype2250_Interest_deferral__Reset { get { return "2250" ; } }  
			 public static string Ftype2251_Interest_deferral__Reset { get { return "2251" ; } }  
			 public static string Ftype2252_Interest_deferral__Reset { get { return "2252" ; } }  
			 public static string Ftype2253_Interest_deferral__Reset { get { return "2253" ; } }  
			 public static string Ftype2255_Int_capitalization__Def_reset { get { return "2255" ; } }  
			 public static string Ftype2300_Interest_acc_def___Diff_proc_ { get { return "2300" ; } }  
			 public static string Ftype2305_Int_cap__acc__def___Diff_proc_ { get { return "2305" ; } }  
			 public static string Ftype3090_Euro_transaction_loss { get { return "3090" ; } }  
			 public static string Ftype3091_Euro_transaction_gain { get { return "3091" ; } }  
			 public static string Ftype3100_Realized_Forex_Loss { get { return "3100" ; } }  
			 public static string Ftype3110_Realized_Forex_Gain { get { return "3110" ; } }  
			 public static string Ftype3120_Realized_Security_Loss { get { return "3120" ; } }  
			 public static string Ftype3130_Realized_Security_Gain { get { return "3130" ; } }  
			 public static string Ftype3400_Forex_Write_Up { get { return "3400" ; } }  
			 public static string Ftype3410_Forex_Write_Down { get { return "3410" ; } }  
			 public static string Ftype3420_Security_Write_Up { get { return "3420" ; } }  
			 public static string Ftype3430_Security_Write_Down { get { return "3430" ; } }  
			 public static string Ftype4040_Take_euro_transact__off_books { get { return "4040" ; } }  
			 public static string Ftype4041_Post_euro_transaction { get { return "4041" ; } }  
			 public static string Ftype4100_รายได้ค่าธรรมเนียมค้ำประกัน { get { return "4100" ; } }  
			 public static string Ftype4101_ร_ด_Fee_ค้ำประกัน { get { return "4101" ; } }  
			 public static string Ftype4102_ร_ด_ค่าปรับ_Fee_ค้ำประกัน { get { return "4102" ; } }  
			 public static string Ftype4103_ร_ด_Fee_กู้ต่อ { get { return "4103" ; } }  
			 public static string Ftype4104_ร_ด_ค่าปรับ_Fee_กู้ต่อ { get { return "4104" ; } }  
			 public static string Ftype4105_ร_ด_ค่าปรับผิดนัดต้นเงิน { get { return "4105" ; } }  
			 public static string Ftype4106_ร_ด_ค่าปรับผิดนัด_ด_บ { get { return "4106" ; } }  
			 public static string Ftype4107_ร_ด_ค่าปรับผิดนัด_Fee { get { return "4107" ; } }  
			 public static string Ftype4108_ร_ด_เงินให้กู้ต่ออื่นๆ { get { return "4108" ; } }  
			 public static string Ftype4109_ร_ด_Fee_ชำระหนี้แทน { get { return "4109" ; } }  
			 public static string Ftype4L01_ร_ด_Fee_เงินกู้ { get { return "4L01" ; } }  
			 public static string Ftype4L02_ร_ด_Fee_เงินกู้ { get { return "4L02" ; } }  
			 public static string Ftype4L03_ร_ด_Fee_เงินกู้ { get { return "4L03" ; } }  
			 public static string Ftype5000_Taxes_1 { get { return "5000" ; } }  
			 public static string Ftype5001_Taxes_2 { get { return "5001" ; } }  
			 public static string Ftype5002_คชจ_การพิมพ์_โฆษณาขายพันธบัตร { get { return "5002" ; } }  
			 public static string Ftype5003_ค่าธรรมเนียมจัดจำหน่าย__UW_ { get { return "5003" ; } }  
			 public static string Ftype5004_ค่านายทะเบียน { get { return "5004" ; } }  
			 public static string Ftype5005_ค่าธรรมเนียมการจ่ายดอกเบี้ย { get { return "5005" ; } }  
			 public static string Ftype5006_ค่าธรรมเนียมการไถ่ถอนพันธบัตร { get { return "5006" ; } }  
			 public static string Ftype5007_ค่าธรรมเนียมในการจัดการ { get { return "5007" ; } }  
			 public static string Ftype5008_ค่าธรรมเนียมในการรับซื้อคืน { get { return "5008" ; } }  
			 public static string Ftype5009_ค่าให้บริการแก่ผู้ถือกรรมสิทธิ { get { return "5009" ; } }  
			 public static string Ftype5010_ค่าใช้จ่ายในการประชาสัมพันธ์ { get { return "5010" ; } }  
			 public static string Ftype5011_คชจ_จัดส่งพันธบัตรทางไปรษณีย์ { get { return "5011" ; } }  
			 public static string Ftype5012_ค่าธรรมเนียมการค้ำประกัน_อาวัล { get { return "5012" ; } }  
			 public static string Ftype5013_Commitment_fee { get { return "5013" ; } }  
			 public static string Ftype5014_Cancellation_fee { get { return "5014" ; } }  
			 public static string Ftype5015_Extension_fee { get { return "5015" ; } }  
			 public static string Ftype5016_Management_fee__Front_end_fee { get { return "5016" ; } }  
			 public static string Ftype5017_Prepayment_fee { get { return "5017" ; } }  
			 public static string Ftype5018_Underwriting_fee { get { return "5018" ; } }  
			 public static string Ftype5019_Paying_Agent_Fee { get { return "5019" ; } }  
			 public static string Ftype5020_ค่าที่ปรึกษากฏหมาย { get { return "5020" ; } }  
			 public static string Ftype5021_ค่าที่ปรึกษากฏหมาย { get { return "5021" ; } }  
			 public static string Ftype5099_ค่าใช้จ่าย__ค่าธรรมเนียมอื่นๆ { get { return "5099" ; } }  
			 public static string Ftype5A01_ค่าใช้จ่าย__ค่าธรรมเนียมอื่นๆ { get { return "5A01" ; } }  
			 public static string Ftype5L01_คชจ__Fee_เงินกู้ { get { return "5L01" ; } }  
			 public static string Ftype5L02_คชจ__Fee_เงินกู้ { get { return "5L02" ; } }  
			 public static string Ftype5L03_คชจ__Fee_เงินกู้ { get { return "5L03" ; } }  
			 public static string Ftype7D00_Discount_รอตัดจ่าย_ในงบในTR1 { get { return "7D00" ; } }  
			 public static string Ftype7D01_Discount_รอตัดจ่าย_ของกม_พิเศษ { get { return "7D01" ; } }  
			 public static string Ftype7D02_Discount_รอตัดจ่าย_ของ_สงน { get { return "7D02" ; } }  
			 public static string Ftype7D03_Discount_รอตัดจ่าย_ของ_สงต_ { get { return "7D03" ; } }  
			 public static string Ftype7D04_Discount_new_issue_พรก_ { get { return "7D04" ; } }  
			 public static string Ftype7D05_Discount_รอตัดจ่าย_พรบ_บริหาร { get { return "7D05" ; } }  
			 public static string Ftype7D06_Discount_new_issue_พรก_ { get { return "7D06" ; } }  
			 public static string Ftype7D07_Discount_รอตัดจ่าย_พรบ_บริหาร { get { return "7D07" ; } }  
			 public static string Ftype7D08_Discount_new_issue_พรก_ { get { return "7D08" ; } }  
			 public static string Ftype7D09_Discount_รอตัดจ่าย_พรบ_บริหาร { get { return "7D09" ; } }  
			 public static string Ftype7D11_ตัดจำหน่าย_Discount { get { return "7D11" ; } }  
			 public static string Ftype7D12_Discount_new_issue_พรก_ { get { return "7D12" ; } }  
			 public static string Ftype7D13_Discount_รอตัดจ่าย_ของ_สงน { get { return "7D13" ; } }  
			 public static string Ftype7P00_Premium__รอตัดจ่าย_ในงบใน_TR1 { get { return "7P00" ; } }  
			 public static string Ftype7P01_Premium__รอตัดจ่าย_ของกม_พิเศษ { get { return "7P01" ; } }  
			 public static string Ftype7P02_Premium__รอตัดจ่าย_ของ_สงน { get { return "7P02" ; } }  
			 public static string Ftype7P03_Premium__รอตัดจ่าย_ของ_สงต_ { get { return "7P03" ; } }  
			 public static string Ftype7P04_Premium__new_issue_พรก_ { get { return "7P04" ; } }  
			 public static string Ftype7P05_Premiumรอตัดจ่าย_พรบ_บริหารหนี { get { return "7P05" ; } }  
			 public static string Ftype7P06_Premium__new_issue_พรก_ { get { return "7P06" ; } }  
			 public static string Ftype7P07_Premiumรอตัดจ่าย_พรบ_บริหารหนี { get { return "7P07" ; } }  
			 public static string Ftype7P08_Premium__new_issue_พรก_ { get { return "7P08" ; } }  
			 public static string Ftype7P09_Premium__new_issue_พรก_ { get { return "7P09" ; } }  
			 public static string Ftype7P11_ตัดจำหน่าย_Premium { get { return "7P11" ; } }  
			 public static string Ftype7P12_Premium__new_issue_พรก_ { get { return "7P12" ; } }  
			 public static string Ftype7P13_Premium__รอตัดจ่าย_ของ_สงน { get { return "7P13" ; } }  
			 public static string Ftype7T01_กำไรจากการไถ่ถอนก่อนกำหนด { get { return "7T01" ; } }  
			 public static string Ftype7T02_ขาดทุนจากการไถ่ถอนก่อนกำหนด { get { return "7T02" ; } }  
			 public static string Ftype7T03_กำไรจากการไถ่ถอนก่อนกำหนด { get { return "7T03" ; } }  
			 public static string FtypeCD11_Conv_T_Bill { get { return "CD11" ; } }  
			 public static string FtypeCD12_Conv_P_N_ST { get { return "CD12" ; } }  
			 public static string FtypeCD13_Conv_Current_portion { get { return "CD13" ; } }  
			 public static string FtypeCD20_Conv_Bond { get { return "CD20" ; } }  
			 public static string FtypeCD30_Conv_P_N_LT { get { return "CD30" ; } }  
			 public static string FtypeCI10_Conv_ECP { get { return "CI10" ; } }  
			 public static string FtypeCI20_Conv_Loan__LT { get { return "CI20" ; } }  
			 public static string FtypeCI30_Conv_Inter_Bond { get { return "CI30" ; } }  
			 public static string FtypeCONI_Discount_รอตัดจ่าย_พรบ_บริหาร { get { return "CONI" ; } }  
			 public static string FtypeCONL_Conv_Onlent { get { return "CONL" ; } }  
			 public static string FtypeCONV_Conversion_not_posting_FI { get { return "CONV" ; } }  
			 public static string FtypeCR20_Conv_Loan__LT___Prin_Decrease { get { return "CR20" ; } }  
			 public static string FtypeCR30_Conv_Intl_Bond___Prin_Decrease { get { return "CR30" ; } }  
			 public static string FtypeCRD1_Conv_CurPortion__Prin_Decrease { get { return "CRD1" ; } }  
			 public static string FtypeCRD2_Conv_PN_LT___Prin_Decrease { get { return "CRD2" ; } }  
			 public static string FtypeTONL_ร_ด_Fee_ชำระหนี้แทน { get { return "TONL" ; } }  
			
        }
		 
        public static class Transaction_CashFlow_Type_from_GF 
        {  
		    
            public const string _LOVGroupCode = "GroupFTyp";  
			 public static string เบิกเงินกู้ { get { return "1" ; } }  
			 public static string ชำระเงินต้น { get { return "2" ; } }  
			 public static string ชำระดอกเบี้ย { get { return "3" ; } }  
			 public static string ค่าใช้จ่าย { get { return "4" ; } }  
			 public static string รายได้ { get { return "5" ; } }  
			 public static string รายได้ดอกเบี้ย { get { return "6" ; } }  
			 public static string Installment { get { return "7" ; } }  
			
        }
		 
        public static class Data_log 
        {  
		    
            public const string _LOVGroupCode = "LOG";  
			
        }
		 
        public static class Loan_Type 
        {  
		    
            public const string _LOVGroupCode = "LTYPE";  
			 public static string กู้ตรง__ค้ำ_ไม่ค้ำ_ { get { return "01" ; } }  
			 public static string กู้มาให้กู้ต่อ { get { return "02" ; } }  
			 public static string ปรับสภาพคล่อง
 { get { return "03" ; } }  
			 public static string กู้ต่อจากกระทรวงการคลัง { get { return "04" ; } }  
			
        }
		 
        public static class กลุ่มหน่วยงาน 
        {  
		    
            public const string _LOVGroupCode = "ORGGRP";  
			 public static string หน่วยงานรัฐบาล { get { return "GOV" ; } }  
			 public static string หน่วยงานเอกชน { get { return "PVC" ; } }  
			 public static string หน่วยงานรัฐอื่นๆ { get { return "GOVOTH" ; } }  
			
        }
		 
        public static class สถานะองค์กร 
        {  
		    
            public const string _LOVGroupCode = "ORGSTATUS";  
			 public static string Template_A { get { return "A" ; } }  
			 public static string Template_B { get { return "B" ; } }  
			 public static string Template_C { get { return "C" ; } }  
			 public static string Template_D { get { return "D" ; } }  
			 public static string Template_E { get { return "E" ; } }  
			 public static string Template_F { get { return "F" ; } }  
			 public static string Template_G { get { return "G" ; } }  
			 public static string Template_I { get { return "I" ; } }  
			 public static string Template_J { get { return "J" ; } }  
			 public static string Template_K { get { return "K" ; } }  
			 public static string Template_L { get { return "L" ; } }  
			
        }
		 
        public static class ประเภทหน่วยงาน 
        {  
		    
            public const string _LOVGroupCode = "ORGTYPE";  
			 public static string ส่วนราชการ__กระทรวง_ทบวง_กรม__ { get { return "GOV" ; } }  
			 public static string รัฐวิสาหกิจ { get { return "SEN" ; } }  
			 public static string องค์กรปกครองส่วนท้องถิ่น { get { return "LGO" ; } }  
			 public static string หน่วยงานเอกชน { get { return "PVC" ; } }  
			 public static string กองทุนนิติบุคคล { get { return "FUND" ; } }  
			 public static string มหาวิทยาลัยในการกำกับดูแลของรัฐ { get { return "UN" ; } }  
			 public static string องค์การมหาชน { get { return "LIM" ; } }  
			 public static string หน่วยงานอื่น_ๆ_ของรัฐ { get { return "OTH" ; } }  
			 public static string หน่วยงานอิสระ { get { return "IDP" ; } }  
			 public static string รัฐวิสาหกิจที่ทำธุรกิจให้กู้ยืมเงิน { get { return "FIN" ; } }  
			 public static string ธนาคารแห่งประเทศไทย { get { return "BOT" ; } }  
			
        }
		 
        public static class Payment_Source 
        {  
		    
            public const string _LOVGroupCode = "PAYSRC";  
			 public static string เงินรายได้ { get { return "04" ; } }  
			 public static string เงินงบประมาณ { get { return "03" ; } }  
			 public static string เงินจากแหล่งอื่นๆ { get { return "07" ; } }  
			 public static string เงินกู้ { get { return "L" ; } }  
			
        }
		 
        public static class DebtPaymentPlanType 
        {  
		    
            public const string _LOVGroupCode = "PAYTYPE";  
			 public static string ชำระตามกำหนด { get { return "SCH" ; } }  
			 public static string ชำระหนี้ล่วงหน้า { get { return "ADV" ; } }  
			 public static string Roll_over { get { return "ROLL" ; } }  
			 public static string Refinance { get { return "REF" ; } }  
			 public static string Cross_Currency_Swap { get { return "CCS" ; } }  
			 public static string Interrest_Rate_ { get { return "IRS" ; } }  
			 public static string ไม่ระบุ { get { return "NA" ; } }  
			 public static string Roll_over_Refinance { get { return "RORF" ; } }  
			 public static string Cross_Currency_Swap_Interrest_Rate { get { return "COIR" ; } }  
			
        }
		 
        public static class พรบ__หนี้สาธารณะ 
        {  
		    
            public const string _LOVGroupCode = "PDA";  
			 public static string หน่วยงานของรัฐ { get { return "P1" ; } }  
			 public static string หน่วยงานในการกำกับดูแลของรัฐ { get { return "P2" ; } }  
			 public static string องค์กรปกครองส่วนท้องถิ่น { get { return "P3" ; } }  
			 public static string รัฐวิสาหกิจประเภท__ก_ { get { return "P4" ; } }  
			 public static string รัฐวิสาหกิจประเภท__ข_ { get { return "P5" ; } }  
			 public static string รัฐวิสาหกิจประเภท__ค_ { get { return "P6" ; } }  
			 public static string สถาบันการเงินภาครัฐ { get { return "P7" ; } }  
			
        }
		 
        public static class Project_Amount_Type 
        {  
		    
            public const string _LOVGroupCode = "PJAMT";  
			 public static string ค่าใช้จ่ายตามมติ { get { return "01" ; } }  
			 public static string ค่าใช้จ่ายตามสัญญาจ้าง { get { return "02" ; } }  
			 public static string เงินงบประมาณ { get { return "03" ; } }  
			 public static string เงินรายได้ { get { return "04" ; } }  
			 public static string เงินกู้ลงนาม { get { return "05" ; } }  
			 public static string เงินกู้เบิกจ่าย { get { return "06" ; } }  
			 public static string เงินจากแหล่งอื่นๆ { get { return "07" ; } }  
			 public static string กู้ตรง__รัฐบาล_ { get { return "D" ; } }  
			 public static string กู้โดยขอค้ำจากกระทรวงการคลัง { get { return "G" ; } }  
			 public static string กู้ต่อจากกระทรวงการคลัง_ของรัฐวิสาหกิจ_ { get { return "MF" ; } }  
			 public static string แผนการใช้จ่ายเงิน { get { return "PD" ; } }  
			 public static string กู้มาเพื่อให้กู้ต่อ_ของรัฐบาล_ { get { return "FL" ; } }  
			 public static string กู้โดยไม่ขอค้ำจากกระทรวงการคลัง { get { return "NG" ; } }  
			
        }
		 
        public static class สถานะโครงการ 
        {  
		    
            public const string _LOVGroupCode = "PJSTATUS";  
			 public static string ความสอดคล้องกับนโยบายยุทธศาสตร์ชาติ { get { return "01" ; } }  
			 public static string ความสอดคล้องกับแผนการปฏิรูปประเทศ { get { return "02" ; } }  
			 public static string ความสอดคล้องกับแผนพัฒนาเศรษฐกิจและสังคมแห่งชาติ { get { return "03" ; } }  
			 public static string มีรายงานการศึกษาความเหมาะสมด้านเทคนิค_เศรษฐกิจ_สังคม_การเงิน { get { return "04" ; } }  
			 public static string มีรายงานการวิเคราะห์ผลกระทบสิ่งแวดล้อม { get { return "05" ; } }  
			 public static string ได้รับการอนุมัติจากคณะกรรมการของหน่วยงาน { get { return "06" ; } }  
			 public static string ได้รับการอนุมัติจากกระทรวงเจ้าสังกัด { get { return "07" ; } }  
			 public static string ได้รับการอนุมัติจากสภาพัฒนาการเศรษฐกิจและสังคมแห่งชาติ { get { return "08" ; } }  
			 public static string ได้รับการอนุมัติจากคณะรัฐมนตรี { get { return "09" ; } }  
			
        }
		 
        public static class Project_Type 
        {  
		    
            public const string _LOVGroupCode = "PJTYPE";  
			 public static string กู้เพื่อลงทุนในโครงการพัฒนา { get { return "DEV" ; } }  
			 public static string กู้เพื่อโครงการ { get { return "GEN" ; } }  
			 public static string กู้เพื่อดำเนินโครงการหรือเพื่อใช้เป็นเงินทุนหมุนเวียนในการดำเนินกิจการทั่วไป { get { return "OPR" ; } }  
			 public static string กู้เพื่อชดเชยการขาดดุลสำหรับปีงบประมาณ { get { return "COMP" ; } }  
			 public static string กู้เพื่อชดเชยการขาดดุลสำหรับการเบิกจ่ายกันเหลื่อมปี { get { return "COMPY" ; } }  
			 public static string กู้เพื่อโครงการอื่นๆ { get { return "OTHER" ; } }  
			 public static string กู้เพื่อบริหารสภาพคล่องของเงินคงคลัง { get { return "LIQ" ; } }  
			 public static string กู้เพื่อ_พ_ร_ก__เราไม่ทิ้งกัน_2020__COVID_19_ { get { return "COVID" ; } }  
			
        }
		 
        public static class Plan_Release 
        {  
		    
            public const string _LOVGroupCode = "PLR";  
			 public static string จัดทำแผนฯ_ตั้งต้น { get { return "S" ; } }  
			 public static string ปรับปรุงแผนฯ_ครั้งที่_1 { get { return "M1" ; } }  
			 public static string ปรับปรุงแผนฯ_ครั้งที่_2 { get { return "M2" ; } }  
			
        }
		 
        public static class Plan_Status 
        {  
		    
            public const string _LOVGroupCode = "PLSTATUS";  
			 public static string ระหว่างการดำเนินงาน { get { return "OP" ; } }  
			 public static string รอแก้ไข { get { return "WU" ; } }  
			 public static string บรรจุลงแผน_สบน { get { return "PDMO" ; } }  
			 public static string ส่งแล้ว { get { return "ST" ; } }  
			 public static string สร้าง { get { return "C" ; } }  
			
        }
		 
        public static class Plan_Type 
        {  
		    
            public const string _LOVGroupCode = "PLTYPE";  
			 public static string แผน_5_ปี { get { return "MP" ; } }  
			 public static string แผนก่อหนี้ใหม่ { get { return "NP" ; } }  
			 public static string แผนบริหารหนี้เดิม { get { return "EP" ; } }  
			 public static string รายงานสถานะทางการเงินและภาระหนี้ { get { return "RP" ; } }  
			 public static string ข้อเสนอแผนบริหารหนี้ { get { return "PP" ; } }  
			 public static string รายงานประจำเดือน { get { return "R" ; } }  
			 public static string รายงานประจำเดือน__แผนก่อหนี้ใหม่_ { get { return "NPM" ; } }  
			 public static string รายงานประจำเดือน__แผนบริหารหนี้เดิม_ { get { return "EPM" ; } }  
			
        }
		 
        public static class Plan_of_Proposal_Status 
        {  
		    
            public const string _LOVGroupCode = "PPPSTATUS";  
			 public static string สร้างใหม่ { get { return "01" ; } }  
			 public static string เสนอแผนฯ
 { get { return "02" ; } }  
			 public static string รอแก้ไข
 { get { return "03" ; } }  
			 public static string เห็นชอบ_โดย_สบน_
 { get { return "05" ; } }  
			 public static string เห็นชอบโดย_คณะอนุฯ { get { return "06" ; } }  
			 public static string เห็นชอบและอนุมัติโดย_คนน_ { get { return "07" ; } }  
			
        }
		 
        public static class Proposal_Status 
        {  
		    
            public const string _LOVGroupCode = "PPSTATUS";  
			 public static string จัดทำข้อเสนอแผนฯ { get { return "01" ; } }  
			 public static string เห็นชอบโดย_คณะอนุฯ_ { get { return "05" ; } }  
			 public static string ระหว่างการทบทวนพิจารณาแผน { get { return "03" ; } }  
			 public static string รับข้อเสนอโดย_สบน_ { get { return "04" ; } }  
			 public static string เสนอแผน { get { return "02" ; } }  
			 public static string เห็นชอบและอนุมัติโดย_คนน_ { get { return "06" ; } }  
			
        }
		 
        public static class Request_Status 
        {  
		    
            public const string _LOVGroupCode = "RQSTATUS";  
			 public static string Pending { get { return "P" ; } }  
			 public static string ยื่นคำขอปรับปรุงข้อมูล { get { return "RU" ; } }  
			 public static string อนุมัติคำขอ { get { return "A" ; } }  
			 public static string ไม่อนุมัติคำขอ { get { return "RJ" ; } }  
			 public static string สร้างข้อมูล { get { return "C" ; } }  
			 public static string แก้ไขข้อมูล { get { return "U" ; } }  
			 public static string ยกเลิกคำขอ { get { return "CR" ; } }  
			
        }
		 
        public static class Request_Type 
        {  
		    
            public const string _LOVGroupCode = "RQTYPE";  
			 public static string Change_Organization_Data { get { return "ORG" ; } }  
			 public static string แผน_5_ปี { get { return "P5Y" ; } }  
			 public static string Change_log_history { get { return "LOG" ; } }  
			
        }
		 
        public static class สาขาย่อย 
        {  
		    
            public const string _LOVGroupCode = "SFIELD";  
			 public static string สาขาขนส่งทางบก { get { return "0101" ; } }  
			 public static string สาขาขนส่งทางน้ำ { get { return "0102" ; } }  
			 public static string สาขาขนส่งทางอากาศ { get { return "0103" ; } }  
			 public static string สาขาพลังงาน { get { return "0201" ; } }  
			 public static string สาขาสาธารณูปการ { get { return "0301" ; } }  
			 public static string สาขาสื่อสาร { get { return "0401" ; } }  
			 public static string สาขาอุตสาหกรรทและพาณิชยากรรม { get { return "0501" ; } }  
			 public static string สาขาเกษตร { get { return "0601" ; } }  
			 public static string สาขาทรัพยากรธรรมชาติ { get { return "0701" ; } }  
			 public static string สาขาสังคมและเทคโนโลยี { get { return "0801" ; } }  
			 public static string สาขาสถาบันการเงิน { get { return "0901" ; } }  
			
        }
		 
        public static class Source_of_Loan 
        {  
		    
            public const string _LOVGroupCode = "SOURCELOAN";  
			 public static string แหล่งเงินกู้ในประเทศ { get { return "01" ; } }  
			 public static string แหล่งเงินกู้ต่างประเทศ { get { return "02" ; } }  
			 public static string แหล่งเงินรายได้ { get { return "03" ; } }  
			 public static string เงินงบประมาณ { get { return "04" ; } }  
			 public static string เงินกองทุน { get { return "05" ; } }  
			 public static string เอกชนร่วมลงทุน { get { return "06" ; } }  
			 public static string อื่นๆ { get { return "99" ; } }  
			
        }
		 
        public static class System 
        {  
		    
            public const string _LOVGroupCode = "SYSTEM";  
			 public static string Develop_Environment { get { return "DEV" ; } }  
			
        }
		
    }
   
}

