using System.Threading.Tasks;

namespace ExpressionCache.Core.Tests.TestHelpers
{
    public class TestFunctionsFixture
    {
        public string ClassName => GetType().Name;

        public readonly string ReturnResult = "ReturnResult";

        public string FunctionWithoutParameters()
        {
            return ReturnResult;
        }

        public string FunctionWithOneParameter(int number)
        {
            return ReturnResult + number;
        }

        public string FunctionWithTwoParameters(int number, string text)
        {
            return ReturnResult + number + text;
        }

        #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

        public async Task<string> FunctionWithoutParametersAsync()
        {
            return FunctionWithoutParameters();
        }

        public async Task<string> FunctionWithOneParameterAsync(int number)
        {
            return FunctionWithOneParameter(number);
        }

        public async Task<string> FunctionWithTwoParametersAsync(int number, string text)
        {
            return FunctionWithTwoParameters(number, text);
        }

        #pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}
