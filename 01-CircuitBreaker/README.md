# Circuit Breaker Pattern

## ?? Theoretical Explanation

The **Circuit Breaker** pattern is a design pattern used to prevent an application from making repeated requests to a service that is likely to fail. It works like an electrical circuit breaker in your home—when too many electrical faults occur, the breaker trips (opens) and stops electricity flow to prevent damage. Similarly, the pattern monitors for failures and "opens" to prevent cascading failures.

### The Three States-

1. **CLOSED** (Normal Operation)
   - Requests flow through to the external service
   - The circuit breaker counts failures
   - If failures are within acceptable threshold, state remains CLOSED

2. **OPEN** (Service Unreachable)
   - The failure threshold has been exceeded
   - Requests are immediately rejected without calling the service
   - No attempts are made to reach the failing service
   - Prevents wasting time and resources on guaranteed failures

3. **HALF-OPEN** (Recovery Testing)
   - After a timeout period, the circuit transitions to HALF-OPEN
   - A limited number of test requests are allowed through
   - If these succeed, the service is assumed recovered ? transition to CLOSED
   - If these fail, the circuit goes back to OPEN

### Key Benefits

- **Fail Fast**: Stop trying to call a dead service immediately
- **Reduce Resource Waste**: Don't waste CPU, memory, and network bandwidth on failing calls
- **Prevent Cascading Failures**: One failing microservice doesn't bring down the entire system
- **Graceful Degradation**: Return cached data, defaults, or user-friendly errors instead of timeouts
- **Self-Healing**: Automatically recover when the service is back online

---

## ?? Real-World Enterprise Scenarios

### 1. **Payment Gateway Integration**
When calling an external payment provider (Stripe, PayPal), if it experiences an outage, you don't want to queue up thousands of failed requests. Circuit Breaker immediately stops attempts, lets the gateway recover, and prevents customer frustration with repeated timeouts. You can show a message like "Payment service temporarily unavailable" instead of hanging.

### 2. **Third-Party Weather API**
Your application displays weather on a dashboard. If the weather API goes down, you don't want every weather widget request to hang for 30 seconds waiting for a timeout. Circuit Breaker stops calling the API after a few failures, and you can display the last known weather data from cache instead.

### 3. **Database Connection Pooling**
Your application connects to a database that temporarily becomes overloaded. If all connection attempts fail, the circuit breaker stops attempting to acquire new connections momentarily, allowing the database to catch up. This prevents connection pool exhaustion and the entire application freezing.

### 4. **Microservice-to-Microservice Communication**
In a microservices architecture, Service A depends on Service B. If Service B is redeploying and unreachable, Circuit Breaker immediately stops Service A from making requests instead of causing Service A's threads to pile up waiting for timeouts.

### 5. **Authentication Service Dependency**
Many services depend on a central authentication service. If that service crashes, Circuit Breaker prevents all services from being blocked. Services can use cached authentication tokens or allow requests through temporarily while authentication service recovers.

### 6. **Cache Provider (Redis) Failure**
Your application uses Redis for caching. If Redis becomes unavailable, Circuit Breaker stops attempting to connect to it after a few failed attempts. Your application can immediately fall back to querying the database directly instead of wasting time on guaranteed Redis connection failures.

### 7. **Email Service in E-Commerce Platform**
When customers place orders, you send confirmation emails via an external email service. If that service is down, Circuit Breaker stops calling it, allows you to queue the email for retry later, and lets the order complete without hanging the checkout flow.

### 8. **Real-Time Analytics Event Collection**
Your application sends analytics events to an external service. If that service degrades, Circuit Breaker stops sending events (after initial failures), preventing the analytics calls from slowing down user-facing operations.

### 9. **Cloud Storage Upload to Blob Storage**
When uploading files to Azure Blob Storage, if the service is throttled or temporarily unavailable, Circuit Breaker stops hammering the storage account after a few failures. Your application queues the upload for later retry instead of exhausting bandwidth and time.

### 10. **Search Engine (Elasticsearch) Queries**
Your application relies on Elasticsearch for search functionality. If Elasticsearch becomes unavailable (during maintenance or failure), Circuit Breaker prevents hundreds of search requests from timing out. Instead, you can show a "Search temporarily unavailable" message or return empty results from cache.

### 11. **Inventory Service in Order Management**
When processing orders, you check inventory with a dedicated microservice. If that service has a bug or is redeploying, Circuit Breaker prevents the order service from being blocked. You can use last-known inventory levels or allow limited orders while inventory service recovers.

### 12. **Rate-Limited Third-Party API**
When calling an expensive third-party API with strict rate limits, Circuit Breaker can be configured to detect rate-limit responses and open the circuit proactively, preventing further requests until the rate limit window resets. This preserves your API quota.

---

## ?? When to Use This Pattern

? **Use Circuit Breaker when:**
- Your application calls external services that might fail or become slow
- Failures are usually temporary and self-healing (service restarts, network recovers)
- You want to prevent cascading failures in a microservices architecture
- You need to fail fast instead of waiting for timeout
- You want to give failing services time to recover without constantly pounding them

? **Don't use Circuit Breaker for:**
- Local function calls (no need to circuit-break internal methods)
- Permanent, non-recoverable errors (authorization failures, validation errors)
- Services that are always reliable with no history of failures (no point)

---

## ?? Flow Diagram

```
Request Arrives
    ?
Is Circuit OPEN?
    ?? YES ? Return Error/Cached Response Immediately (FAIL FAST)
    ?? NO ? Try to Call Service
             ?
          Call Succeeds?
             ?? YES ? Reset failure count, return response
             ?? NO ? Increment failure count
                    ?
                   Failures Exceed Threshold?
                      ?? YES ? TRIP CIRCUIT (Open), return error
                      ?? NO ? Keep trying

Is Circuit HALF-OPEN?
    ?? YES ? Allow limited test requests
    ?         ?? Success ? Close circuit
    ?         ?? Fail ? Re-open circuit
    ?? NO ? Continue
```

---

## ?? Configuration Parameters

- **Failure Threshold**: Number of failures before opening (e.g., 5 failures)
- **Timeout Duration**: How long to stay open before transitioning to half-open (e.g., 30 seconds)
- **Half-Open Request Count**: How many test requests to allow in half-open state (e.g., 3 requests)

---

## ?? Related Patterns

- **Retry**: Often works alongside Circuit Breaker—Retry handles transient failures, Circuit Breaker prevents repeated attempts on persistent failures
- **Bulkhead**: Isolates failures to specific components to prevent system-wide impact
- **Fallback**: Provides alternate behavior when service is unavailable
