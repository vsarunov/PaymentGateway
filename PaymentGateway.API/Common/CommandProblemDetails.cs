using System.Collections.Generic;

namespace PaymentGateway.API.Common
{
    public class CommandProblemDetails : Microsoft.AspNetCore.Mvc.ProblemDetails
    {
        public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
    }
}
