using System;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace ExpressionCache.Core.Testing
{
    public class ExpressionCacheBaseFixture : IDisposable
    {
        public static readonly TimeSpan TimeSpan = TimeSpan.FromHours(1);

        public readonly string CachedResult = "CachedResult";

        public Mock<IExpressionCacheProvider> CacheProviderMock { get; private set; }
        public ExpressionCacheBaseWrapper ExpressionCacheBase { get; private set; }

        public ExpressionCacheBaseFixture()
        {
            var internalCacheMock = new MemoryCacheWrapper(new MemoryCacheOptions());
            CacheProviderMock = new Mock<IExpressionCacheProvider>();
            ExpressionCacheBase = new ExpressionCacheBaseWrapper(internalCacheMock, CacheProviderMock.Object);
        }

        public void SetCacheProviderGetSuccess()
        {
            CacheProviderMock
                .Setup(m => m.Get<string>(It.IsAny<string>()))
                .Returns(new CacheResult<string>
                {
                    Success = true,
                    Content = CachedResult
                });
        }

        public void SetCacheProviderGetAsyncSuccess()
        {
            CacheProviderMock
                .Setup(m => m.GetAsync<string>(It.IsAny<string>()))
                .ReturnsAsync(new CacheResult<string>
                {
                    Success = true,
                    Content = CachedResult
                });
        }

        public void SetCacheProviderGetFailure()
        {
            CacheProviderMock
                .Setup(m => m.Get<string>(It.IsAny<string>()))
                .Returns(CacheResult<string>.Failure);
        }

        public void SetCacheProviderGetAsyncFailure()
        {
            CacheProviderMock
                .Setup(m => m.GetAsync<string>(It.IsAny<string>()))
                .ReturnsAsync(CacheResult<string>.Failure);
        }

        public void Dispose()
        {
            ExpressionCacheBase = null;
            CacheProviderMock = null;
        }
    }
}
