using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class AttachFile
    {
        public AttachFile()
        {
            PlanAttach = new HashSet<PlanAttach>();
            PlanProjectFile = new HashSet<PlanProjectFile>();
            PlanProjectResolution = new HashSet<PlanProjectResolution>();
            ProjectResolution = new HashSet<ProjectResolution>();
        }

        public long AttachFileId { get; set; }
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public long? RecordId { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public byte[] TimeStamp { get; set; }
        public string FileDetail { get; set; }
        public string FileExtension { get; set; }
        public byte[] FileData { get; set; }
        public DateTime? CreateDt { get; set; }

        public virtual ICollection<PlanAttach> PlanAttach { get; set; }
        public virtual ICollection<PlanProjectFile> PlanProjectFile { get; set; }
        public virtual ICollection<PlanProjectResolution> PlanProjectResolution { get; set; }
        public virtual ICollection<ProjectResolution> ProjectResolution { get; set; }
    }
}
