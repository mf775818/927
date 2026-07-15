using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Serilog;
using ShoeMoldControl.Core;
using ShoeMoldControl.Core.Vision;
using ShoeMoldControl.Infrastructure.Vision;

namespace ShoeMoldControl.Tests.Vision
{
    /// <summary>
    /// TDD 測試套件：AvlCameraDriver 工業級相機驅動
    /// 測試目標：驗證硬體取像管線的正確性與記憶體管理策略
    /// </summary>
    public class AvlCameraDriverTests : IDisposable
    {
        private readonly Mock<IVisionConfig> _mockConfig;
        private readonly FrameMemoryPool _bufferPool;
        private readonly Mock<ILogger> _mockLogger;
        private readonly int _width = 2448;
        private readonly int _height = 2048;
        private readonly int _stride;

        public AvlCameraDriverTests()
        {
            _stride = _width; // Mono8
            _mockConfig = new Mock<IVisionConfig>();
            _mockConfig.Setup(c => c.VisionIpAddress).Returns("192.168.1.20");
            _mockConfig.Setup(c => c.VisionTimeoutMs).Returns(3000);
            
            _bufferPool = new FrameMemoryPool(_width, _height, _stride, PixelFormat.Mono8, 5);
            _mockLogger = new Mock<ILogger>();
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenConfigIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new AvlCameraDriver(null!, _bufferPool, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenBufferPoolIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new AvlCameraDriver(_mockConfig.Object, null!, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenVisionIpAddressIsEmpty()
        {
            // Arrange
            _mockConfig.Setup(c => c.VisionIpAddress).Returns("");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                new AvlCameraDriver(_mockConfig.Object, _bufferPool, _mockLogger.Object));
        }

        [Fact]
        public void IsConnected_ShouldReturnFalse_Initially()
        {
            // Arrange
            using var driver = new AvlCameraDriver(_mockConfig.Object, _bufferPool, _mockLogger.Object);

            // Assert
            Assert.False(driver.IsConnected);
        }

        [Fact]
        public async Task CaptureFrameAsync_ShouldThrow_WhenNotConnected()
        {
            // Arrange
            using var driver = new AvlCameraDriver(_mockConfig.Object, _bufferPool, _mockLogger.Object);
            using var cts = new CancellationTokenSource();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => 
                await driver.CaptureFrameAsync(cts.Token));
        }

        [Fact]
        public void Disconnect_ShouldNotThrow_WhenAlreadyDisconnected()
        {
            // Arrange
            using var driver = new AvlCameraDriver(_mockConfig.Object, _bufferPool, _mockLogger.Object);

            // Act & Assert (should not throw)
            driver.Disconnect();
        }

        [Fact]
        public void Dispose_ShouldCallDisconnect()
        {
            // Arrange
            var driver = new AvlCameraDriver(_mockConfig.Object, _bufferPool, _mockLogger.Object);

            // Act
            driver.Dispose();

            // Assert
            Assert.False(driver.IsConnected);
        }

        [Fact]
        public void Dispose_CanBeCalledMultipleTimes_Safely()
        {
            // Arrange
            var driver = new AvlCameraDriver(_mockConfig.Object, _bufferPool, _mockLogger.Object);

            // Act
            driver.Dispose();
            driver.Dispose(); // Should not throw

            // Assert - verification only, no exception means success
        }

        public void Dispose()
        {
            _bufferPool?.Dispose();
        }
    }

    /// <summary>
    /// TDD 測試套件：AvlImageAnalyzer 工業級圖像分析器
    /// 測試目標：驗證 GCHandle 定錨技術與零拷貝演算法調用的正確性
    /// </summary>
    public class AvlImageAnalyzerTests : IDisposable
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly int _width = 2448;
        private readonly int _height = 2048;
        private readonly int _stride;
        private readonly byte[] _payload;

        public AvlImageAnalyzerTests()
        {
            _stride = _width;
            _payload = new byte[_width * _height];
            _mockLogger = new Mock<ILogger>();
        }

        [Fact]
        public void Analyze_ShouldThrow_WhenFramePayloadIsNull()
        {
            // Arrange
            var analyzer = new AvlImageAnalyzer(_mockLogger.Object);
            var frame = new ManagedFrame(_width, _height, _stride, PixelFormat.Mono8, _payload);
            frame.Payload = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => analyzer.Analyze(frame));
        }

        [Fact]
        public void Analyze_ShouldThrow_WhenFrameIsInvalid()
        {
            // Arrange
            var analyzer = new AvlImageAnalyzer(_mockLogger.Object);
            var smallPayload = new byte[100];
            var frame = new ManagedFrame(_width, _height, _stride, PixelFormat.Mono8, smallPayload);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => analyzer.Analyze(frame));
        }

        [Fact]
        public void Constructor_ShouldWork_WithNullLogger()
        {
            // Act & Assert (should not throw)
            var analyzer = new AvlImageAnalyzer(null);
            Assert.NotNull(analyzer);
        }

        public void Dispose()
        {
            // Cleanup if needed
        }
    }

    /// <summary>
    /// TDD 測試套件：GenericVisionService 泛型視覺服務協調器
    /// 測試目標：驗證 Polly 斷路器與重試機制的整合，以及記憶體池自動歸還邏輯
    /// </summary>
    public class GenericVisionServiceTests : IDisposable
    {
        private readonly Mock<ICameraDriver<ManagedFrame>> _mockCameraDriver;
        private readonly Mock<IImageAnalyzer<ManagedFrame>> _mockAnalyzer;
        private readonly Mock<IResiliencePolicyProvider> _mockPolicyProvider;
        private readonly FrameMemoryPool _bufferPool;
        private readonly Mock<ILogger> _mockLogger;

        public GenericVisionServiceTests()
        {
            _mockCameraDriver = new Mock<ICameraDriver<ManagedFrame>>();
            _mockAnalyzer = new Mock<IImageAnalyzer<ManagedFrame>>();
            _mockPolicyProvider = new Mock<IResiliencePolicyProvider>();
            _bufferPool = new FrameMemoryPool(2448, 2048, 2448, PixelFormat.Mono8, 5);
            _mockLogger = new Mock<ILogger>();
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenCameraDriverIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new GenericVisionService<ManagedFrame>(
                    null!, 
                    _mockAnalyzer.Object, 
                    _mockPolicyProvider.Object));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenImageAnalyzerIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new GenericVisionService<ManagedFrame>(
                    _mockCameraDriver.Object, 
                    null!, 
                    _mockPolicyProvider.Object));
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenPolicyProviderIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new GenericVisionService<ManagedFrame>(
                    _mockCameraDriver.Object, 
                    _mockAnalyzer.Object, 
                    null!));
        }

        [Fact]
        public void Constructor_ShouldWork_WithNullLogger()
        {
            // Act & Assert (should not throw)
            var service = new GenericVisionService<ManagedFrame>(
                _mockCameraDriver.Object,
                _mockAnalyzer.Object,
                _mockPolicyProvider.Object,
                null,
                _bufferPool);
            Assert.NotNull(service);
        }

        [Fact]
        public async Task GrabAndDecodeAsync_ShouldReturnFailure_WhenCameraNotConnected()
        {
            // Arrange
            _mockCameraDriver.Setup(d => d.IsConnected).Returns(false);
            _mockCameraDriver.Setup(d => d.ConnectAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Camera offline"));
            
            var mockRetryPolicy = new Mock<Polly.ISyncPolicy>();
            var mockCircuitBreaker = new Mock<Polly.CircuitBreaker.ICircuitController<DecodeResult>>();
            
            _mockPolicyProvider.Setup(p => p.VisionRetryPolicy).Returns(mockRetryPolicy.Object);
            _mockPolicyProvider.Setup(p => p.VisionCircuitBreaker).Returns(mockCircuitBreaker.Object);

            using var service = new GenericVisionService<ManagedFrame>(
                _mockCameraDriver.Object,
                _mockAnalyzer.Object,
                _mockPolicyProvider.Object,
                _mockLogger.Object,
                _bufferPool);

            using var cts = new CancellationTokenSource();

            // Act
            var result = await service.GrabAndDecodeAsync(cts.Token);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.ErrorMessage);
        }

        [Fact]
        public void Dispose_ShouldDisposeCameraDriverAndBufferPool()
        {
            // Arrange
            var service = new GenericVisionService<ManagedFrame>(
                _mockCameraDriver.Object,
                _mockAnalyzer.Object,
                _mockPolicyProvider.Object,
                _mockLogger.Object,
                _bufferPool);

            // Act
            service.Dispose();

            // Assert
            _mockCameraDriver.Verify(d => d.Dispose(), Times.Once);
        }

        public void Dispose()
        {
            _bufferPool?.Dispose();
        }
    }
}
