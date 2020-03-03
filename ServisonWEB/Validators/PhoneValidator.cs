using Admin.Services;
using Rental.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;

namespace Rental.Validators
{
    public class PhoneValidator : ValidationAttribute
    {
        private Stopwatch s = new Stopwatch();
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            ValidationResult retval = new ValidationResult("");
            IPhoneModel rvm = (IPhoneModel)validationContext.ObjectInstance;
            if (string.IsNullOrEmpty(rvm.PhoneNumber))
            {
                retval.ErrorMessage += "Numer telefonu nie może być pusty.";
            }
            else
            {
                string values = rvm.PhoneNumber.Replace("+48", "");
                int digit = 0;
                foreach (char c in values)
                {
                    if (char.IsDigit(c))
                        digit++;
                }


                if (digit != 9)
                    retval.ErrorMessage += "Numer telefonu składa się z 12 cyfr.";
            }
            s.Stop();
            LoggerController.AddEndMethodLog(this.GetType().Name,
                MethodBase.GetCurrentMethod().Name, s.ElapsedMilliseconds);

            return string.IsNullOrEmpty(retval.ErrorMessage) ? ValidationResult.Success : retval;
        }

        public ValidationResult IsValid(ValidationContext context)
        {
            return IsValid(new object(), context);
        }
    }
}
