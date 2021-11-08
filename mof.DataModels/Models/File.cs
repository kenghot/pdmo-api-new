using System;
using System.Collections.Generic;

namespace mof.DataModels.Models
{
    public partial class File
    {
        public long FileId { get; set; }
        public byte[] FileContent { get; set; }
    }
}
