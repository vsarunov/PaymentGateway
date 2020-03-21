using FluentValidation;
using FluentValidation.Results;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace PaymentGateway.API.Features.Payments
{
    public class CreatePaymentRequestValidator : AbstractValidator<CreatePaymentRequest>
    {
        private const int CVVCeiling = 10000;
        private const int CVVFloor = 99;
        private const int MinPaymentAmount = 1;
        private const int MinMonth = 1;
        private const int MaxMonth = 12;

        public override FluentValidation.Results.ValidationResult Validate(ValidationContext<CreatePaymentRequest> context)
        {
            return (context.InstanceToValidate == null)
                ? new FluentValidation.Results.ValidationResult(new[] { new ValidationFailure("Request body", "Cannot be null") })
                : base.Validate(context);
        }

        public CreatePaymentRequestValidator()
        {
            RuleFor(x => x.CardDetails).NotNull().SetValidator(new CardDtoValidator());
            RuleFor(x => x.Value).NotNull().SetValidator(new MoneyDtoValidator());
            RuleFor(x => x.TimeStamp)
                .Must(BeValidDate).WithMessage("{PropertyName} Must be a valid date time")
                .Must(BeToday).WithMessage("{PropertyName} Must be todays date time")
                .Must(BePrecise).WithMessage("{PropertyName} Must be a precise")
                .WithName(nameof(CreatePaymentRequest.TimeStamp));
        }

        private bool BeValidDate(DateTime dateTime)
        {
            return !dateTime.Equals(default);
        }

        private bool BePrecise(DateTime dateTime)
        {
            return !dateTime.TimeOfDay.Equals(default);
        }

        private bool BeToday(DateTime dateTime)
        {
            return dateTime.Date.Equals(DateTime.Today);
        }

        private class CardDtoValidator : AbstractValidator<CreatePaymentRequest.CardDto>
        {
            public CardDtoValidator()
            {
                RuleFor(x => x.CVV).GreaterThan(CVVFloor).LessThan(CVVCeiling);
                RuleFor(x => x.Number).NotEmpty().Must(new CreditCardAttribute().IsValid)
                    .WithMessage("Card number is not valid");
                RuleFor(x => x.Expiration).NotNull().SetValidator(new ExpirationDateDtoValidator());
            }
        }

        private class MoneyDtoValidator : AbstractValidator<CreatePaymentRequest.MoneyDto>
        {
            public MoneyDtoValidator()
            {
                RuleFor(x => x.Amount).GreaterThanOrEqualTo(MinPaymentAmount);
                RuleFor(x => x.ISOCurrencyCode)
                    .Must(BeValidISOCurrencyCode)
                    .WithMessage("{PropertyName} Must be valid ISO currency code")
                    .WithName(nameof(CreatePaymentRequest.MoneyDto.ISOCurrencyCode));
            }

            private bool BeValidISOCurrencyCode(string isoCode)
            {
                CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

                foreach (CultureInfo ci in cultures)
                {
                    RegionInfo ri = new RegionInfo(ci.LCID);
                    if (ri.ISOCurrencySymbol == isoCode)
                    {
                        return true;
                    }
                }
                return false;
            }

        }

        private class ExpirationDateDtoValidator : AbstractValidator<CreatePaymentRequest.ExpirationDateDto>
        {
            public ExpirationDateDtoValidator()
            {
                RuleFor(x => x.Year).GreaterThanOrEqualTo(DateTime.UtcNow.Year).DependentRules(() =>
                {
                    RuleFor(x => x.Month).GreaterThanOrEqualTo(MinMonth).LessThanOrEqualTo(MaxMonth)
                    .DependentRules(() =>
                    {
                        RuleFor(x => new DateTime(x.Year, x.Month, 1)).Must(BeInTheFuture).WithMessage("Card expired");
                    });
                });
            }

            private bool BeInTheFuture(DateTime dateTime)
            {
                return dateTime > DateTime.UtcNow.Date;
            }
        }
    }
}
