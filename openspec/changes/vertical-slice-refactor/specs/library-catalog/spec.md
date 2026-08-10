## Purpose

Defines the public HTTP API for managing libraries and their books — create, read, update, delete, and the error semantics for missing resources.

## ADDED Requirements

### Requirement: List libraries
The system SHALL expose `GET /api/libraries` returning all stored libraries.

#### Scenario: List libraries
- **WHEN** a client requests `GET /api/libraries`
- **THEN** the system responds `200 OK` with a JSON array of libraries

### Requirement: Get a library by id
The system SHALL expose `GET /api/libraries/{libraryId}` returning a single library.

#### Scenario: Library exists
- **WHEN** a client requests `GET /api/libraries/1` and a library with id 1 exists
- **THEN** the system responds `200 OK` with the library

#### Scenario: Library does not exist
- **WHEN** a client requests `GET /api/libraries/999` and no library has id 999
- **THEN** the system responds `404 Not Found`

### Requirement: Create a library
The system SHALL expose `POST /api/libraries` to create a library from the request body.

#### Scenario: Create library
- **WHEN** a client posts a valid library payload to `POST /api/libraries`
- **THEN** the system persists the library and responds `200 OK` with the created library

### Requirement: Update a library
The system SHALL expose `PUT /api/libraries/{libraryId}` to update an existing library.

#### Scenario: Library exists
- **WHEN** a client puts a library payload to `PUT /api/libraries/1` and the library exists
- **THEN** the system updates the library and responds `204 No Content`

#### Scenario: Library does not exist
- **WHEN** a client puts a library payload to `PUT /api/libraries/999` and no library has id 999
- **THEN** the system responds `404 Not Found`

### Requirement: Delete a library
The system SHALL expose `DELETE /api/libraries/{libraryId}` to delete a library and its books.

#### Scenario: Library exists
- **WHEN** a client requests `DELETE /api/libraries/1` and the library exists
- **THEN** the system deletes the library and responds `204 No Content`, and subsequent requests for that library respond `404 Not Found`

#### Scenario: Library does not exist
- **WHEN** a client requests `DELETE /api/libraries/999` and no library has id 999
- **THEN** the system responds `404 Not Found`

### Requirement: List books in a library
The system SHALL expose `GET /api/libraries/{libraryId}/books` returning the books that belong to the library. This endpoint SHALL require a valid bearer token.

#### Scenario: Library exists
- **WHEN** an authenticated client requests `GET /api/libraries/1/books` and the library exists
- **THEN** the system responds `200 OK` with a JSON array of books belonging to library 1

#### Scenario: Library does not exist
- **WHEN** an authenticated client requests `GET /api/libraries/31232/books` and no library has id 31232
- **THEN** the system responds `404 Not Found`

#### Scenario: No token
- **WHEN** a client requests `GET /api/libraries/1/books` without a bearer token
- **THEN** the system responds `401 Unauthorized`

### Requirement: Add a book to a library
The system SHALL expose `POST /api/libraries/{libraryId}/books` to add a book to a library. This endpoint SHALL require a valid bearer token.

#### Scenario: Library exists
- **WHEN** an authenticated client posts a book payload to `POST /api/libraries/1/books` and the library exists
- **THEN** the system persists the book and responds `201 Created` with the created book

#### Scenario: Library does not exist
- **WHEN** an authenticated client posts a book payload to `POST /api/libraries/100/books` and no library has id 100
- **THEN** the system responds `404 Not Found`
