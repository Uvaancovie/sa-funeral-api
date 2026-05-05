# Testing Strategy (Enterprise & Compliance)

This document defines a comprehensive, enterprise-grade testing strategy for the SA Funeral Supplies platform (backend API, frontend catalog, data pipelines, and integrations). It is designed to meet reliability, security, privacy, and compliance expectations.

## 1) Scope and Objectives

- Prevent regressions across API, UI, data, and integrations.
- Maintain availability and performance under expected load.
- Ensure data integrity and security (PII, auth, role enforcement).
- Provide evidence for compliance and auditability.

## 2) Architecture Under Test

- Backend: ASP.NET Core API with EF Core + Postgres (Supabase).
- Frontend: Angular SPA (catalog + admin).
- Integrations: Brevo (email/marketing), Supabase storage, auth, and DB.
- Data tooling: SQL scripts, data seeders, and migrations.

## 3) Test Levels

### 3.1 Unit Tests

**Purpose:** Verify business logic in isolation.

**Targets:**
- Services: validation, serialization, mapping, and domain logic.
- Utilities: helpers (parsing, formatting, validation).
- Model behaviors: computed properties, parsing methods, rule enforcement.

**Standards:**
- Fast, deterministic, no external dependencies.
- Use test doubles for database/HTTP.

**Tools:**
- Backend: xUnit/NUnit + Moq/FakeItEasy.
- Frontend: Jest/Vitest for utilities and pure components.

### 3.2 Integration Tests

**Purpose:** Validate components working together.

**Targets:**
- API + Database (EF Core against a test DB).
- API + external services (Brevo, Supabase), with mocks or contract tests.
- Data migrations and seeders.

**Standards:**
- Use isolated test database per run.
- Do not call real third-party services in CI; mock or use sandboxes.

**Tools:**
- Backend: Testcontainers for Postgres; WireMock for HTTP.

### 3.3 End-to-End (E2E) Tests

**Purpose:** Validate critical user journeys.

**Targets:**
- Catalog browsing, search, filters.
- Product details and add-to-quote flow.
- Authentication and role-based access.
- Admin workflows: products, orders, audit logs.

**Tools:**
- Playwright (already in use).

**Standards:**
- Deterministic data setup (seed before tests or mock APIs).
- Avoid flaky selectors; use `data-testid`.

### 3.4 Contract Tests

**Purpose:** Enforce stable API contracts between frontend and backend.

**Targets:**
- `/api/products`, `/api/orders`, `/api/auth`.
- Request/response schema validation.

**Standards:**
- Automated schema validation in CI.
- Backward compatibility checks.

**Tools:**
- OpenAPI validation or schema-based testing.

### 3.5 Security Tests

**Purpose:** Ensure system is protected against common threats.

**Targets:**
- Auth flows (JWT expiry, invalid tokens, role checks).
- Input validation and anti-injection.
- RLS policies and authorization in database.

**Standards:**
- Validate OWASP Top 10 risks.
- Verify secrets are not committed (pre-commit + CI scanning).

**Tools:**
- SAST: CodeQL or Semgrep.
- Secret scanning: GitHub Advanced Security (already active).

### 3.6 Performance Tests

**Purpose:** Validate response times and scalability.

**Targets:**
- `GET /api/products` under load.
- Order creation throughput.
- Admin endpoints with pagination.

**Standards:**
- Define SLAs (e.g., p95 < 500ms for catalog queries).

**Tools:**
- k6 or Locust.

### 3.7 Accessibility Tests

**Purpose:** Ensure the UI is accessible to WCAG 2.1 AA.

**Targets:**
- Catalog, product detail, login, admin portal.

**Tools:**
- Playwright + axe-core integration.

### 3.8 Compliance Tests

**Purpose:** Validate privacy/security controls (POPIA/GDPR aligned).

**Targets:**
- PII handling in logs and emails.
- Data retention and deletion workflows.
- Consent for marketing communications.

**Standards:**
- Ensure marketing list sync respects consent flags.
- Audit trails for admin actions.

### 3.9 POPIA/GDPR Control Mapping

**Purpose:** Tie privacy requirements to verifiable tests.

**Controls and Tests:**
- Lawful basis and consent: verify marketing sync only occurs with explicit consent flag.
- Data minimization: ensure only required fields are sent to Brevo.
- Purpose limitation: validate that order data is not reused outside order workflow.
- Data subject rights: test export and deletion workflow (admin or user-initiated where applicable).
- Retention: verify scheduled or manual retention policy enforcement.
- Breach readiness: ensure logs include non-sensitive identifiers and event correlation IDs.

## 4) Test Data Strategy

- Use anonymized or synthetic datasets.
- Separate environments for dev/test/staging/prod.
- Seeders for consistent test data.
- No production data in test environments.

## 5) CI/CD Integration

- Run lint + unit tests on every PR.
- Run integration tests on main branch and nightly.
- Run E2E smoke suite on every PR.
- Full E2E and performance tests in staging.

## 6) Observability and Monitoring

- Log test runs and failures.
- Track flaky tests and quarantine until fixed.
- Capture screenshots and traces for E2E.

## 7) Release Gates

- No high-severity security findings.
- All unit/integration/E2E tests pass.
- Performance thresholds met.
- Required approvals for production deployment.

## 8) Recommended Initial Test Suite

### Backend
- Unit tests for order validation and email logic.
- Integration tests for order creation and status updates.

### Frontend
- E2E: catalog load, product detail navigation, add to cart.
- Accessibility checks on catalog and product pages.

### Integrations
- Brevo email payload schema validation.
- Supabase storage upload mock test.

## 9) Module and Endpoint Coverage Map

### API Endpoints
- `GET /api/products`: list, filters, pagination, error handling.
- `GET /api/products/{id}`: not found, valid product, image parsing.
- `POST /api/orders`: auth required, validation, confirmation email, Brevo sync.
- `GET /api/orders/my`: auth required, only own orders.
- `GET /api/orders`: admin-only access, pagination.
- `PUT /api/orders/{id}/status`: admin-only, valid status transitions.

### Frontend Modules
- Catalog: list load, filters, search, product navigation.
- Product Detail: image viewer, variants, add to quote.
- Cart: add/remove/update quantity.
- Auth: login, role gating, session expiry.
- Admin: products, orders, audit logs (role-restricted).

### Data and Migrations
- Product seed and migration scripts applied in test DB.
- Backward compatibility checks for API response shape.

## 10) Implementation Roadmap (Practical)

### Phase 1 (Immediate)
- Add unit tests for order validation and JSON parsing helpers.
- Add API integration tests for `POST /api/orders` and `GET /api/orders/my`.
- Add Playwright E2E smoke tests for catalog and product detail.

### Phase 2 (Stabilize)
- Add contract tests for core API endpoints.
- Add accessibility checks for catalog and product pages.
- Add Brevo payload tests and consent gating.

### Phase 3 (Enterprise)
- Add performance tests for catalog and orders.
- Add security scanning gates and scheduled scans.
- Add retention and deletion workflow tests.

## 11) Governance

- Test ownership mapped to features.
- Regular reviews for test coverage and failures.
- Compliance evidence stored in CI artifacts.

## 12) Evidence and Auditability

- Retain test reports for each release.
- Capture security scan results.
- Maintain change logs for sensitive features.

---

If you want, I can also add a CI pipeline example (GitHub Actions) with these stages, or scaffold test folders for each layer.