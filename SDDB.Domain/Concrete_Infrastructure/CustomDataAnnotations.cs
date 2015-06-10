using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;

namespace SDDB.Domain.Infrastructure
{
    //DBIsUnique---------------------------------------------------------------------------------------------------------------//
    
    public class DBIsUniqueAttribute : ValidationAttribute, IClientValidatable
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = "true";
            rule.ValidationType = "dbisunique";
            yield return rule;
        }
    }

    //DBIsInteger--------------------------------------------------------------------------------------------------------------//
    
    public class DBIsIntegerAttribute : ValidationAttribute, IClientValidatable
    {
        public DBIsIntegerAttribute() : base("{0} is not an integer.") { }

        protected override ValidationResult IsValid( object value, ValidationContext validationContext)
        {
            int output; 
            if (value != null && !int.TryParse(value.ToString(), out output))
                    return new ValidationResult( FormatErrorMessage(validationContext.DisplayName));
            else
                return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = FormatErrorMessage(metadata.GetDisplayName());
            rule.ValidationType = "dbisinteger";
            yield return rule;
        }
    }

    //DBIsBool-----------------------------------------------------------------------------------------------------------------//
    
    public class DBIsBoolAttribute : ValidationAttribute, IClientValidatable
    {
        public DBIsBoolAttribute() : base("{0} has to be 'true' or 'false'.") { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            bool output;
            if (value != null && !bool.TryParse(value.ToString(), out output))
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            else
                return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = FormatErrorMessage(metadata.GetDisplayName());
            rule.ValidationType = "dbisbool";
            yield return rule;
        }
    }

    //DBIsDateISO--------------------------------------------------------------------------------------------------------------//

    public class DBIsDateISOAttribute : ValidationAttribute, IClientValidatable
    {
        public DBIsDateISOAttribute() : base("{0} has to be in 'YYYY-MM-DD' format.") { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime output;
            if (value != null && !DateTime.TryParseExact(value.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out output))
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            else
                return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = FormatErrorMessage(metadata.GetDisplayName());
            rule.ValidationType = "dbisdateiso";
            yield return rule;
        }
    }

    //DBIsDateTimeISO----------------------------------------------------------------------------------------------------------//

    public class DBIsDateTimeISOAttribute : ValidationAttribute, IClientValidatable
    {
        public DBIsDateTimeISOAttribute() : base("{0} has to be in 'YYYY-MM-dd HH:mm' format.") { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime output;
            if (value != null && !DateTime.TryParseExact(value.ToString(), "YYYY-MM-DD HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out output))
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            else
                return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = FormatErrorMessage(metadata.GetDisplayName());
            rule.ValidationType = "dbisdatetimeiso";
            yield return rule;
        }
    }

}
