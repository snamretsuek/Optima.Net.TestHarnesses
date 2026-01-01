
# Optima.Net.TestHarnesses

## Purpose


The **Optima.Net.TestHarnesses** repository exists to demonstrate how **Optima.Net** and related **Optima.Net.*** packages can be used in real-world scenarios, end to end, without reducing the problems to toy examples or abstract theory.

Not all Optima.Net packages will have a test harness by default.  
Test harnesses are added when there is sufficient demand or when a concrete scenario meaningfully illustrates how a package is intended to be used.

This repository is **not a framework**, **not a reference architecture**, and **not production guidance**.  
It is a **working demonstration** whose sole purpose is to show:

> _What you must build and wire together for the capabilities provided by the Optima.Net.* packages to function correctly._

Nothing more. Nothing less.

Additional test harnesses may be added over time.  
When they are, they will be documented under their own sections, for example:
**What the xxxxxx Test Harness Demonstrates**

----------

## What the NegotiatR Test Harness Demonstrates

The primary scenario covered by the test harness is **Credential Creation**.

Specifically:

-   A user attempts to create **password-based credentials**
    
-   Domain policies are evaluated
    
-   If credential creation fails with _replaceable_ failures
    
-   **NegotiatR proposes biometrics** as an alternative credential method
    

This mirrors a real-world flow where:

-   password rules are strict
    
-   users fail password requirements
    
-   the system suggests a safer or more convenient alternative
    

The harness shows this flow clearly and deterministically.

----------

## Why So Many Domain Objects Exist

To run this scenario, the harness deliberately requires you to define:

-   **Specifications**  
    (What evidence is checked)
    
-   **Policies**  
    (Which rules apply)
    
-   **Policy Justifications**  
    (Why those rules exist and how they are enforced)
    
-   **Contexts**  
    (What the domain needs to reason correctly)
    
-   **Attempts**  
    (What the user actually tried)
    
-   **Proposals**  
    (What intent is being evaluated or negotiated)
    
-   **NegotiatR Rules**  
    (What alternative intent may be suggested)
    

Many of these objects will only ever be created **once per solution** in a real system.

That is intentional.

NegotiatR is designed to make **domain reasoning explicit**, not hidden inside procedural logic.  
The harness reflects that reality rather than hiding it behind shortcuts.

----------

## What the Test Harness Is _Not_

The test harness is **not**:

-   a clean architecture reference implementation
    
-   a reusable library
    
-   a unit test suite
    
-   a production-ready application
    

It **does not enforce** Clean Architecture rules.

It **does not care** if layers are slightly blurred.

It **does not attempt** to be elegant.

Its job is to be **clear**, not pretty.

----------

## Why a Console Application

Each harness is implemented as a **console application** by design.

This choice is deliberate because console apps:

-   are trivial to run
    
-   are easy to debug step-by-step
    
-   make object graphs visible
    
-   allow you to inspect decisions in real time
    
-   avoid framework noise (ASP.NET, DI containers, middleware, etc.)
    

The goal is **understanding**, not deployment.

----------

## Event Simulation

The harness simulates **integration events** to show what would happen _after_ a decision is made.

These events:

-   are emitted by the application layer
    
-   represent outcomes, not domain facts
    
-   are printed to the console for visibility
    

This allows you to see:

-   what decision was reached
    
-   what proposal was accepted or counter-proposed
    
-   what failures influenced the outcome
    

The harness does **not** attempt to model real messaging infrastructure.

----------

## No Unit Tests (By Design)

This repository will **never** contain unit tests.

That is not an omission—it is a boundary.

Unit tests belong in:

-   your application
    
-   your domain
    
-   your production solution
    

The test harness exists to show:

-   **how to wire things together**
    
-   **how NegotiatR behaves**
    
-   **what inputs and outputs look like**
    

Testing correctness is the responsibility of the implementer.

----------

## Intended Audience

This repository is for developers who:

-   want to understand NegotiatR by running it
    
-   prefer concrete examples over abstract explanations
    
-   want to see the full shape of the required domain model
    
-   are evaluating whether NegotiatR fits their problem space
    

If you are looking for:

-   minimal examples
    
-   magic defaults
    
-   implicit behavior
    

This is not the right place.

----------

## Final Note

The test harness is intentionally honest.

It shows:

-   the amount of modeling required
    
-   the explicitness NegotiatR demands
    
-   the clarity you gain in return
    

If this feels like “a lot of code,” that’s because **the problem itself is non-trivial**.

NegotiatR does not reduce complexity.  
It **makes it visible and controllable**.