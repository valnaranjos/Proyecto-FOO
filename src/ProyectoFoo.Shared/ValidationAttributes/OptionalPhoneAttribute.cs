using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFoo.Shared.ValidationAttributes
{
    public class OptionalPhoneAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var phone = value as string;

            if (string.IsNullOrEmpty(phone))
            {
                return ValidationResult.Success;
            }

            var phoneAttribute = new PhoneAttribute();
            if (!phoneAttribute.IsValid(phone))
            {
                return new ValidationResult(ErrorMessage ?? "Número de teléfono no válido.");
            }

            return ValidationResult.Success;
        }
    }
}
