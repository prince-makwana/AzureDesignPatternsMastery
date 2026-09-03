using System;
using System.Collections.Generic;
using System.Text;

namespace CircuitBreakerPattern.Services.Interfaces
{
    public interface IExternalService
    {
        /// <summary>
        /// Simulates calling an external service that might fail
        /// </summary>
        /// <returns></returns>
        Task<string> CallServiceAsync();
    }
}
