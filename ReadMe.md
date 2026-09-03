# Azure Design Patterns Mastery

A comprehensive repository implementing all 37 official Azure Cloud Design Patterns as per [Microsoft Learn - Cloud Design Patterns](https://learn.microsoft.com/en-us/azure/architecture/patterns/).

This repository serves as both a **theoretical learning guide** and a **practical implementation reference** for enterprise-grade cloud architecture patterns.

---

## ?? Complete Pattern Reference

### Batch 1: The Interview Heavyweights (Resiliency, Messaging & Data)

| # | Pattern | Use Case | Category |
|---|---------|----------|----------|
| 1 | **Circuit Breaker** | Prevent cascading failures when calling an unreliable external service. Stop hammering a failing service and let it recover. | Resiliency |
| 2 | **Retry** | Handle temporary, transient failures automatically. Useful for network timeouts, temporary service outages, or brief database locks. | Resiliency |
| 3 | **Cache-Aside** | Improve performance by checking cache first before hitting the database. If data isn't cached, fetch it, cache it, and return it. | Performance |
| 4 | **CQRS** | Separate read and write operations into different models. Optimize reads with denormalized views while keeping writes transactional. | Data Architecture |
| 5 | **Publisher-Subscriber** | Decouple event producers from event consumers. Multiple subscribers can react to the same event independently. | Messaging |
| 6 | **Event Sourcing** | Store the complete history of state changes as immutable events instead of just the current state. Enables full audit trails and temporal queries. | Data Architecture |

---

### Batch 2: Microservices & Modernization (Routing & Integration)

| # | Pattern | Use Case | Category |
|---|---------|----------|----------|
| 7 | **Anti-Corruption Layer** | Translate between your domain model and an external system's model. Protect your code from being contaminated by ugly legacy APIs. | Integration |
| 8 | **Strangler Fig** | Gradually replace a monolithic application by wrapping it and redirecting features to new microservices piece by piece. | Migration |
| 9 | **Backends for Frontends (BFF)** | Create separate backend services optimized for different client types (web, mobile, desktop). Each frontend gets its own tailored API. | Architecture |
| 10 | **Gateway Aggregation** | Combine multiple microservice calls into a single gateway endpoint. Reduces chattiness and improves client experience. | Integration |
| 11 | **Gateway Routing** | Route incoming requests to different microservices based on the URL path, hostname, or HTTP headers. | Integration |
| 12 | **Sidecar** | Deploy a companion service alongside your main application to handle cross-cutting concerns (logging, monitoring, networking). | Architecture |

---

### Batch 3: Scalability & Messaging Under Load

| # | Pattern | Use Case | Category |
|---|---------|----------|----------|
| 13 | **Competing Consumers** | Allow multiple consumer instances to process messages from the same queue in parallel, increasing throughput and reliability. | Scalability |
| 14 | **Queue-Based Load Leveling** | Buffer incoming requests in a queue and process them at a steady rate. Protects backend services from traffic spikes. | Scalability |
| 15 | **Rate Limiting / Throttling** | Limit the number of requests a client can make in a time period. Protects your service from being overwhelmed and ensures fair resource usage. | Resiliency |
| 16 | **Asynchronous Request-Reply** | Client sends a request and receives a callback or polls for results later, instead of waiting synchronously. | Messaging |
| 17 | **Claim-Check** | For large payloads, store the actual data externally and pass a reference (claim ticket) through the message pipeline. | Messaging |
| 18 | **Sharding** | Partition data horizontally across multiple databases so no single database becomes a bottleneck. | Scalability |

---

### Batch 4: Distributed Data & Advanced Transactions

| # | Pattern | Use Case | Category |
|---|---------|----------|----------|
| 19 | **Choreography** | Coordinate distributed transactions where each service subscribes to events and performs its own actions. No central orchestrator. | Distributed Transactions |
| 20 | **Compensating Transaction** | When a distributed transaction fails partway through, execute compensating actions to undo previous steps. | Distributed Transactions |
| 21 | **Materialized View** | Pre-compute and store the results of expensive queries (denormalized data) so reads are lightning fast. | Performance |
| 22 | **Index Table** | Create a secondary index structure to speed up queries on data attributes other than the primary key. | Performance |
| 23 | **Valet Key** | Generate short-lived, limited-scope credentials so clients can directly access cloud storage without exposing full credentials. | Security |

---

### Batch 5: Reliability, Health & Compute

| # | Pattern | Use Case | Category |
|---|---------|----------|----------|
| 24 | **Bulkhead** | Isolate critical resources so one failing component doesn't drag down the entire system. Like compartments in a ship. | Resiliency |
| 25 | **Geode** | Replicate data across multiple geographic regions for low-latency access and disaster recovery. | Scalability |
| 26 | **Health Endpoint Monitoring** | Expose a `/health` endpoint that checks if your service and its dependencies are healthy. Used by load balancers for routing decisions. | Reliability |
| 27 | **Ambassador** | Deploy a local proxy agent within the same container/VM as your application to handle communication with external services. | Architecture |
| 28 | **Leader Election** | Designate one instance among many to coordinate distributed work, preventing duplicate actions. | Distributed Systems |
| 29 | **Compute Resource Consolidation** | Combine multiple tasks or applications onto fewer compute instances to reduce costs and improve resource utilization. | Scalability |

---

### Batch 6: Security, Configuration & Execution

| # | Pattern | Use Case | Category |
|---|---------|----------|----------|
| 30 | **External Configuration Store** | Keep configuration outside your code (in Key Vault, Config Server, etc.) so you can change settings without redeploying. | Configuration |
| 31 | **Federated Identity** | Delegate authentication to an external identity provider. Users sign in once and access multiple services securely. | Security |
| 32 | **Gatekeeper** | Place a protective layer before sensitive services that validates and sanitizes all incoming requests. | Security |
| 33 | **Idempotent Consumer** | Design message consumers so processing the same message multiple times produces the same result as processing it once. | Messaging |
| 34 | **Deployment Stamps** | Deploy independent copies of your entire application across regions or availability zones for disaster recovery. | Resilience |
| 35 | **Pipes and Filters** | Chain independent processing components where output of one becomes input to the next. | Architecture |
| 36 | **Priority Queue** | Process high-priority messages before low-priority ones. Ensures critical work gets done first. | Messaging |
| 37 | **Gateway Offloading** | Move cross-cutting concerns (SSL termination, compression, authentication) to a gateway so services don't repeat this work. | Architecture |

---

## ?? Cheat Sheet & Memory Tricks

### When to Use Each Pattern

**Resiliency & Fault Tolerance:**
- ?? **Retry?** ? Ask: "Is this error temporary?" (network timeout, service briefly down) ? **YES** = Retry
- ?? **Circuit Breaker?** ? Ask: "Is the service DOWN and I keep failing?" ? **YES** = Circuit Breaker (avoid hammering)
- ??? **Bulkhead?** ? Ask: "Could one failure take down the WHOLE system?" ? **YES** = Bulkhead (isolate resources)

**Performance & Caching:**
- ?? **Cache-Aside?** ? Ask: "Is this data read frequently and expensive to fetch?" ? **YES** = Cache-Aside
- ?? **Materialized View?** ? Ask: "Do I have expensive joins/aggregations I run constantly?" ? **YES** = Pre-compute and cache

**Data & Architecture:**
- ?? **Event Sourcing?** ? Ask: "Do I need a complete audit trail?" or "Do I need to replay state?" ? **YES** = Event Sourcing
- ?? **CQRS?** ? Ask: "Are my read and write patterns VERY different?" ? **YES** = Separate them
- ?? **Sharding?** ? Ask: "Is my single database becoming a bottleneck?" ? **YES** = Shard the data

**Messaging & Integration:**
- ?? **Pub-Sub?** ? Ask: "Do I have one event many subscribers care about?" ? **YES** = Pub-Sub
- ?? **Queue-Based Load Leveling?** ? Ask: "Do traffic spikes overwhelm my backend?" ? **YES** = Queue requests
- ?? **Claim-Check?** ? Ask: "Are my messages HUGE?" ? **YES** = Store data externally, pass reference

**Microservices & API:**
- ?? **BFF?** ? Ask: "Do different clients need different APIs?" (web vs mobile) ? **YES** = BFF
- ?? **Gateway Routing?** ? Ask: "Do I route different URLs to different services?" ? **YES** = API Gateway
- ?? **Anti-Corruption Layer?** ? Ask: "Does that legacy API look ugly and I want to hide it?" ? **YES** = ACL wrapper

**Distributed Systems:**
- ?? **Choreography vs Orchestration?** ? "Do services talk to each other or to a coordinator?" ? Choreography = P2P, Orchestration = Coordinator
- ?? **Compensating Transaction?** ? "If step 3 fails, can I undo steps 1 & 2?" ? **YES** = Have compensating actions ready

**Security & Authentication:**
- ?? **Federated Identity?** ? Ask: "Should users sign in via external provider?" ? **YES** = Federated
- ?? **Valet Key?** ? Ask: "Should clients access storage directly without my creds?" ? **YES** = Generate temp access tokens

---

## ?? Project Structure

Each pattern has its own project folder containing:
- **README.md** - Detailed explanation and 10-12 real-world scenarios
- **Program.cs** - Practical console application demonstrating the pattern
- **Supporting Classes** - Interfaces, services, and models following SOLID principles

---

## ?? Getting Started

1. Start with **Batch 1** patterns (Circuit Breaker, Retry, Cache-Aside, CQRS, Pub-Sub, Event Sourcing)
2. Read each project's README.md for theoretical understanding
3. Run Program.cs to see the pattern in action
4. Review the supporting code to understand implementation details
5. Move to subsequent batches once comfortable with fundamentals

---

## ?? Learning Approach

- **Theory First**: Each README explains the "why" before the "what"
- **Practical Implementation**: Every pattern includes working C# code
- **Real-World Scenarios**: Each pattern documentation includes enterprise use cases
- **Interview Preparation**: Patterns are ordered by interview frequency and relevance

---

## ?? References

- [Microsoft Azure Architecture Patterns](https://learn.microsoft.com/en-us/azure/architecture/patterns/)
- [Cloud Design Patterns Book](https://www.microsoft.com/en-us/download/details.aspx?id=42038)

---

**Happy Learning! Master these patterns and you'll excel in system design interviews and build resilient cloud systems.** ??