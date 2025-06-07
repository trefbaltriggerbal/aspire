# Client Portal Roadmap

This document outlines the proposed features and milestones for building a simple client portal for an insurance broker. The portal will extend the existing **WebFrontend** Blazor project while keeping the repository focused on a minimal Aspire sample.

## Big Picture

1. **Authentication & Authorization**
   - Client login with client number and password.
   - Parent login using token or password.
   - Administrator login with full access.

2. **Policy & Claims Management**
   - Display policies associated with a client.
   - View open/closed claims and create new claims within the policy period.
   - Upload documents (photos, invoices) to a claim and track status updates.
   - Share claims with parents and track claim payments.

3. **Invoices & Payments**
   - Show paid, pending and upcoming invoices per policy.
   - Provide downloadable policy documents and payment summaries.

4. **Notifications & Logging**
   - Send email or portal notifications on claim status changes and new invoices.
   - Record security events and maintain an audit trail.
   - Support multi-language user interface.

## Milestones

1. **Initial Setup**
   - Create API endpoints and database models for policies, claims and invoices.
   - Secure the Blazor app with cookie-based authentication.

2. **Client Features**
   - List policies and show claim history.
   - Allow claim creation with document upload.
   - Display invoice overview.

3. **Parent Features**
   - Token-based login and optional password setup.
   - Access to shared claims and payment overviews.

4. **Administrator Features**
   - Manage clients and parents (create, reset password).
   - Assign claims to handlers and view security logs.

5. **General Enhancements**
   - Multi-language support and email notifications.
   - Central logging and auditing.

Each milestone will be tracked via GitHub issues. Individual pull requests will implement the tasks iteratively while ensuring `dotnet build` and `dotnet test` succeed.
