using System.ComponentModel.DataAnnotations;

namespace LgkProductions.Inspector;

public static class Extensions
{
    /// <inheritdoc cref="Validator.TryValidateObject(object, ValidationContext, ICollection{ValidationResult}?)"/>
    public static bool IsValid(this object instance, ICollection<ValidationResult>? validationResults = null)
        => Validator.TryValidateObject(instance, new ValidationContext(instance), validationResults, validateAllProperties: true);

    /// <inheritdoc cref="Validator.TryValidateObject(object, ValidationContext, ICollection{ValidationResult}?)"/>
    public static bool IsValid(this object instance, out ICollection<ValidationResult> validationResults)
    {
        validationResults = [];
        return instance.IsValid(validationResults);
    }
}
