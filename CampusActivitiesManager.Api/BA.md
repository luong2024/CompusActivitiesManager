1. General Information
Project: Campus Activity Manager (.NET MAUI & Backend Services)
User Story: US35 – Account Management API (Develop an account management API: create, update, lock/unlock accounts)
Task ID: T35.1 – Create Account + Update Account API
Estimated Time: 10 hours
Assignee: Nguyen Duc Manh
Architecture & Tech Stack:
Backend REST API integrated with Firebase (Firebase Authentication, Cloud Firestore / Realtime Database, Firebase Admin SDK)
Multi-layer pattern: Controller – Service – Repository / Dependency Injection
Service layer supporting data synchronization with .NET MAUI client applications

2. Business Objectives
Provide secure, standardized, and robust RESTful API endpoints enabling authorized users (e.g., Administrators/System Management) to create new accounts and update existing user profile details, ensuring seamless synchronization with Firebase Authentication and the application database.

3. Detailed Acceptance Criteria & Scenarios
AC1: The API successfully creates an account when input data is valid
Description: When a valid payload is supplied, the system creates the user identity in Firebase Authentication, stores user profile details in the database, and returns the newly generated account information.
Gherkin Scenario:
Gherkin
Scenario: Successfully create a new user account
  Given An authenticated administrator with valid administrative permissions
  When A POST request is sent to `/api/v1/accounts` with the following body:
    | email               | password      | fullName       | role    |
    | student1@campus.edu | SecureP@ss123 | Nguyen Van A   | Student |
  Then A new user record is created in Firebase Auth and the database
  And The API responds with HTTP Status Code 201 Created
  And The response body contains the new account ID, email, fullName, role, and createdAt timestamp

AC2: The API validates required fields when creating an account
Description: The system performs request validation before executing business logic. If required fields are missing, invalid, or duplicated, the request is rejected with explicit validation error messages.
Validation Rules:
email: Mandatory, RFC 5322 standard format, must be unique across the system.
password: Mandatory, minimum 8 characters, containing uppercase, lowercase, numeric, and special characters.
fullName: Mandatory, non-empty, and cannot consist solely of whitespace.
role: Mandatory, must match one of the predefined roles (Admin, Lecturer, Student).

Gherkin Scenario:
Gherkin
Scenario Outline: Fail account creation due to invalid input
  Given An account creation payload with invalid or missing <Field> value <Value>
  When A POST request is sent to `/api/v1/accounts`
  Then No new account is created
  And The API responds with HTTP Status Code <HTTPCode>
  And The response body contains the relevant error code and message for <Field>

  Examples:
    | Field    | Value            | HTTPCode | Message                             |
    | email    | ""               | 400      | "Email is required"                 |
    | email    | "invalid-format" | 400      | "Invalid email format"              |
    | email    | "exist@knu.ac.kr"| 409      | "Email is already registered"       |
    | password | "123"            | 400      | "Password must be at least 8 chars" |
    | fullName | ""               | 400      | "Full name cannot be blank"         |
    | role     | "UnknownRole"    | 400      | "Invalid user role specified"       |

AC3: The API successfully updates existing account information
Description: Allows authorized users/administrators to update profile metadata (e.g., Full Name, Phone Number, Avatar URL, Role) of an existing account identified by its accountId (or Firebase UID).
Gherkin Scenario:
Gherkin
Scenario: Successfully update existing account details
  Given An account with ID "acc_12345" exists in the database
  When A PUT/PATCH request is sent to `/api/v1/accounts/acc_12345` with the following body:
    | fullName     | phoneNumber | avatarUrl                     |
    | Nguyen Duc M | 0987654321  | https://cdn.campus.edu/avt.png|
  Then The account record is updated in the database and Firebase
  And The API responds with HTTP Status Code 200 OK
  And The response body contains the updated account details

AC4: Updating non-existent accounts is not permitted
Description: If a client attempts to update an account using a non-existent or invalid accountId, the system rejects the operation.
Gherkin Scenario:
Gherkin
Scenario: Reject account update when account does not exist
  Given No account exists with ID "acc_99999"
  When A PUT/PATCH request is sent to `/api/v1/accounts/acc_99999`
  Then The update operation is aborted
  And The API responds with HTTP Status Code 404 Not Found
  And The response body returns the error message "Account with ID acc_99999 not found"

AC5: The API returns accurate HTTP Status Codes and standard response formatting
Description: All endpoints must adhere to RESTful standards and provide a consistent response envelope for both success and error responses.
Standard Success Response:
JSON
{
  "success": true,
  "statusCode": 200,
  "message": "Account updated successfully",
  "data": {
    "id": "acc_12345",
    "email": "student@campus.edu",
    "fullName": "Nguyen Duc M",
    "role": "Student",
    "updatedAt": "2026-08-20T13:55:00Z"
  }
}

Standard Error Response (RFC 7807 Problem Details compliant):
JSON
{
  "success": false,
  "statusCode": 400,
  "error": "BAD_REQUEST",
  "message": "Validation failed",
  "errors": [
    {
      "field": "email",
      "message": "Invalid email format"
    }
  ],
  "timestamp": "2026-08-20T13:55:00Z"
}

AC6: Firebase integration for sprint task execution
Description: The implementation leverages Firebase services via Firebase Admin SDK / Firebase Auth REST API:
Manages credentials and authentication state via Firebase Authentication (CreateUserAsync, UpdateUserAsync).
Persists extended user attributes in Cloud Firestore / Realtime Database.
Ensures transactional integrity between Firebase Authentication credentials and database entities.

4. Technical API Specifications
4.1. Create Account API
Endpoint: POST /api/v1/accounts
Headers: Authorization: Bearer <Token>, Content-Type: application/json
Request Body:
JSON
{
  "email": "student@knu.ac.kr",
  "password": "SecurePassword@2026",
  "fullName": "Nguyen Duc Manh",
  "phoneNumber": "0912345678",
  "role": "Student",
  "studentCode": "B202600100"
}

Status Codes:
201 Created: Account created successfully.
400 Bad Request: Missing or invalid mandatory fields.
401 Unauthorized: Missing or invalid authentication token.
403 Forbidden: Insufficient permissions (Non-Admin caller).
409 Conflict: Email already exists.

4.2. Update Account API
Endpoint: PUT /api/v1/accounts/{id} or PATCH /api/v1/accounts/{id}
Headers: Authorization: Bearer <Token>, Content-Type: application/json
Path Parameter: id (String - Unique Account identifier / Firebase UID)
Request Body:
JSON
{
  "fullName": "Nguyen Duc Manh",
  "phoneNumber": "0987654321",
  "avatarUrl": "https://storage.googleapis.com/.../avatar.png",
  "role": "Student"
}

Status Codes:
200 OK: Account updated successfully.
400 Bad Request: Invalid payload parameters.
401 Unauthorized: Authentication required or expired token.
403 Forbidden: Unauthorized to modify the specified resource.
404 Not Found: Target account ID does not exist.
500 Internal Server Error: Unexpected database or Firebase communication failure.
