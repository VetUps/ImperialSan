using System.ComponentModel.DataAnnotations;

namespace ImperialSanAPI.Utils.CustomValidationAttributes
{
    public class AllowedValuesAttribute : ValidationAttribute
    {
        private readonly string[] _allowedValues;

        public AllowedValuesAttribute(params string[] allowedValues)
        {
            _allowedValues = allowedValues ?? throw new ArgumentNullException(nameof(allowedValues));
        }

        public override bool IsValid(object? value)
        {
            return _allowedValues.Contains(value);
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} должно быть одним из следующих значений: {string.Join(' ', _allowedValues)}";
        }
    }
}
