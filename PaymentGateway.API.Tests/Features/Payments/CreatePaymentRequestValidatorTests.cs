using FluentAssertions;
using PaymentGateway.API.Features.Payments;
using System;
using Xunit;
using Xunit.Categories;

namespace PaymentGateway.API.Tests.Features.Payments
{
    [UnitTest]
    public class CreatePaymentRequestValidatorTests
    {
        private readonly CreatePaymentRequestValidator _classUnderTest;
        public CreatePaymentRequestValidatorTests()
        {
            _classUnderTest = new CreatePaymentRequestValidator();
        }

        [Fact]
        public void Validate_WhenEverythingSupplied_ExpectedNoValidationErrors()
        {
            var request = ValidPaymentRequest();

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_WhenNoCardDetailsSupplied_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.CardDetails = null;

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_NegativeCVV_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.CardDetails.CVV = -1;

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_ZeorCVV_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.CardDetails.CVV = 0;

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_LessThan3DigitsCVV_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.CardDetails.CVV = 99;

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_MoreThan4DigitsCVV_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.CardDetails.CVV = 1000;

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_CardNumberNull_ExpectedErrors()
        {
            CreatePaymentRequest request = ValidPaymentRequest();
            request.CardDetails.Number = null;

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_CardNumberEmpty_ExpectedErrors()
        {
            CreatePaymentRequest request = ValidPaymentRequest();
            request.CardDetails.Number = string.Empty;

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_InvalidCardNumber_ExpectedErrors()
        {
            CreatePaymentRequest request = ValidPaymentRequest();
            request.CardDetails.Number = "78549621";

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_NullExpirationDate_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.CardDetails.Expiration = null;

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_NotValidMonth_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.CardDetails.Expiration.Month = "13";

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_NullMonth_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.CardDetails.Expiration.Month = null;

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_EmptyMonth_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.CardDetails.Expiration.Month = string.Empty;

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_NegativeYear_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.CardDetails.Expiration.Year = -1;
            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_ZeorYear_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.CardDetails.Expiration.Year = 0;
            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_ExpiredYear_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.CardDetails.Expiration.Year = 1999;
            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_CurrentYearButMonthExpired_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.CardDetails.Expiration.Year = DateTime.Now.Year;
            request.CardDetails.Expiration.Month = DateTime.Now.AddMonths(-1).Month.ToString();
            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_NullAmount_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.Value = null;

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_ZeroAmount_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.Value.Amount = 0;

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_NegativeAmount_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.Value.Amount = -1;

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_NullCurrency_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.Value.Currency = null;

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }


        [Fact]
        public void Validate_EmptyCurrency_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.Value.Currency = string.Empty;

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_NonExistingCurrency_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.Value.Currency = "KKKKKL";

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_NotValidTimeStampMinValue_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.TimeStamp = DateTime.MinValue;

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_NotValidTimeStampMaxValue_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.TimeStamp = DateTime.MaxValue;

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_TimeStampIsInTheFuture_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.TimeStamp = DateTime.MaxValue;

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_TimeStampIsNotToday_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.TimeStamp = DateTime.MaxValue;

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_TimeStampIsNotPrecise_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.TimeStamp = DateTime.MaxValue;

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        private CreatePaymentRequest ValidPaymentRequest() =>
            new CreatePaymentRequest
            {
                CardDetails = new CreatePaymentRequest.CardDto
                {
                    CVV = 514,
                    Expiration = new CreatePaymentRequest.ExpirationDateDto
                    {
                        Month = "03",
                        Year = DateTime.Now.AddYears(3).Year
                    },
                    Number = "1111-2222-3333-4444"
                },
                Value = new CreatePaymentRequest.MoneyDto
                {
                    Amount = 150,
                    Currency = "EUR"
                },
                TimeStamp = DateTime.Now
            };

    }
}
