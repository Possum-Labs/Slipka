using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Slipka.ApiArguments
{
    public class IsFutureAttribute : ValidationAttribute
    {

        public IsFutureAttribute()
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if(!TimeSpan.TryParse(value.ToString(), out TimeSpan ts))
                return new ValidationResult($"Unable to parse timespan for {validationContext.DisplayName} I recieved {value} how about 0.05:30:10 for 0 days, 5 hours, 30 minutes and 10 seconds");

            if (ts <= new TimeSpan(0,0,0))
                return new ValidationResult($"The timespan for {validationContext.DisplayName} needs to be atleast {new TimeSpan(0, 0, 0)} where as {ts} was provided");

            return ValidationResult.Success;
        }
    }
}
