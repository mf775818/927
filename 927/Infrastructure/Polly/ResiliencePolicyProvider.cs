using System;
using Polly;
using Polly.Retry;
using Polly.CircuitBreaker;
using ShoeMoldControl.Core;

namespace ShoeMoldControl.Infrastructure.Polly
{
    public interface IResiliencePolicyProvider
    {
        AsyncRetryPolicy<DecodeResult> VisionRetryPolicy { get; }
        AsyncCircuitBreakerPolicy<DecodeResult> VisionCircuitBreaker { get; }
        AsyncRetryPolicy<int> RobotCommandRetryPolicy { get; }
        AsyncCircuitBreakerPolicy<int> RobotCommandCircuitBreaker { get; }
    }

    public class ResiliencePolicyProvider : IResiliencePolicyProvider
    {
        private readonly AsyncRetryPolicy<DecodeResult> _visionRetryPolicy;
        private readonly AsyncCircuitBreakerPolicy<DecodeResult> _visionCircuitBreaker;
        private readonly AsyncRetryPolicy<int> _robotCommandRetryPolicy;
        private readonly AsyncCircuitBreakerPolicy<int> _robotCommandCircuitBreaker;

        public AsyncRetryPolicy<DecodeResult> VisionRetryPolicy => _visionRetryPolicy;
        public AsyncCircuitBreakerPolicy<DecodeResult> VisionCircuitBreaker => _visionCircuitBreaker;
        public AsyncRetryPolicy<int> RobotCommandRetryPolicy => _robotCommandRetryPolicy;
        public AsyncCircuitBreakerPolicy<int> RobotCommandCircuitBreaker => _robotCommandCircuitBreaker;

        public ResiliencePolicyProvider()
        {
            // Vision retry: 3 attempts, exponential backoff (1s, 2s, 4s)
            _visionRetryPolicy = Policy<DecodeResult>
                .HandleResult(r => !r.IsSuccess)
                .Or<TimeoutException>()
                .Or<System.Net.Sockets.SocketException>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt - 1)),
                    onRetry: (outcome, timespan, retryNumber, context) =>
                    {
                        var logger = context.ContainsKey("Logger") ? context["Logger"] as Serilog.ILogger : null;
                        logger?.Warning("Vision operation failed. Retry {RetryNumber} of {MaxRetries} after {Delay}ms. Error: {Error}", 
                            retryNumber, 3, timespan.TotalMilliseconds, outcome.Result?.ErrorMessage ?? outcome.Exception?.Message);
                    }
                );

            // Vision circuit breaker: 5 failures in 30 seconds opens for 60 seconds
            _visionCircuitBreaker = Policy<DecodeResult>
                .HandleResult(r => !r.IsSuccess)
                .Or<TimeoutException>()
                .Or<System.Net.Sockets.SocketException>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(60),
                    onBreak: (outcome, breakDelay) =>
                    {
                        Serilog.Log.Warning("Vision circuit breaker OPEN for {BreakDelay}ms due to {Error}", 
                            breakDelay.TotalMilliseconds, outcome.Result?.ErrorMessage ?? outcome.Exception?.Message);
                    },
                    onReset: () => Serilog.Log.Information("Vision circuit breaker CLOSED - service recovered"),
                    onHalfOpen: () => Serilog.Log.Information("Vision circuit breaker HALF-OPEN - testing recovery")
                );

            // Robot command retry: 3 attempts with fixed delay
            _robotCommandRetryPolicy = Policy<int>
                .HandleResult(id => id < 0)
                .Or<TimeoutException>()
                .Or<System.Net.Sockets.SocketException>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(500 * attempt),
                    onRetry: (outcome, timespan, retryNumber, context) =>
                    {
                        var logger = context.ContainsKey("Logger") ? context["Logger"] as Serilog.ILogger : null;
                        logger?.Warning("Robot command failed. Retry {RetryNumber} of {MaxRetries}. Error: {Error}", 
                            retryNumber, 3, outcome.Exception?.Message ?? "Invalid command ID");
                    }
                );

            // Robot circuit breaker: 4 failures in 20 seconds opens for 45 seconds
            _robotCommandCircuitBreaker = Policy<int>
                .HandleResult(id => id < 0)
                .Or<TimeoutException>()
                .Or<System.Net.Sockets.SocketException>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 4,
                    durationOfBreak: TimeSpan.FromSeconds(45),
                    onBreak: (outcome, breakDelay) =>
                    {
                        Serilog.Log.Warning("Robot circuit breaker OPEN for {BreakDelay}ms", breakDelay.TotalMilliseconds);
                    },
                    onReset: () => Serilog.Log.Information("Robot circuit breaker CLOSED - service recovered"),
                    onHalfOpen: () => Serilog.Log.Information("Robot circuit breaker HALF-OPEN - testing recovery")
                );
        }
    }
}
