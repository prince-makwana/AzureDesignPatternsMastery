using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitBreakerPattern.Models
{
    /// <summary>
    /// Represents the state of the circuit breaker.
    /// </summary>
    public enum CircuitState
    {
        Closed, // Normal operation, requests are allowed to pass through.
        Open,  // Service is failing, requests are blocked to prevent further failures.
        HalfOpen // Testing if service recovered, allowing a limited number of requests to pass through.
    }
    
    /// <summary>
    /// Statistics about circuit breaker performance, such as failure counts and success counts.
    /// </summary>
    public class  CircuitBreakerStats
    {
        public CircuitState State { get; set; }
        public int FailureCount { get; set; }
        public int SuccessCount { get; set; }
        public DateTime? LastFailureTime { get; set; }
        public DateTime? LastTrippedTime { get; set; }
    }

    /// <summary>
    /// Configuration for the Circuit Breaker policy, including failure thresholds, timeout durations, and half-open test request counts.
    /// </summary>
    public class CircuitBreakerConfig
    {
        public int FailureThreshold { get; set; }
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
        public int HalfOpenTestRequests { get; set; } = 3;
    }

    /// <summary>
    /// Represents the result of a circuit breaker operation, including success status, data, error messages, and the current state of the circuit breaker.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CircuitBreakerResult<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public CircuitState CurrentState { get; set; }
    }
}
