using CircuitBreakerPattern.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitBreakerPattern.Services.Interfaces
{
    /// <summary>
    /// Defines the interface for a circuit breaker, which manages the state of service calls and 
    /// prevents cascading failures by controlling the flow of requests based on the health of the service.
    /// </summary>
    public interface ICircuitBreaker<T>
    {
        /// <summary>
        /// Gets the current state of the circuit breaker.
        /// </summary>
        /// <returns></returns>
        CircuitState GetState();

        /// <summary>
        /// Executes an operation thorugh the cicuit breaker, handling state transitions and failures according to the circuit breaker
        /// pattern. Returns a CircuitBreakerResult indicating the success or failure of the operation, along with any relevant data 
        /// or error messages.
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        Task<CircuitBreakerResult<T>> ExecuteAsync(Func<Task<T>> operation);

        /// <summary>
        /// Gets statistics about the circuit breaker
        /// </summary>
        /// <returns></returns>
        CircuitBreakerStats GetStats();
    }
}
