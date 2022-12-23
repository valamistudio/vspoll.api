using System;
using System.ComponentModel.DataAnnotations;

namespace VSPoll.API.Validations;

[AttributeUsage(AttributeTargets.Property)]
public class TrimAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (validationContext.MemberName is { } memberName
            && validationContext.ObjectType.GetProperty(memberName) is { } property)
        {
            var str = property.GetValue(validationContext.ObjectInstance) as string;
            property.SetValue(validationContext.ObjectInstance, str?.Trim());
        }
        return ValidationResult.Success;
    }
}
