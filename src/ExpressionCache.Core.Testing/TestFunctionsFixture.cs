using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace ExpressionCache.Core.Testing
{
    public class TestFunctionsFixture
    {
        public const string ReturnResult = "ReturnResult";

        public string ClassName => GetType().Name;

        [SuppressMessage("Microsoft.Performance", "CA1822", Justification = "Testing purpose")]
        public string NullFunctionWithoutParameters()
        {
            return null;
        }

        [SuppressMessage("Microsoft.Performance", "CA1822", Justification = "Testing purpose")]
        public string FunctionWithoutParameters()
        {
            return ReturnResult;
        }

        [SuppressMessage("Microsoft.Performance", "CA1822", Justification = "Testing purpose")]
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Testing purpose")]
        public string FunctionWithOneParameter(int number)
        {
            return ReturnResult;
        }

        [SuppressMessage("Microsoft.Performance", "CA1822", Justification = "Testing purpose")]
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Testing purpose")]
        public string FunctionWithTwoParameters(int number, string text)
        {
            return ReturnResult;
        }

        [SuppressMessage("Microsoft.Performance", "CA1822", Justification = "Testing purpose")]
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Testing purpose")]
        public string FunctionWithObjectParameter(CacheableObject obj)
        {
            return ReturnResult;
        }

        [SuppressMessage("Microsoft.Performance", "CA1822", Justification = "Testing purpose")]
        public Task<string> NullFunctionWithoutParametersAsync()
        {
            return Task.FromResult((string)null);
        }

        [SuppressMessage("Microsoft.Performance", "CA1822", Justification = "Testing purpose")]
        public Task<string> FunctionWithoutParametersAsync()
        {
            return Task.FromResult(ReturnResult);
        }

        [SuppressMessage("Microsoft.Performance", "CA1822", Justification = "Testing purpose")]
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Testing purpose")]
        public Task<string> FunctionWithOneParameterAsync(int number)
        {
            return Task.FromResult(ReturnResult);
        }

        [SuppressMessage("Microsoft.Performance", "CA1822", Justification = "Testing purpose")]
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Testing purpose")]
        public Task<string> FunctionWithTwoParametersAsync(int number, string text)
        {
            return Task.FromResult(ReturnResult);
        }

        [SuppressMessage("Microsoft.Performance", "CA1822", Justification = "Testing purpose")]
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Testing purpose")]
        public string FunctionWithObjectParameterAsync(CacheableObject obj)
        {
            return ReturnResult;
        }
    }
}
