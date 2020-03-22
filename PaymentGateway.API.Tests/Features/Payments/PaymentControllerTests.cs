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
using System;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Linq;

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
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("sub", Guid.NewGuid().ToString()),
            }, "mock"));

            _classUnderTest.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            CreatePayment actualCommand = null;

            var paymentFailure = GetMockedPayment();

            var createPayment = new Seq<Failure>
            {
                Failure.Of(paymentFailure, "Value -1 has to be non-negative")
            };

            _mocker.GetMock<IMediator>()
                  .Setup(x => x.Send(It.IsAny<CreatePayment>(), It.IsAny<CancellationToken>()))
                  .Callback<IRequest<Either<Seq<Failure>, int>>, CancellationToken>((command, ct) => actualCommand = (CreatePayment)command)
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
            response.Should().BeOfType<BadRequestObjectResult>();
            _mocker.GetMock<IMediator>().Verify(x => x.Send(It.IsAny<CreatePayment>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreatePaymentAsync_WhenNoError_ExpectedNoContentAsync()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("sub", Guid.NewGuid().ToString()),
            }, "mock"));

            _classUnderTest.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            CreatePayment actualCommand = null;

            _mocker.GetMock<IMediator>()
                  .Setup(x => x.Send(It.IsAny<CreatePayment>(), It.IsAny<CancellationToken>()))
                  .Callback<IRequest<Either<Seq<Failure>, int>>, CancellationToken>((command, ct) => actualCommand = (CreatePayment)command)
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

            response.Should().BeOfType<NoContentResult>();
            _mocker.GetMock<IMediator>().Verify(x => x.Send(It.IsAny<CreatePayment>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreatePaymentAsync_WhenNoSubClaim_ExpectedBadRequestAsync()
        {
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

            response.Should().BeOfType<BadRequestObjectResult>();
            _mocker.GetMock<IMediator>().Verify(x => x.Send(It.IsAny<CreatePayment>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion

        #region GetPaymentsAsync

        [Theory]
        [MemberData(nameof(PaymentCollections))]
        public async Task GetPaymentsAsync_GivenValidRequest_ReturnsEntitiesAsync(IEnumerable<Payment> queryResponseMock)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("sub", Guid.NewGuid().ToString()),
            }, "mock"));

            _classUnderTest.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            GetPaymentsByUserId actualQuery = null;

            _mocker.GetMock<IMediator>()
                  .Setup(x => x.Send(It.IsAny<GetPaymentsByUserId>(), It.IsAny<CancellationToken>()))
                  .Callback<IRequest<IEnumerable<Payment>>, CancellationToken>((query, ct) => actualQuery = (GetPaymentsByUserId)query)
                  .ReturnsAsync(queryResponseMock);

            var expectedResponse = queryResponseMock.Select(x => new GetPaymentsResponse
            {
                Id = x.Id,
                PaymentStatus = x.Status.ToString(),
                CardNumber = new string(x.Card?.Number?.Select((p, index) => index <= 12 ? '*' : p).ToArray()),
                Value = new GetPaymentsResponse.MoneyResponse
                {
                    Amount = x.Value.Amount,
                    ISOCurrencyCode = x.Value.ISOCurrencyCode
                }
            });

            var response = await _classUnderTest.GetPaymentsAsync();

            var okResult = response.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(expectedResponse);
            _mocker.GetMock<IMediator>().Verify(x => x.Send(It.IsAny<GetPaymentsByUserId>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetPaymentsAsync_GivenNoSubClaim_ReturnsBadRequestAsync()
        {
            var response = await _classUnderTest.GetPaymentsAsync();

            var okResult = response.Result.Should().BeOfType<BadRequestObjectResult>();
            _mocker.GetMock<IMediator>().Verify(x => x.Send(It.IsAny<GetPaymentsByUserId>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion

        #region Test data

        public static IEnumerable<object[]> PaymentCollections => new[]
        {
            new object[] { new List<Payment> { GetMockedPayment(), GetMockedPayment() } },
            new object[] { new List<Payment> () },
        };

        private static Payment GetMockedPayment()
        {
            var userGuidMock = Guid.NewGuid();
            var paymentFailure = new Payment
                (
                userGuidMock,
                new Domain.Card(userGuidMock, "1234567891011123", 123, new Domain.ExpirationDate(2020, 7)),
                new Domain.Money(123, "GPB"),
                DateTime.UtcNow
                );
            return paymentFailure;
        }

        #endregion
    }
}
