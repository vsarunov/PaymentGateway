using AutoMoqCore;
using FluentAssertions;
using LanguageExt.UnitTesting;
using Moq;
using PaymentGateway.Application.Common;
using PaymentGateway.Application.Payments.Commands;
using PaymentGateway.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Categories;

namespace PaymentGateway.Application.Tests.Payments.Commands
{
    [UnitTest]
    public class CreatePaymentHandlerTests
    {
        private readonly CreatePaymentHandler _classUnderTest;
        private readonly AutoMoqer _mocker;

        public CreatePaymentHandlerTests()
        {
            _mocker = new AutoMoqer();
            _classUnderTest = _mocker.Resolve<CreatePaymentHandler>();
        }

        [Fact]
        public async Task Handle_WhenEverythingIsValidAndNoPreviousPayments_ExpectedSuccessAsync()
        {
            _mocker.GetMock<IBankRepository>()
                  .Setup(x => x.IsValidPayment(It.IsAny<Payment>()))
                  .ReturnsAsync(true);

            var paymentListMock = new List<Payment>();

            _mocker.GetMock<IPaymentRepository>()
                  .Setup(x => x.GetPaymentsByUserIdAsync(It.IsAny<Guid>()))
                  .ReturnsAsync(paymentListMock);

            var requestMock = new CreatePayment
            {
                CardDetails = new CreatePayment.Card
                {
                    CVV = 541,
                    Number = "1234567891234567",
                    Expiration = new CreatePayment.ExpirationDate
                    {
                        Month = 1,
                        Year = 2025,
                    }
                },
                Value = new CreatePayment.Money
                {
                    Amount = 25,
                    Currency = "EUR"
                },
                TimeStamp = DateTime.UtcNow,
                UserId = Guid.NewGuid()
            };

            var result = await _classUnderTest.Handle(requestMock, It.IsAny<CancellationToken>());

            result.ShouldBeRight(response => response.Should().Be(1));
        }

        [Fact]
        public async Task Handle_WhenEverythingIsValidWithExistingPreviousPayments_ExpectedSuccessAsync()
        {
            _mocker.GetMock<IBankRepository>()
                  .Setup(x => x.IsValidPayment(It.IsAny<Payment>()))
                  .ReturnsAsync(true);

            var paymentListMock = new List<Payment>()
            {
                new Payment( Guid.NewGuid(), new Card("1234567891234567", 123, new ExpirationDate(2025,1)) , new Money(25,"EUR"), DateTime.UtcNow)
            };

            _mocker.GetMock<IPaymentRepository>()
                      .Setup(x => x.GetPaymentsByUserIdAsync(It.IsAny<Guid>()))
                      .ReturnsAsync(paymentListMock);

            var requestMock = new CreatePayment
            {
                CardDetails = new CreatePayment.Card
                {
                    CVV = 541,
                    Number = "1234567891234567",
                    Expiration = new CreatePayment.ExpirationDate
                    {
                        Month = 1,
                        Year = 2025,
                    }
                },
                Value = new CreatePayment.Money
                {
                    Amount = 25,
                    Currency = "EUR"
                },
                TimeStamp = DateTime.UtcNow.AddSeconds(-10)
            };

            var result = await _classUnderTest.Handle(requestMock, It.IsAny<CancellationToken>());

            result.ShouldBeRight(response => response.Should().Be(1));
        }

        [Fact]
        public async Task Handle_WhenPaymentAlreadyExsist_ExpectedFailureAsync()
        {
            _mocker.GetMock<IBankRepository>()
                 .Setup(x => x.IsValidPayment(It.IsAny<Payment>()))
                 .ReturnsAsync(true);

            var paymentTimeStampMock = DateTime.UtcNow;
            var userIdMock = Guid.NewGuid();

            var paymentListMock = new List<Payment>()
            {
                new Payment(userIdMock, new Card("1234567891234567", 541, new ExpirationDate(2025,1)) , new Money(25,"EUR"), paymentTimeStampMock)
            };

            _mocker.GetMock<IPaymentRepository>()
                      .Setup(x => x.GetPaymentsByUserIdAsync(It.IsAny<Guid>()))
                      .ReturnsAsync(paymentListMock);

            var requestMock = new CreatePayment
            {
                CardDetails = new CreatePayment.Card
                {
                    CVV = 541,
                    Number = "1234567891234567",
                    Expiration = new CreatePayment.ExpirationDate
                    {
                        Month = 1,
                        Year = 2025,
                    }
                },
                Value = new CreatePayment.Money
                {
                    Amount = 25,
                    Currency = "EUR"
                },
                TimeStamp = paymentTimeStampMock,
                UserId = userIdMock
            };

            var result = await _classUnderTest.Handle(requestMock, It.IsAny<CancellationToken>());

            result.ShouldBeLeft(fail =>
            {
                fail.Should().NotBeNull();
            });
        }

        [Fact]
        public async Task Handle_WhenSameCardDifferentUser_ExpectedFailureAsync()
        {
            _mocker.GetMock<IBankRepository>()
            .Setup(x => x.IsValidPayment(It.IsAny<Payment>()))
            .ReturnsAsync(true);

            var paymentTimeStampMock = DateTime.UtcNow;
            var userIdMock = Guid.NewGuid();

            var paymentListMock = new List<Payment>()
            {
                new Payment(Guid.NewGuid(), new Card("1234567891234567", 541, new ExpirationDate(2025,1)) , new Money(30,"EUR"), DateTime.UtcNow.AddSeconds(-10))
            };

            _mocker.GetMock<IPaymentRepository>()
                      .Setup(x => x.GetPaymentsByUserIdAsync(It.IsAny<Guid>()))
                      .ReturnsAsync(paymentListMock);

            var requestMock = new CreatePayment
            {
                CardDetails = new CreatePayment.Card
                {
                    CVV = 541,
                    Number = "1234567891234567",
                    Expiration = new CreatePayment.ExpirationDate
                    {
                        Month = 1,
                        Year = 2025,
                    }
                },
                Value = new CreatePayment.Money
                {
                    Amount = 25,
                    Currency = "EUR"
                },
                TimeStamp = paymentTimeStampMock,
                UserId = userIdMock
            };

            var result = await _classUnderTest.Handle(requestMock, It.IsAny<CancellationToken>());

            result.ShouldBeLeft(fail =>
            {
                fail.Should().NotBeNull();
            });
        }

        [Fact]
        public async Task Hanlde_WhenBankValidationFails_ExpectedFailureAsync()
        {
            _mocker.GetMock<IBankRepository>()
                  .Setup(x => x.IsValidPayment(It.IsAny<Payment>()))
                  .ReturnsAsync(false);

            var paymentListMock = new List<Payment>();

            _mocker.GetMock<IPaymentRepository>()
                  .Setup(x => x.GetPaymentsByUserIdAsync(It.IsAny<Guid>()))
                  .ReturnsAsync(paymentListMock);

            var requestMock = new CreatePayment
            {
                CardDetails = new CreatePayment.Card
                {
                    CVV = 541,
                    Number = "1234567891234567",
                    Expiration = new CreatePayment.ExpirationDate
                    {
                        Month = 1,
                        Year = 2025,
                    }
                },
                Value = new CreatePayment.Money
                {
                    Amount = 25,
                    Currency = "EUR"
                },
                TimeStamp = DateTime.UtcNow,
                UserId = Guid.NewGuid()
            };

            var result = await _classUnderTest.Handle(requestMock, It.IsAny<CancellationToken>());

            result.ShouldBeLeft(fail =>
            {
                fail.Should().NotBeNull();
            });
        }
    }
}
