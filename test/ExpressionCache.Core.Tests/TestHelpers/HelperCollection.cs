using Xunit;

namespace ExpressionCache.Core.Tests.TestHelpers
{
    [CollectionDefinition("HelperCollection")]
    public class HelperCollection :
        ICollectionFixture<TestFunctionsFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
