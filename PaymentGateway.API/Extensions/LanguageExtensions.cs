using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.API.Common;
using PaymentGateway.Application.Common;
using System.Collections.Generic;
using System.Linq;

namespace PaymentGateway.API.Extensions
{
    public static class LanguageExtensions
    {
        public static ActionResult ToActionResult<TResult>(this Either<Seq<Failure>, TResult> either) => either.Match<ActionResult>(
            Right: _ => new NoContentResult(),
            Left: failures => new BadRequestObjectResult(failures.FormatProblemDetails()));

        private static CommandProblemDetails FormatProblemDetails(this IEnumerable<Failure> failures) => new CommandProblemDetails
        {
            Title = "One or more validation errors occurred.",
            Errors = failures.GroupBy(k => k.Id).ToDictionary(s => s.Key, s => s.Map(k => k.Error.Message).ToArray())
        };
    }
}
