using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Serilog;
using ShoeMoldControl.Core;
using ShoeMoldControl.Core.Vision;
using ShoeMoldControl.Infrastructure.Vision;
using ShoeMoldControl.Vision;

namespace ShoeMoldControl.Tests.Vision
{
    /// <summary>
    /// TDD 測試套件：FrameMemoryPool 工業級幀緩衝區物件池
    /// 測試目標：驗證零配置 (Zero-Allocation) 記憶體池化技術的正確性
    /// </summary>
    public class FrameMemoryPoolTests : IDisposable
    {
        private readonly int _width = 2448;
        private readonly int _height = 2048;
        private readonly int _stride;
        private readonly PixelFormat _format = PixelFormat.Mono8;
        private readonly int _capacity = 5;

        public FrameMemoryPoolTests()
        {
            _stride = _width; // Mono8: 1 byte per pixel
        }

        [Fact]
        public void Constructor_ShouldPreAllocateExactNumberOfBuffers()
        {
            // Arrange
            var expectedCapacity = 5;

            // Act
            using var pool = new FrameMemoryPool(_width, _height, _stride, _format, expectedCapacity);

            // Assert
            Assert.Equal(expectedCapacity, pool.AvailableCount);
            Assert.Equal(0, pool.TotalRented);
            Assert.Equal(0, pool.TotalReturned);
        }

        [Fact]
        public void Rent_ShouldDecreaseAvailableCountAndIncreaseTotalRented()
        {
            // Arrange
            using var pool = new FrameMemoryPool(_width, _height, _stride, _format, _capacity);
            var initialCount = pool.AvailableCount;

            // Act
            var frame = pool.Rent();

            // Assert
            Assert.NotNull(frame);
            Assert.Equal(initialCount - 1, pool.AvailableCount);
            Assert.Equal(1, pool.TotalRented);
            Assert.False(frame.IsReturned);
        }

        [Fact]
        public void Return_ShouldIncreaseAvailableCountAndMarkFrameAsReturned()
        {
            // Arrange
            using var pool = new FrameMemoryPool(_width, _height, _stride, _format, _capacity);
            var frame = pool.Rent();
            var countAfterRent = pool.AvailableCount;

            // Act
            pool.Return(frame);

            // Assert
            Assert.Equal(countAfterRent + 1, pool.AvailableCount);
            Assert.True(frame.IsReturned);
            Assert.Equal(1, pool.TotalReturned);
        }

        [Fact]
        public void Return_PreventsDoubleReturning_WhenCalledTwice()
        {
            // Arrange
            using var pool = new FrameMemoryPool(_width, _height, _stride, _format, _capacity);
            var frame = pool.Rent();
            pool.Return(frame);
            var countAfterFirstReturn = pool.AvailableCount;

            // Act
            pool.Return(frame); // Second return should be ignored

            // Assert
            Assert.Equal(countAfterFirstReturn, pool.AvailableCount);
            Assert.Equal(1, pool.TotalReturned); // Should not increment again
        }

        [Fact]
        public void Rent_WhenPoolEmpty_ShouldCreateNewFrameDynamically()
        {
            // Arrange
            using var pool = new FrameMemoryPool(_width, _height, _stride, _format, 1);
            var firstFrame = pool.Rent();
            Assert.Equal(0, pool.AvailableCount);

            // Act
            var secondFrame = pool.Rent();

            // Assert
            Assert.NotNull(secondFrame);
            Assert.Equal(0, pool.AvailableCount); // Pool remains empty
            Assert.Equal(2, pool.TotalRented);
        }

        [Fact]
        public void RentAndReturn_ShouldAllowFrameReuse()
        {
            // Arrange
            using var pool = new FrameMemoryPool(_width, _height, _stride, _format, 1);
            var firstFrame = pool.Rent();

            // Act
            pool.Return(firstFrame);
            var reusedFrame = pool.Rent();

            // Assert
            Assert.Same(firstFrame, reusedFrame); // Should be the same instance
            Assert.Equal(0, pool.AvailableCount);
            Assert.Equal(2, pool.TotalRented);
            Assert.Equal(1, pool.TotalReturned);
        }

        [Fact]
        public void Clear_ShouldRemoveAllFramesFromPool()
        {
            // Arrange
            using var pool = new FrameMemoryPool(_width, _height, _stride, _format, _capacity);
            var frame = pool.Rent();
            pool.Return(frame);
            Assert.Equal(_capacity, pool.AvailableCount);

            // Act
            pool.Clear();

            // Assert
            Assert.Equal(0, pool.AvailableCount);
        }

        [Fact]
        public void Dispose_ShouldClearPoolAndPreventFurtherOperations()
        {
            // Arrange
            var pool = new FrameMemoryPool(_width, _height, _stride, _format, _capacity);
            pool.Dispose();

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => pool.Rent());
        }

        [Fact]
        public void Validate_ShouldThrow_WhenFrameIsReturned()
        {
            // Arrange
            using var pool = new FrameMemoryPool(_width, _height, _stride, _format, _capacity);
            var frame = pool.Rent();
            pool.Return(frame);

            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => frame.Validate());
        }

        [Fact]
        public void ResetMetadata_ShouldResetFrameStateForReuse()
        {
            // Arrange
            using var pool = new FrameMemoryPool(_width, _height, _stride, _format, _capacity);
            var frame = pool.Rent();
            pool.Return(frame);

            // Act
            frame.ResetMetadata(1920, 1080, 1920, PixelFormat.Bgr24);

            // Assert
            Assert.Equal(1920, frame.Width);
            Assert.Equal(1080, frame.Height);
            Assert.Equal(1920, frame.Stride);
            Assert.Equal(PixelFormat.Bgr24, frame.Format);
            Assert.False(frame.IsReturned); // Should be reset to false
        }

        public void Dispose()
        {
            // Cleanup if needed
        }
    }

    /// <summary>
    /// TDD 測試套件：ManagedFrame 工業級受控記憶體描述符
    /// </summary>
    public class ManagedFrameTests : IDisposable
    {
        private readonly int _width = 2448;
        private readonly int _height = 2048;
        private readonly int _stride;
        private readonly byte[] _payload;

        public ManagedFrameTests()
        {
            _stride = _width;
            _payload = new byte[_width * _height];
        }

        [Fact]
        public void Constructor_ShouldInitializeWithCorrectMetadata()
        {
            // Act
            var frame = new ManagedFrame(_width, _height, _stride, PixelFormat.Mono8, _payload);

            // Assert
            Assert.Equal(_width, frame.Width);
            Assert.Equal(_height, frame.Height);
            Assert.Equal(_stride, frame.Stride);
            Assert.Equal(PixelFormat.Mono8, frame.Format);
            Assert.Same(_payload, frame.Payload);
            Assert.False(frame.IsReturned);
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenPayloadIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new ManagedFrame(_width, _height, _stride, PixelFormat.Mono8, null!));
        }

        [Fact]
        public void BufferSize_ShouldCalculateCorrectly()
        {
            // Arrange
            var frame = new ManagedFrame(_width, _height, _stride, PixelFormat.Mono8, _payload);
            var expectedSize = _stride * _height;

            // Act & Assert
            Assert.Equal(expectedSize, frame.BufferSize);
        }

        [Fact]
        public void Validate_ShouldPass_WhenFrameIsValid()
        {
            // Arrange
            var frame = new ManagedFrame(_width, _height, _stride, PixelFormat.Mono8, _payload);

            // Act & Assert (should not throw)
            frame.Validate();
        }

        [Fact]
        public void Validate_ShouldThrow_WhenPayloadIsTooSmall()
        {
            // Arrange
            var smallPayload = new byte[100]; // Too small
            var frame = new ManagedFrame(_width, _height, _stride, PixelFormat.Mono8, smallPayload);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => frame.Validate());
        }

        [Fact]
        public void MarkAsReturned_ShouldSetIsReturnedToTrue()
        {
            // Arrange
            var frame = new ManagedFrame(_width, _height, _stride, PixelFormat.Mono8, _payload);

            // Act
            frame.MarkAsReturned();

            // Assert
            Assert.True(frame.IsReturned);
        }

        [Fact]
        public void Dispose_ShouldMarkFrameAsReturned()
        {
            // Arrange
            var frame = new ManagedFrame(_width, _height, _stride, PixelFormat.Mono8, _payload);

            // Act
            frame.Dispose();

            // Assert
            Assert.True(frame.IsReturned);
        }

        [Fact]
        public void Dispose_CanBeCalledMultipleTimes_Safely()
        {
            // Arrange
            var frame = new ManagedFrame(_width, _height, _stride, PixelFormat.Mono8, _payload);

            // Act
            frame.Dispose();
            frame.Dispose(); // Should not throw

            // Assert
            Assert.True(frame.IsReturned);
        }

        public void Dispose()
        {
            // Cleanup if needed
        }
    }
}
