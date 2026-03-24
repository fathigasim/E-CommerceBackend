using EcommerceApplication.Common.Validators.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Common.Validators
{
    public abstract class LocalizedValidator<T> : AbstractValidator<T>
    {
        protected readonly IStringLocalizer<ValidationMessages> Localizer;

        protected LocalizedValidator(IStringLocalizer<ValidationMessages> localizer)
        {
            Localizer = localizer;
        }

        // Helper methods for common validations
        protected string Required => Localizer["Required"];
        protected string MaxLength => Localizer["MaxLength"];
        protected string MinLength => Localizer["MinLength"];
        protected string Email => Localizer["Email"];
        protected string GreaterThan => Localizer["GreaterThan"];
        protected string InvalidFormat => Localizer["InvalidFormat"];
    }
}
