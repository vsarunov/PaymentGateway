using System;
using System.Collections.Generic;
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
            // This user Id getting could and should be done better. However just to keep the contract keeping it here.
            var userId = User.FindFirst("sub")?.Value;

            if (userId == null)
            {
                return BadRequest("User not identified");
            }

            var result = await _mediator.Send(request.ToCommand(new Guid(userId)));

            return result.ToActionResult();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<GetPaymentsResponse>>> GetPaymentsAsync()
        {
            // This user Id getting could and should be done better. However just to keep the contract keeping it here.
            var userId = User.FindFirst("sub")?.Value;

            if (userId == null)
            {
                return BadRequest("User not identified");
            }

            var result = await _mediator.Send(new GetPayments.Query(new Guid(userId)));

            return Ok(result.ToResponse());
        }
    }
}