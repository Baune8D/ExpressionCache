using System.Threading.Tasks;

namespace ExpressionCache.Redis.Tests.TestHelpers
{
    public class TestFunctionsFixture
    {
        public readonly string ReturnResult = "ReturnResult";

        public string FunctionWithoutParameters()
        {
            return ReturnResult;
        }

        public string OtherFunctionWithoutParameters()
        {
            return ReturnResult;
        }

        #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

        public async Task<string> FunctionWithoutParametersAsync()
        {
            return ReturnResult;
        }

        public async Task<string> OtherFunctionWithoutParametersAsync()
        {
            return ReturnResult;
        }

        #pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}
