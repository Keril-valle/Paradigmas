## Purpose

Provides JWT-based authentication for the API so protected resources can only be accessed with a valid bearer token.

## ADDED Requirements

### Requirement: Login with credentials
The system SHALL expose `POST /login` accepting email and password, returning a JWT bearer token for valid credentials.

#### Scenario: Valid credentials
- **WHEN** a client posts valid credentials (`admin` / `1234`) to `POST /login`
- **THEN** the system responds `200 OK` with a JSON body containing a `token`

#### Scenario: Invalid credentials
- **WHEN** a client posts unknown credentials to `POST /login`
- **THEN** the system responds `401 Unauthorized`

### Requirement: Protect book endpoints
The system SHALL reject requests to book endpoints that do not carry a valid, unexpired JWT bearer token.

#### Scenario: No token
- **WHEN** a client requests a protected book endpoint without a bearer token
- **THEN** the system responds `401 Unauthorized`

#### Scenario: Invalid or expired token
- **WHEN** a client requests a protected book endpoint with a malformed or expired bearer token
- **THEN** the system responds `401 Unauthorized`
