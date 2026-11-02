using System.Threading.Tasks;

namespace ExpressionCache.Core.Tests.Data;

public class InstanceFunctions(string value)
{
    public string LoadValue()
    {
        return value;
    }

    public Task<string> LoadValueAsync()
    {
        return Task.FromResult(value);
    }
}
