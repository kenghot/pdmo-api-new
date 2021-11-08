using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using mof.ServiceModels.Identity;

namespace mof.ServiceModels.Request
{
    public class Login
    {
        /// <summary>
        /// User code (รหัสผู้ใช้)
        /// </summary>
        [Required]
        public string UserName { get; set; }
        /// <summary>
        /// User name (ชื่อผู้ใช้)
        /// </summary>
        [Required]
        public string Password { get; set; }
        /// <summary>
        /// Not use (ไม่ได้ใช้งาน)
        /// </summary>
        public bool RememberLogin { get; set; }
    }
    public class Paging
    {
        /// <summary>
        /// Serch text
        /// </summary>
        public string SearchText { get; set; }
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 100;
        /// <summary>
        /// Sorting
        /// </summary>
        public Sorting SortBy { get; set; }

    }
    public class Sorting
    {
        /// <summary>
        /// column ที่ต้องการ Sort
        /// </summary>
        [Required]
        public string ColumnName { get; set; }
        /// <summary>
        /// true = มากไปน้อย
        /// </summary>
        [Required]
        public bool OrderByDescending { get; set; } = true;
    }
    public class Register
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public string ConfrimUrl { get; set; }
        
        public ApplicationUser AppUser { get; set; }
    }
  
}
