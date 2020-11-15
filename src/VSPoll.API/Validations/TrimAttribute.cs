using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace VSPoll.API.Validations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TrimAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (validationContext.MemberName is string memberName
                && validationContext.ObjectType.GetProperty(memberName) is PropertyInfo property)
            {
                var str = property?.GetValue(validationContext.ObjectInstance) as string;
                property?.SetValue(validationContext.ObjectInstance, str?.Trim());
            }
            return ValidationResult.Success;
        }
    }
}
