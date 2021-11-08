using mof.DataModels.Models;
using mof.IServices;
using mof.ServiceModels.Common.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace mof.Services.Helper
{
    public class CopyEnitiy
    {
        public static AttachFileData NewAttachFileData(AttachFile af)
        {
            var ret = new AttachFileData { FileDetail = af.FileDetail, FileExtension = af.FileExtension, FileName = af.FileName, FileSize = af.FileSize,  ID = af.AttachFileId };
            return ret;
        }
        public static AttachFile NewAttachFile(AttachFileData af,string tabName , string fieldName, long? attachID, long? recID)
        {
            var ret = new AttachFile { FileDetail = af.FileDetail, FileExtension = af.FileExtension, FileName = af.FileName, FileSize = af.FileSize,  
             FieldName = fieldName, TableName = tabName };
            if (attachID.HasValue)
            {
                ret.AttachFileId = attachID.Value;
            }
            if (recID.HasValue)
            {
                ret.RecordId = recID.Value;
            }
            return ret;
        }

        public static void GetLOVBasicData(ISystemHelper help, BasicData basic, string GroupCode)
        {
            var b = help.GetLOVByCode(basic.Code, GroupCode);
            if (b.IsCompleted)
            {
                basic.Description = b.Data.LOVValue;
                basic.ID = b.Data.LOVKey;

            }
            else
            {
                basic.Description = "";
                basic.ID = 0;
            }
        }
    }
}
