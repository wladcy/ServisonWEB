using Admin.Services;
using Rental.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;

namespace Rental.Validators
{
    public class PasswordValidator : ValidationAttribute
    {
        private Stopwatch s = new Stopwatch();
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            ValidationResult retval = new ValidationResult("");
            IPasswordModel rvm = (IPasswordModel)validationContext.ObjectInstance;
            int nonLetterAndDigit = 0;
            int digit = 0;
            int upperCase = 0;
            int lowerCase = 0;
            string password = string.Empty;
            if (rvm.Password == null)
            {
                password = rvm.NewPassword;
            }
            else
            {
                password = rvm.Password;
            }
            foreach (char c in password)
            {
                if (char.IsDigit(c))
                    digit++;
                else if (!char.IsLetterOrDigit(c))
                    nonLetterAndDigit++;
                else if (char.IsUpper(c))
                    upperCase++;
                else if (char.IsLower(c))
                    lowerCase++;
            }

            if (nonLetterAndDigit == 0)
                retval.ErrorMessage += "Hasło powinno zawierać przynajmniej jeden znak niealfanumeryczny.";
            if (digit == 0)
                retval.ErrorMessage += "Hasło powinno zawierać przynajmniej jedną cyfrę.";
            if (upperCase == 0)
                retval.ErrorMessage += "Hasło powinno zawierać przynajmniej jedną dużą literę.";
            if (lowerCase == 0)
                retval.ErrorMessage += "Hasło powinno zawierać przynajmniej jedną małą literę.";

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
