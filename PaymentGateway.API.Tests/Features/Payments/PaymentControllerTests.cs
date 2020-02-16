using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Categories;
using PaymentGateway.Domain;
using PaymentGateway.API.Features.Payments;
using MediatR;
using Moq;
using PaymentGateway.Application.Payments.Commands;
using PaymentGateway.Application.Common;
using System.Threading;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using PaymentGateway.Application.Payments.Queries;
using AutoMoqCore;

namespace PaymentGateway.API.Tests.Features.Payments
{
    [UnitTest]
    public class PaymentControllerTests
    {
        private readonly PaymentController _classUnderTest;
        private readonly AutoMoqer _mocker;
        public PaymentControllerTests()
        {
            _mocker = new AutoMoqer();
            _classUnderTest = _mocker.Resolve<PaymentController>();
        }

        #region CreatePaymentAsync

        [Fact]
        public async Task CreatePaymentAsync_WhenReturnsError_ExpectedBadRequestAsync()
        {
            CreatePayment.Command actualCommand = null;

            var createPayment = new Seq<Failure>
            {
                Failure.Of(Mock.Of<Payment>(), "Value -1 has to be non-negative")
            };

            _mocker.GetMock<IMediator>()
                  .Setup(x => x.Send(It.IsAny<CreatePayment.Command>(), It.IsAny<CancellationToken>()))
                  .Callback<IRequest<Either<Seq<Failure>, int>>, CancellationToken>((command, ct) => actualCommand = (CreatePayment.Command)command)
                  .ReturnsAsync(Left<Seq<Failure>, int>(createPayment));

            var request = new CreatePaymentRequest
            {
                CardDetails = new CreatePaymentRequest.CardDto
                {
                    CVV = 123,
                    Expiration = new CreatePaymentRequest.ExpirationDateDto
                    {
                        Month = 3,
                        Year = 2033
                    },
                    Number = "1234-5678-9101-1121"
                },
                Value = new CreatePaymentRequest.MoneyDto
                {
                    Amount = 12345,
                    ISOCurrencyCode = "EUR"
                }
            };

            var response = await _classUnderTest.CreatePaymentAsync(request);

            var expectedommand = new CreatePayment.Command
            {
                CardDetails = new CreatePayment.Command.Card
                {
                    CVV = 123,
                    Expiration = new CreatePayment.Command.ExpirationDate
                    {
                        Month = 3,
                        Year = 2033
                    },
                    Number = "1234-5678-9101-1121"
                },
                Value = new CreatePayment.Command.Money
                {
                    Amount = 12345,
                    Currency = "EUR"
                }
            };

            response.Should().BeOfType<BadRequestObjectResult>();
            _mocker.GetMock<IMediator>().Verify(x => x.Send(It.IsAny<CreatePayment.Command>(), It.IsAny<CancellationToken>()), Times.Once);
            actualCommand.Should().BeEquivalentTo(expectedommand);
        }

        [Fact]
        public async Task CreatePaymentAsync_WhenNoError_ExpectedNoContentAsync()
        {
            CreatePayment.Command actualCommand = null;

            _mocker.GetMock<IMediator>()
                  .Setup(x => x.Send(It.IsAny<CreatePayment.Command>(), It.IsAny<CancellationToken>()))
                  .Callback<IRequest<Either<Seq<Failure>, int>>, CancellationToken>((command, ct) => actualCommand = (CreatePayment.Command)command)
                  .ReturnsAsync(Right<Seq<Failure>, int>(It.IsAny<int>()));

            var request = new CreatePaymentRequest
            {
                CardDetails = new CreatePaymentRequest.CardDto
                {
                    CVV = 123,
                    Expiration = new CreatePaymentRequest.ExpirationDateDto
                    {
                        Month = 3,
                        Year = 2033
                    },
                    Number = "1234-5678-9101-1121"
                },
                Value = new CreatePaymentRequest.MoneyDto
                {
                    Amount = 12345,
                    ISOCurrencyCode = "EUR"
                }
            };

            var response = await _classUnderTest.CreatePaymentAsync(request);

            var expectedommand = new CreatePayment.Command
            {
                CardDetails = new CreatePayment.Command.Card
                {
                    CVV = 123,
                    Expiration = new CreatePayment.Command.ExpirationDate
                    {
                        Month = 3,
                        Year = 2033
                    },
                    Number = "1234-5678-9101-1121"
                },
                Value = new CreatePayment.Command.Money
                {
                    Amount = 12345,
                    Currency = "EUR"
                }
            };

            response.Should().BeOfType<NoContentResult>();
            _mocker.GetMock<IMediator>().Verify(x => x.Send(It.IsAny<CreatePayment.Command>(), It.IsAny<CancellationToken>()), Times.Once);
            actualCommand.Should().BeEquivalentTo(expectedommand);
        }

        #endregion

        #region GetPaymentsAsync

        [Theory]
        [MemberData(nameof(PaymentCollections))]
        public async Task GetPaymentsAsync_GivenValidRequest_ReturnsEntitiesAsync(IEnumerable<Payment> queryResponseMock)
        {
            GetPayments.Query actualQuery = null;

            _mocker.GetMock<IMediator>()
                  .Setup(x => x.Send(It.IsAny<GetPayments.Query>(), It.IsAny<CancellationToken>()))
                  .Callback<IRequest<IEnumerable<Payment>>, CancellationToken>((query, ct) => actualQuery = (GetPayments.Query)query)
                  .ReturnsAsync(queryResponseMock);

            var response = await _classUnderTest.GetPaymentsAsync();

            var okResult = response.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(queryResponseMock);
            _mocker.GetMock<IMediator>().Verify(x => x.Send(It.IsAny<GetPayments.Query>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region Test data

        public static IEnumerable<object[]> PaymentCollections => new[]
        {
            new object[] { new List<Payment> { Mock.Of<Payment>(), Mock.Of<Payment>() } },
            new object[] { new List<Payment> () },
        };

        #endregion
    }
}
