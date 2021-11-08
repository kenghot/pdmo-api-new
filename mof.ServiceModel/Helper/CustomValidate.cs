 
using System.Text;
using System;
using System.Collections.Generic;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace mof.ServiceModels.Helper
{
    public class StringRangeAttribute : ValidationAttribute
    {
        public string[] AllowableValues { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (AllowableValues?.Contains(value?.ToString()) == true)
            {
                return ValidationResult.Success;
            }

            var msg = $"Please enter one of the allowable values: {string.Join(", ", (AllowableValues ?? new string[] { "No allowable values found" }))}.";
            return new ValidationResult(msg);
        }
    }

    public class IntRangeAttribute : ValidationAttribute
    {
        public int[] AllowableValues { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
             
 
            if (AllowableValues?.Contains((int)value) == true)
            {
                return ValidationResult.Success;
            }
            string txt = "";
            if (AllowableValues != null)
            {
                foreach (var t in AllowableValues)
                {
                    txt += t.ToString() + " ";
                }
            }else
            {
                txt = "No allowable values found";
            }
            var msg = $"Please enter one of the allowable values: {txt}.";
            return new ValidationResult( msg);
        }
    }
}
