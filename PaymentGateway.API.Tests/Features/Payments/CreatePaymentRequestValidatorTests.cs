using FluentAssertions;
using FluentValidation.TestHelper;
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
        public void Validate_NullObject_ExpectedErrors()
        {
            CreatePaymentRequest request = null;

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_WhenNoCardDetailsSupplied_ExpectedErrors()
        {
            _classUnderTest.ShouldHaveValidationErrorFor(r => r.CardDetails, null as CreatePaymentRequest.CardDto);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(99)]
        [InlineData(10000)]
        public void Validate_CVVInvalidValues_ExpectedErrors(int errorValue)
        {
            var request = ValidPaymentRequest();
            request.CardDetails.CVV = errorValue;

            _classUnderTest.ShouldHaveValidationErrorFor(r => r.CardDetails.CVV, request);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("78549621")]
        public void Validate_CardNumberInvalidValues_ExpectedErrors(string errorCardNumber)
        {
            var request = ValidPaymentRequest();
            request.CardDetails.Number = errorCardNumber;

            _classUnderTest.ShouldHaveValidationErrorFor(r => r.CardDetails.Number, request);
        }

        [Fact]
        public void Validate_NullExpirationDate_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.CardDetails.Expiration = null;

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData(13)]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_ExpirationInvalidMonth_ExpectedErrors(int errorMonth)
        {
            var request = ValidPaymentRequest();
            request.CardDetails.Expiration.Month = errorMonth;

            _classUnderTest.ShouldHaveValidationErrorFor(r => r.CardDetails.Expiration.Month, request);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1999)]
        public void Validate_ExpirationInvalidYear_ExpectedErrors(int errorYear)
        {
            var request = ValidPaymentRequest();
            request.CardDetails.Expiration.Year = errorYear;

            _classUnderTest.ShouldHaveValidationErrorFor(r => r.CardDetails.Expiration.Year, request);
        }

        [Fact]
        public void Validate_CurrentYearButMonthExpired_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.CardDetails.Expiration.Year = DateTime.Now.Year;
            request.CardDetails.Expiration.Month = DateTime.Now.AddMonths(-1).Month;
            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_NullValueForMoney_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.Value = null;

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_InvalidAmount_ExpectedErrors(decimal errorAmount)
        {
            var request = ValidPaymentRequest();
            request.Value.Amount = errorAmount;

            _classUnderTest.ShouldHaveValidationErrorFor(r => r.Value.Amount, request);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("KKKKKL")]
        public void Validate_InvalidISOCurrencyCode_ExpectedErrors(string errorISOCurrencyCode)
        {
            var request = ValidPaymentRequest();
            request.Value.ISOCurrencyCode = errorISOCurrencyCode;

            _classUnderTest.ShouldHaveValidationErrorFor(r => r.Value.ISOCurrencyCode, request);
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
            request.TimeStamp = DateTime.UtcNow.AddDays(1);

            var result = _classUnderTest.Validate(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_TimeStampIsNotPrecise_ExpectedErrors()
        {
            var request = ValidPaymentRequest();
            request.TimeStamp = DateTime.UtcNow.Date;

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
                        Month = 3,
                        Year = DateTime.Now.AddYears(3).Year
                    },
                    Number = "1111-2222-3333-4444"
                },
                Value = new CreatePaymentRequest.MoneyDto
                {
                    Amount = 150,
                    ISOCurrencyCode = "EUR"
                },
                TimeStamp = DateTime.Now
            };

    }
}
