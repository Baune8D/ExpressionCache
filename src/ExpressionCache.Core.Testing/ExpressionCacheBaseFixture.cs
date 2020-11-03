using System;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace ExpressionCache.Core.Testing
{
    public sealed class ExpressionCacheBaseFixture : IDisposable
    {
        public const string CachedResult = "CachedResult";

        public static readonly TimeSpan TimeSpan = TimeSpan.FromHours(1);

        private readonly MemoryCacheWrapper _memoryCacheWrapper;

        public ExpressionCacheBaseFixture()
        {
            _memoryCacheWrapper = new MemoryCacheWrapper(new MemoryCacheOptions());
            CacheProviderMock = new Mock<IExpressionCacheProvider>();
            ExpressionCacheBase = new ExpressionCacheBaseWrapper(_memoryCacheWrapper, CacheProviderMock.Object);
        }

        public Mock<IExpressionCacheProvider> CacheProviderMock { get; }

        public ExpressionCacheBaseWrapper ExpressionCacheBase { get; }

        public void Dispose()
        {
            _memoryCacheWrapper.Dispose();
        }

        public void SetCacheProviderGetSuccess()
        {
            CacheProviderMock
                .Setup(m => m.Get<string>(It.IsAny<string>()))
                .Returns(new CacheResult<string>
                {
                    Success = true,
                    Content = CachedResult,
                });
        }

        public void SetCacheProviderGetAsyncSuccess()
        {
            CacheProviderMock
                .Setup(m => m.GetAsync<string>(It.IsAny<string>()))
                .ReturnsAsync(new CacheResult<string>
                {
                    Success = true,
                    Content = CachedResult,
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
    }
}
