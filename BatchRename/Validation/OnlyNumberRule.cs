using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BatchRename
{
    class OnlyNumberRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string fileName = (string)value;
            ValidationResult result = ValidationResult.ValidResult;
            if (fileName.Length > 0)
            {
                string pattern = @"^\d+$";
                var regex = new Regex(pattern);
                if (regex.IsMatch(fileName) == false)
                {
                    result = new ValidationResult(false, "Illegal characters");
                }
            }
            else
            {
                result = new ValidationResult(false, "Required");
            }
            return result;
        }
    }
}
