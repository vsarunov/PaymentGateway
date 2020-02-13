using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.API.Features.Payments
{
    public class CreatePaymentRequestValidator : AbstractValidator<CreatePaymentRequest>
    {
        public CreatePaymentRequestValidator()
        {
            RuleFor(x => x.CardDetails).NotNull().SetValidator(new CardDtoValidator());
            RuleFor(x => x.Value).NotNull().SetValidator(new MoneyDtoValidator());
            RuleFor(x => x.TimeStamp)
                .Must(BeValidDate).WithMessage("{PropertyName} Must be a valid date time")
                .Must(BeToday).WithMessage("{PropertyName} Must Be todays date time")
                .Must(BeInThePast).WithMessage("{PropertyName} Must occur in the past")
                .Must(BePrecise).WithMessage("{PropertyName} Must be a precise")
                .WithName(nameof(CreatePaymentRequest.TimeStamp));
        }

        private class CardDtoValidator : AbstractValidator<CreatePaymentRequest.CardDto>
        {

        }

        private class ExpirationDateDtoValidator : AbstractValidator<CreatePaymentRequest.ExpirationDateDto>
        {

        }

        private class MoneyDtoValidator : AbstractValidator<CreatePaymentRequest.MoneyDto>
        {

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

        private bool BeInThePast(DateTime dateTime)
        {
            return dateTime.TimeOfDay < DateTime.UtcNow.TimeOfDay;
        }
    }
}
