using System.ComponentModel.DataAnnotations;


namespace SRCore.Utils
{
    public class DirectoryExistsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var directoryPath = value as string;
            if (directoryPath == null)
            {
                return new ValidationResult("The path should not be empty.");
            }

            if (!Directory.Exists(directoryPath))
            {
                return new ValidationResult("The folder does not exist.");
            }

            return ValidationResult.Success;
        }
    }
}
