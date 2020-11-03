using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace ExpressionCache.Distributed.Testing
{
    public class TestFunctionsFixture
    {
        public const string ReturnResult = "ReturnResult";

        [SuppressMessage("Microsoft.Performance", "CA1822", Justification = "Testing purpose")]
        public string FunctionWithoutParameters()
        {
            return ReturnResult;
        }

        [SuppressMessage("Microsoft.Performance", "CA1822", Justification = "Testing purpose")]
        public string OtherFunctionWithoutParameters()
        {
            return ReturnResult;
        }

        [SuppressMessage("Microsoft.Performance", "CA1822", Justification = "Testing purpose")]
        public Task<string> FunctionWithoutParametersAsync()
        {
            return Task.FromResult(ReturnResult);
        }

        [SuppressMessage("Microsoft.Performance", "CA1822", Justification = "Testing purpose")]
        public Task<string> OtherFunctionWithoutParametersAsync()
        {
            return Task.FromResult(ReturnResult);
        }
    }
}
