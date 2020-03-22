using AutoMoqCore;
using FluentAssertions;
using LanguageExt;
using LanguageExt.UnitTesting;
using MediatR;
using Moq;
using PaymentGateway.Application.Banks.Queries;
using PaymentGateway.Application.Cards.Queries;
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
            _mocker.GetMock<IMediator>()
                  .Setup(x => x.Send(It.IsAny<GetBankPaymentVerification>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(true);

            _mocker.GetMock<IMediator>()
                  .Setup(x => x.Send(It.IsAny<GetCardByNumber>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(Option<Domain.Card>.None);

            var paymentListMock = new List<Payment>();

            _mocker.GetMock<IPaymentRepository>()
                  .Setup(x => x.GetPaymentsByUserIdAsync(It.IsAny<Guid>()))
                  .ReturnsAsync(paymentListMock);

            _mocker.GetMock<IPaymentRepository>()
                  .Setup(x => x.SavePaymentAsync(It.IsAny<Payment>()))
                  .ReturnsAsync(1);

            var requestMock = new CreatePayment
                (
                Guid.NewGuid(),
                DateTime.UtcNow,
                new Common.Money(25, "EUR"),
                new Common.Card("1234567891234567", 541, new Common.ExpirationDate(2025, 1))
                );

            var result = await _classUnderTest.Handle(requestMock, It.IsAny<CancellationToken>());

            result.ShouldBeRight(response => response.Should().Be(1));

            _mocker.GetMock<IPaymentRepository>().Verify(x => x.SavePaymentAsync(It.IsAny<Payment>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenEverythingIsValidWithExistingPreviousPayments_ExpectedSuccessAsync()
        {
            var userIdMock = Guid.NewGuid();

            _mocker.GetMock<IMediator>()
                  .Setup(x => x.Send(It.IsAny<GetBankPaymentVerification>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(true);

            _mocker.GetMock<IMediator>()
                  .Setup(x => x.Send(It.IsAny<GetCardByNumber>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(Option<Domain.Card>.None);

            _mocker.GetMock<IPaymentRepository>()
                  .Setup(x => x.SavePaymentAsync(It.IsAny<Payment>()))
                  .ReturnsAsync(1);

            var paymentListMock = new List<Payment>()
            {
                new Payment( Guid.NewGuid(), new Domain.Card(userIdMock,"1234567891234567", 123, new Domain.ExpirationDate(2025,1)) , new Domain.Money(25,"EUR"), DateTime.UtcNow)
            };

            _mocker.GetMock<IPaymentRepository>()
                      .Setup(x => x.GetPaymentsByUserIdAsync(It.IsAny<Guid>()))
                      .ReturnsAsync(paymentListMock);

            var requestMock = new CreatePayment
                  (
                  userIdMock,
                  DateTime.UtcNow,
                  new Common.Money(25, "EUR"),
                  new Common.Card("1234567891234567", 541, new Common.ExpirationDate(2025, 1))
                  );

            var result = await _classUnderTest.Handle(requestMock, It.IsAny<CancellationToken>());

            result.ShouldBeRight(response => response.Should().Be(1));

            _mocker.GetMock<IPaymentRepository>().Verify(x => x.SavePaymentAsync(It.IsAny<Payment>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenPaymentAlreadyExsist_ExpectedFailureAsync()
        {
            _mocker.GetMock<IMediator>()
                  .Setup(x => x.Send(It.IsAny<GetBankPaymentVerification>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(true);

            _mocker.GetMock<IMediator>()
                  .Setup(x => x.Send(It.IsAny<GetCardByNumber>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(Option<Domain.Card>.None);

            var paymentTimeStampMock = DateTime.UtcNow;
            var userIdMock = Guid.NewGuid();

            var paymentListMock = new List<Payment>()
            {
                new Payment(userIdMock, new Domain.Card(userIdMock,"1234567891234567", 541, new Domain.ExpirationDate(2025,1)) , new Domain.Money(25,"EUR"), paymentTimeStampMock)
            };

            _mocker.GetMock<IPaymentRepository>()
                      .Setup(x => x.GetPaymentsByUserIdAsync(It.IsAny<Guid>()))
                      .ReturnsAsync(paymentListMock);

            var requestMock = new CreatePayment
                  (
                  userIdMock,
                  paymentTimeStampMock,
                  new Common.Money(25, "EUR"),
                  new Common.Card("1234567891234567", 541, new Common.ExpirationDate(2025, 1))
                  );

            var result = await _classUnderTest.Handle(requestMock, It.IsAny<CancellationToken>());

            result.ShouldBeLeft(fail =>
            {
                fail.Should().NotBeNull();
            });

            _mocker.GetMock<IPaymentRepository>().Verify(x => x.SavePaymentAsync(It.IsAny<Payment>()), Times.Never);
            _mocker.GetMock<IMediator>().Verify(x => x.Send(It.IsAny<GetBankPaymentVerification>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenSameCardDifferentUser_ExpectedFailureAsync()
        {
            _mocker.GetMock<IMediator>()
                  .Setup(x => x.Send(It.IsAny<GetBankPaymentVerification>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(true);

            var cardResponseMock = new Domain.Card(Guid.NewGuid(), "1234567891234567", 541, new Domain.ExpirationDate(2025, 1));

            _mocker.GetMock<IMediator>()
                  .Setup(x => x.Send(It.IsAny<GetCardByNumber>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(Option<Domain.Card>.Some(cardResponseMock));

            var paymentTimeStampMock = DateTime.UtcNow;
            var userIdMock = Guid.NewGuid();

            var paymentListMock = new List<Payment>()
            {
                new Payment(Guid.NewGuid(), new Domain.Card(userIdMock, "1234567891234567", 541, new Domain.ExpirationDate(2025,1)) , new Domain.Money(30,"EUR"), DateTime.UtcNow.AddSeconds(-10))
            };

            _mocker.GetMock<IPaymentRepository>()
                      .Setup(x => x.GetPaymentsByUserIdAsync(It.IsAny<Guid>()))
                      .ReturnsAsync(paymentListMock);



            _mocker.GetMock<ICardRepository>()
                      .Setup(x => x.GetCardByNumberAsync(It.IsAny<string>()))
                      .ReturnsAsync(cardResponseMock);

            var requestMock = new CreatePayment
                  (
                  userIdMock,
                  paymentTimeStampMock,
                  new Common.Money(25, "EUR"),
                  new Common.Card("1234567891234567", 541, new Common.ExpirationDate(2025, 1))
                  );

            var result = await _classUnderTest.Handle(requestMock, It.IsAny<CancellationToken>());

            result.ShouldBeLeft(fail =>
            {
                fail.Should().NotBeNull();
            });
            _mocker.GetMock<IPaymentRepository>().Verify(x => x.SavePaymentAsync(It.IsAny<Payment>()), Times.Never);
            _mocker.GetMock<IMediator>().Verify(x => x.Send(It.IsAny<GetBankPaymentVerification>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Hanlde_WhenBankValidationFails_ExpectedPaymentSaveAndFailureAsync()
        {
            _mocker.GetMock<IMediator>()
                   .Setup(x => x.Send(It.IsAny<GetBankPaymentVerification>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(false);

            _mocker.GetMock<IMediator>()
                  .Setup(x => x.Send(It.IsAny<GetCardByNumber>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(Option<Domain.Card>.None);

            var paymentListMock = new List<Payment>();

            _mocker.GetMock<IPaymentRepository>()
                  .Setup(x => x.GetPaymentsByUserIdAsync(It.IsAny<Guid>()))
                  .ReturnsAsync(paymentListMock);

            _mocker.GetMock<IPaymentRepository>()
                  .Setup(x => x.SavePaymentAsync(It.IsAny<Payment>()))
                  .ReturnsAsync(1);

            var requestMock = new CreatePayment
                  (
                  Guid.NewGuid(),
                  DateTime.UtcNow,
                  new Common.Money(25, "EUR"),
                  new Common.Card("1234567891234567", 541, new Common.ExpirationDate(2025, 1))
                  );

            var result = await _classUnderTest.Handle(requestMock, It.IsAny<CancellationToken>());

            result.ShouldBeLeft(fail =>
            {
                fail.Should().NotBeNull();
            });

            _mocker.GetMock<IPaymentRepository>().Verify(x => x.SavePaymentAsync(It.IsAny<Payment>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenPaymentAlreadyExistsAndCardAlreadyInUse_ExpectedFailAsync()
        {
            _mocker.GetMock<IMediator>()
                  .Setup(x => x.Send(It.IsAny<GetBankPaymentVerification>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(true);

            var cardResponseMock = new Domain.Card(Guid.NewGuid(), "1234567891234567", 541, new Domain.ExpirationDate(2025, 1));

            _mocker.GetMock<IMediator>()
                  .Setup(x => x.Send(It.IsAny<GetCardByNumber>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(Option<Domain.Card>.Some(cardResponseMock));

            var paymentTimeStampMock = DateTime.UtcNow;
            var userIdMock = Guid.NewGuid();

            var paymentListMock = new List<Payment>()
            {
                new Payment(userIdMock, new Domain.Card(userIdMock,"1234567891234567", 541, new Domain.ExpirationDate(2025,1)) , new Domain.Money(25,"EUR"), paymentTimeStampMock)
            };

            _mocker.GetMock<IPaymentRepository>()
                  .Setup(x => x.GetPaymentsByUserIdAsync(It.IsAny<Guid>()))
                  .ReturnsAsync(paymentListMock);

            _mocker.GetMock<IPaymentRepository>()
                  .Setup(x => x.SavePaymentAsync(It.IsAny<Payment>()))
                  .ReturnsAsync(1);

            var requestMock = new CreatePayment
                (
                userIdMock,
                paymentTimeStampMock,
                new Common.Money(25, "EUR"),
                new Common.Card("1234567891234567", 541, new Common.ExpirationDate(2025, 1))
                );

            var result = await _classUnderTest.Handle(requestMock, It.IsAny<CancellationToken>());

            result.ShouldBeLeft(fail =>
            {
                fail.Should().NotBeNull();
            });

            _mocker.GetMock<IPaymentRepository>().Verify(x => x.SavePaymentAsync(It.IsAny<Payment>()), Times.Never);
            _mocker.GetMock<IMediator>().Verify(x => x.Send(It.IsAny<GetBankPaymentVerification>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
