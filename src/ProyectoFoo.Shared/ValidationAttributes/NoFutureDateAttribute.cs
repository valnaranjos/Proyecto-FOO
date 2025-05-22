using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Shared.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class NotFutureDateAttribute : ValidationAttribute
    {
        public NotFutureDateAttribute()
            : base("La fecha de {0} no puede ser en el futuro.")
        {
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dateTimeValue)
            {
              
                if (dateTimeValue.Date > DateTime.UtcNow.Date)
                {
                    return new ValidationResult(
                        FormatErrorMessage(validationContext.DisplayName),
                        [validationContext.MemberName!]
                    );
                }
            }
           
            return ValidationResult.Success;
        }
    }
}
