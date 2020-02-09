using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.API.Features.Payments
{
    public class CreatePaymentRequestValidator: AbstractValidator<CreatePaymentRequest>
    {
    }
}
