using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.API.Extensions;
using PaymentGateway.Application.Payments.Queries;

namespace PaymentGateway.API.Features.Payments
{
    [Route("[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> CreatePaymentAsync([FromBody]CreatePaymentRequest request)
        {
            var result = await _mediator.Send(request.ToCommand());

            return result.ToActionResult();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<GetPaymentsResponse>>> GetPaymentsAsync()
        {
            var result = await _mediator.Send(new GetPayments.Query());

            return Ok(result.ToResponse());
        }
    }
}