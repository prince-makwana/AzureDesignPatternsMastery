using CircuitBreakerPattern.Models;
using CircuitBreakerPattern.Services.Implementations;
using CircuitBreakerPattern.Services.Interfaces;

// ============================================================================
// CIRCUIT BREAKER PATTERN DEMONSTRATION
// ============================================================================
// This program demonstrates how Circuit Breaker prevents cascading failures
// by stopping requests to a failing service and allowing it to recover.
// ============================================================================

Console.WriteLine("??????????????????????????????????????????????????????????????????");
Console.WriteLine("?           CIRCUIT BREAKER PATTERN DEMONSTRATION                ?");
Console.WriteLine("?  Preventing Cascading Failures Through Smart Request Handling  ?");
Console.WriteLine("??????????????????????????????????????????????????????????????????\n");

// Create a simulated payment gateway that will fail for the first 5 calls
IExternalService paymentGateway = new SimulatedPaymentGateway(failureUpTo: 5, serviceName: "PaymentGateway");

// Configure the circuit breaker
var config = new CircuitBreakerConfig
{
    FailureThreshold = 3,           // Open circuit after 3 failures
    Timeout = TimeSpan.FromSeconds(2),  // Stay open for 2 seconds before half-open
    HalfOpenTestRequests = 2        // Allow 2 test requests in half-open state
};

// Create circuit breaker instance
var circuitBreaker = new SimpleCircuitBreaker<string>(config);

Console.WriteLine($"Configuration:");
Console.WriteLine($"  • Failure Threshold: {config.FailureThreshold} failures");
Console.WriteLine($"  • Open Timeout: {config.Timeout.TotalSeconds} seconds");
Console.WriteLine($"  • Half-Open Test Requests: {config.HalfOpenTestRequests}\n");

Console.WriteLine("???????????????????????????????????????????????????????????????\n");

// ============================================================================
// SCENARIO 1: INITIAL FAILURES - CIRCUIT OPENS
// ============================================================================
Console.WriteLine("?? SCENARIO 1: Initial Failures - Circuit Opens");
Console.WriteLine("   Simulating payment gateway failures...\n");

for (int i = 1; i <= 6; i++)
{
    Console.WriteLine($"Request #{i}:");

    var result = await circuitBreaker.ExecuteAsync(async () =>
        await paymentGateway.CallServiceAsync()
    );

    if (result.IsSuccess)
    {
        Console.WriteLine($"  ? SUCCESS: {result.Data}");
    }
    else
    {
        Console.WriteLine($"  ? FAILED: {result.ErrorMessage}");
    }

    Console.WriteLine($"  Current State: {result.CurrentState}\n");

    // Give a small delay between requests
    await Task.Delay(200);
}

var stats = circuitBreaker.GetStats();
Console.WriteLine($"Circuit Breaker Stats:");
Console.WriteLine($"  • Current State: {stats.State}");
Console.WriteLine($"  • Failures: {stats.FailureCount}");
Console.WriteLine($"  • Successes: {stats.SuccessCount}");
Console.WriteLine($"  • Last Failure: {stats.LastFailureTime}\n");

Console.WriteLine("???????????????????????????????????????????????????????????????\n");

// ============================================================================
// SCENARIO 2: CIRCUIT OPEN - FAST FAIL (Prevent Cascading Failures)
// ============================================================================
Console.WriteLine("?? SCENARIO 2: Circuit Open - Requests Fail Immediately (Fast Fail)");
Console.WriteLine("   Demonstrating how Circuit Breaker prevents wasting resources...\n");

for (int i = 1; i <= 3; i++)
{
    Console.WriteLine($"Request #{i + 6}:");

    // These requests will fail immediately without hitting the service
    var result = await circuitBreaker.ExecuteAsync(async () =>
        await paymentGateway.CallServiceAsync()
    );

    if (result.IsSuccess)
    {
        Console.WriteLine($"  ? SUCCESS: {result.Data}");
    }
    else
    {
        Console.WriteLine($"  ? FAILED (Circuit Open): {result.ErrorMessage}");
    }

    Console.WriteLine($"  Current State: {result.CurrentState}\n");

    await Task.Delay(200);
}

Console.WriteLine("? Notice: Requests failed INSTANTLY without attempting to call the service!");
Console.WriteLine("  This is the Circuit Breaker in OPEN state - preventing cascading failures.\n");

Console.WriteLine("???????????????????????????????????????????????????????????????\n");

// ============================================================================
// SCENARIO 3: WAIT FOR TIMEOUT - TRANSITION TO HALF-OPEN
// ============================================================================
Console.WriteLine("?? SCENARIO 3: Waiting for Timeout - Transitioning to Half-Open State");
Console.WriteLine($"   Waiting {config.Timeout.TotalSeconds} seconds for circuit to test recovery...\n");

await Task.Delay((int)config.Timeout.TotalMilliseconds + 500);

Console.WriteLine("Time elapsed! Circuit should now be HALF-OPEN to test recovery...\n");

Console.WriteLine("???????????????????????????????????????????????????????????????\n");

// ============================================================================
// SCENARIO 4: HALF-OPEN STATE - SERVICE STILL FAILING
// ============================================================================
Console.WriteLine("?? SCENARIO 4: Half-Open State - Testing Recovery (Service Still Failing)");
Console.WriteLine("   Attempting test requests to check if service recovered...\n");

for (int i = 1; i <= 2; i++)
{
    Console.WriteLine($"Test Request #{i}:");

    var result = await circuitBreaker.ExecuteAsync(async () =>
        await paymentGateway.CallServiceAsync()
    );

    if (result.IsSuccess)
    {
        Console.WriteLine($"  ? SUCCESS: {result.Data}");
    }
    else
    {
        Console.WriteLine($"  ? FAILED: {result.ErrorMessage}");
    }

    Console.WriteLine($"  Current State: {result.CurrentState}\n");

    await Task.Delay(200);
}

Console.WriteLine("???????????????????????????????????????????????????????????????\n");

// ============================================================================
// SCENARIO 5: SERVICE RECOVERS - CIRCUIT CLOSES
// ============================================================================
Console.WriteLine("?? SCENARIO 5: Service Recovers - Circuit Closes");
Console.WriteLine("   Simulating service recovery and resuming normal operation...\n");

// Reset the payment gateway to simulate recovery
((SimulatedPaymentGateway)paymentGateway).Reset();

Console.WriteLine("(Simulated service recovery - payment gateway reset)\n");

// Wait for another timeout and transition to half-open
await Task.Delay((int)config.Timeout.TotalMilliseconds + 500);

for (int i = 1; i <= 3; i++)
{
    Console.WriteLine($"Request #{i + 9}:");

    var result = await circuitBreaker.ExecuteAsync(async () =>
        await paymentGateway.CallServiceAsync()
    );

    if (result.IsSuccess)
    {
        Console.WriteLine($"  ? SUCCESS: {result.Data}");
    }
    else
    {
        Console.WriteLine($"  ? FAILED: {result.ErrorMessage}");
    }

    Console.WriteLine($"  Current State: {result.CurrentState}\n");

    await Task.Delay(200);
}

Console.WriteLine("???????????????????????????????????????????????????????????????\n");

// ============================================================================
// FINAL STATISTICS
// ============================================================================
stats = circuitBreaker.GetStats();

Console.WriteLine("?? FINAL CIRCUIT BREAKER STATISTICS:");
Console.WriteLine($"  • Current State: {stats.State}");
Console.WriteLine($"  • Total Failures: {stats.FailureCount}");
Console.WriteLine($"  • Total Successes: {stats.SuccessCount}");
Console.WriteLine($"  • Last Failure: {stats.LastFailureTime}");
Console.WriteLine($"  • Last Tripped: {stats.LastTrippedTime}\n");

Console.WriteLine("??????????????????????????????????????????????????????????????????");
Console.WriteLine("?                      KEY TAKEAWAYS                             ?");
Console.WriteLine("??????????????????????????????????????????????????????????????????");
Console.WriteLine("? 1. CLOSED State: Normal operation, requests go through        ?");
Console.WriteLine("? 2. OPEN State: Service failing, requests rejected immediately ?");
Console.WriteLine("? 3. HALF-OPEN: Testing recovery with limited requests          ?");
Console.WriteLine("? 4. Prevents cascading failures by failing fast                ?");
Console.WriteLine("? 5. Protects system resources from wasted attempts             ?");
Console.WriteLine("? 6. Automatically recovers when service is healthy             ?");
Console.WriteLine("??????????????????????????????????????????????????????????????????");
