## Purpose

Defines the HTTP contract for the library management service: creating, reading, updating, and deleting libraries, and listing and adding the books that belong to a library, including the required response codes and JWT-based authorization.

## Requirements

### Requirement: List all libraries
The system SHALL return all libraries in the system when requested.
The system MUST respond with HTTP 200 and a JSON array of libraries on success.

#### Scenario: Successful list of all libraries
- **WHEN** a client issues a GET request to `/api/libraries`
- **THEN** the system responds with HTTP 200 and a JSON array containing all libraries

### Requirement: Retrieve a single library
The system SHALL return a single library identified by its id, or indicate that it does not exist.
- **WHEN** a client issues a GET request to `/api/libraries/{id}` and a library with that id exists
- **THEN** the system responds with HTTP 200 and the library representation
- **WHEN** a client issues a GET request to `/api/libraries/{id}` for a library id that does not exist
- **THEN** the system responds with HTTP 404 Not Found

#### Scenario: Get an existing library
- **WHEN** a client issues a GET request to `/api/libraries/{id}` for an existing library id
- **THEN** the system responds with HTTP 200 and the library representation

#### Scenario: Get a missing library
- **WHEN** a client issues a GET request to `/api/libraries/{id}` for a library id that does not exist
- **THEN** the system responds with HTTP 404 Not Found

### Requirement: Add a library
The system SHALL create a new library from a client-supplied representation and return it.

#### Scenario: Create a library
- **WHEN** a client issues a POST request to `/api/libraries` with a valid library representation
- **THEN** the system persists the new library and responds with HTTP 200 and the created library representation

### Requirement: Update a library
The system SHALL update an existing library by its id using a client-supplied representation.

#### Scenario: Update an existing library
- **WHEN** a client issues a PUT request to `/api/libraries/{id}` with a valid library representation and a library with that id exists
- **THEN** the system applies the update and responds with HTTP 204 No Content

#### Scenario: Update a missing library
- **WHEN** a client issues a PUT request to `/api/libraries/{id}` for a library id that does not exist
- **THEN** the system responds with HTTP 404 Not Found

### Requirement: Delete a library
The system SHALL delete a library by its id and remove its associated books.

#### Scenario: Delete an existing library
- **WHEN** a client issues a DELETE request to `/api/libraries/{id}` and a library with that id exists
- **THEN** the system deletes the library and responds with HTTP 204 No Content

#### Scenario: Delete a missing library
- **WHEN** a client issues a DELETE request to `/api/libraries/{id}` for a library id that does not exist
- **THEN** the system responds with HTTP 404 Not Found

#### Scenario: Delete library removes its books
- **WHEN** a client deletes a library that has books via a DELETE request to `/api/libraries/{id}`
- **THEN** the books that belonged to that library are also removed and a subsequent request to list that library's books responds with HTTP 404 Not Found

### Requirement: List books in a library
The system SHALL return all books belonging to a library when requested.

#### Scenario: Successful list of books in an existing library
- **WHEN** a client issues a GET request to `/api/libraries/{id}/books` and a library with that id exists
- **THEN** the system responds with HTTP 200 and a JSON array of the library's books

#### Scenario: List books for a missing library
- **WHEN** a client issues a GET request to `/api/libraries/{id}/books` for a library id that does not exist
- **THEN** the system responds with HTTP 404 Not Found

### Requirement: Add a book to a library
The system SHALL add a book to the specified library and return it with HTTP 201 on success.

#### Scenario: Add a book to an existing library
- **WHEN** a client issues a POST request to `/api/libraries/{libraryId}/books` with a valid book representation and a library with that id exists
- **THEN** the system persists the book linked to that library and responds with HTTP 201 Created

#### Scenario: Add a book to a missing library
- **WHEN** a client issues a POST request to `/api/libraries/{libraryId}/books` for a library id that does not exist
- **THEN** the system responds with HTTP 404 Not Found

### Requirement: Authentication endpoint
The system SHALL expose a login endpoint that returns a JWT token when valid credentials are supplied.

#### Scenario: Successful login
- **WHEN** a client issues a POST request to `/login` with valid credentials
- **THEN** the system responds with HTTP 200 and a token

#### Scenario: Unsuccessful login
- **WHEN** a client issues a POST request to `/login` with invalid credentials
- **THEN** the system responds with HTTP 401 Unauthorized