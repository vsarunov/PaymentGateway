using System.Threading.Tasks;
using LanguageExt;
using PaymentGateway.Application.Common;

namespace PaymentGateway.Application.Extensions
{
    public static class LanguageExtensions
    {
        public static Task<Either<Seq<Failure>, R>> ToEitherAsync<R>(this Validation<Failure, Task<R>> validation) => validation.ToEither()
            .MapLeft(errors => errors)
            .MapAsync<Seq<Failure>, Task<R>, R>(e => e);
    }
}
