using CircuitBreakerPattern.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitBreakerPattern.Services.Implementations
{
    public class SimulatedPaymentGateway: IExternalService
    {
        private int _callCount = 0;
        private readonly int _failureUpTo;
        private readonly string _serviceName;

        public SimulatedPaymentGateway(int failureUpTo = 5, string serviceName = "Simulated Payment Gateway")
        {
            _failureUpTo = failureUpTo;
            _serviceName = serviceName;
        }

        public async Task<string> CallServiceAsync()
        {
            _callCount++;

            //Simulate network latency
            await Task.Delay(100);

            if (_callCount <= _failureUpTo)
            {
                throw new HttpRequestException($"[{_serviceName}] is currently unavailable (call #{_callCount})");
            }

            return $"{_serviceName} processed payment successfully (call #{_callCount})";
        }
        
        /// <summary>
        /// Resets the call count to simulate service gateway
        /// </summary>
        public void Reset()
        {
            _callCount = 0;
        }

        /// <summary>
        /// Get the current count
        /// </summary>
        public int GetCallCount() => _callCount;
    }
}
