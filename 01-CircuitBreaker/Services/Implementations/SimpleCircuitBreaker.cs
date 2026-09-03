using CircuitBreakerPattern.Models;
using CircuitBreakerPattern.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitBreakerPattern.Services.Implementations
{
    public class SimpleCircuitBreaker<T> : ICircuitBreaker<T>
    {
        private CircuitState _state = CircuitState.Closed;
        private int _failureCount = 0;
        private int _successCount = 0;
        private DateTime? _lastFailureTime;
        private DateTime? _lastTrippedTime;
        private readonly object _lock = new();

        private readonly CircuitBreakerConfig _config;

        public SimpleCircuitBreaker(CircuitBreakerConfig config)
        {
            _config = config ?? new CircuitBreakerConfig();
        }

        public CircuitState GetState()
        {
            lock (_lock)
            {
                if (_state == CircuitState.Open && DateTime.UtcNow - _lastTrippedTime >= _config.Timeout)
                {
                    _state = CircuitState.HalfOpen;
                    _failureCount = 0; // Reset failed count when transitioning to HalfOpen
                    Console.WriteLine($" [Circuit Breaker] Transitioning to HALF-OPEN state to test recovery...");
                }
                return _state;
            }
        }

        public async Task<CircuitBreakerResult<T>> ExecuteAsync(Func<Task<T>> operation)
        {
            lock(_lock)
            {
                GetState(); // Check state transitions before executing the operation

                if (_state == CircuitState.Open)
                {
                    return new CircuitBreakerResult<T>
                    {
                        IsSuccess = false,
                        ErrorMessage = "Circuit Breaker is OPEN. Service is unavailable. Requests are rejected to prevent cascading failure.",
                        CurrentState = CircuitState.Open
                    };
                }
            }

            try
            {
                var result = await operation();
                lock(_lock)
                {
                    _failureCount = 0;
                    _successCount++;

                    if (_state == CircuitState.HalfOpen)
                    {
                        _state = CircuitState.Closed;
                        Console.WriteLine($" [Circuit Breaker] Success in HALF-OPEN state! Circuit CLOSED - service recovered.");
                    }
                }

                return new CircuitBreakerResult<T>
                {
                    IsSuccess = true,
                    Data = result,
                    CurrentState = GetState()
                };
            }
            catch (Exception ex)
            {
                lock(_lock)
                {
                    _failureCount++;
                    _lastFailureTime = DateTime.UtcNow;

                    Console.WriteLine($" [Circuit Breaker] Failure detected (Total failures: {_failureCount}/{_config.FailureThreshold})");

                    if (_failureCount >= _config.FailureThreshold)
                    {
                        _state = CircuitState.Open;
                        _lastTrippedTime = DateTime.UtcNow;
                        Console.WriteLine($" [Circuit Breaker] Failure threshold exceeded. Circuit OPENED - Rejecting future requests.");
                    }
                    else if(_state == CircuitState.HalfOpen)
                    {
                        _state = CircuitState.Open;
                        _lastTrippedTime = DateTime.UtcNow;
                        Console.WriteLine($" [Circuit Breaker] Test failed in HALF-OPEN state! Circuit re-OPENED.");
                    }
                }
                return new CircuitBreakerResult<T>
                {
                    IsSuccess = false,
                    ErrorMessage = $"Operation failed: {ex.Message}",
                    CurrentState = GetState()
                };
            }
        }

        public CircuitBreakerStats GetStats()
        {
            lock(_lock)
            {
                return new CircuitBreakerStats
                {
                    State = _state,
                    FailureCount = _failureCount,
                    SuccessCount = _successCount,
                    LastFailureTime = _lastFailureTime,
                    LastTrippedTime = _lastTrippedTime
                };
            }
        }
    }
}
