using Admin.Services;
using ServisonWEB.Models.ServisonViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ServisonWEB.Validators
{
    public class NameValidator : ValidationAttribute
    {
        private Stopwatch s = new Stopwatch();
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            LoggerController.AddBeginMethodLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            s.Restart();
            ValidationResult retval = new ValidationResult("");
            ClientViewModel rvm = (ClientViewModel)validationContext.ObjectInstance;
            bool isEmpty = string.IsNullOrEmpty(rvm.Name);
            bool isSpace = false;
            int letterts = 0;
            if (!isEmpty)
            {
                foreach (char c in rvm.Name)
                {
                    if (char.IsWhiteSpace(c))
                        isSpace = true;
                    else if (char.IsLetter(c))
                        letterts++;
                }
            }

            bool isLastNameEmpty = false;
            if (isSpace)
            {
                string[] tmp = rvm.Name.Split(' ');
                isLastNameEmpty = string.IsNullOrEmpty(tmp[1]);                
            }
            

            if (isEmpty)
                retval.ErrorMessage += "Imie i nazwisko nie może być puste.";
            if ((!isSpace && !isEmpty) || isLastNameEmpty)
                retval.ErrorMessage += "Nie podano nazwiska.";
            if (letterts == 0)
                retval.ErrorMessage += "Podany ciąg znaków nie jest ani imieniem ani nazwiskiem.";            

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
