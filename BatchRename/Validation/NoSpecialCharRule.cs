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
    class NoSpecialCharRule : ValidationRule
    { 
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string fileName = (string)value;
            ValidationResult result = ValidationResult.ValidResult;
            if (fileName.Length>0)
            {
                string pattern = "^[^\\\\/:*?\"<>|]+$";
                var regex = new Regex(pattern);
                if (regex.IsMatch(fileName))
                {

                }
                else
                {
                    result = new ValidationResult(false,
                        "A filename can't contain: \\ / : * ? \" < > | ");
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
