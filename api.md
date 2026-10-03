# SmartBiz API

**Base URL (development):** `http://localhost:5266`
**Auth scheme:** JWT Bearer (`Authorization: Bearer <accessToken>`)
**Error format:** RFC 7807 Problem Details

## Auth Endpoints

| Method | Path | Auth | Purpose |
|---|---|---|---|
| POST | `/api/v1/auth/register` | Public | Create user + business |
| POST | `/api/v1/auth/login` | Public | Get JWT + refresh token |
| POST | `/api/v1/auth/refresh` | Public | Rotate refresh token |
| POST | `/api/v1/auth/logout` | Public | Revoke refresh token |
| GET  | `/api/v1/auth/me` | 🔒 Required | Current user info |

## Health

| Method | Path | Auth | Purpose |
|---|---|---|---|
| GET | `/api/health` | Public | Liveness probe |

## Status Codes

| Code | Meaning |
|---|---|
| 200 | OK |
| 201 | Created |
| 204 | No Content (idempotent operations) |
| 400 | Validation failed |
| 401 | Invalid credentials / token |
| 403 | Authenticated but not allowed |
| 409 | Conflict (e.g. duplicate email) |
| 500 | Server error |

## Multi-Tenancy

Every authenticated request's `BusinessId` is derived **exclusively** from the JWT. EF Core global query filters automatically scope every query to the current tenant. Tenant data cannot leak between businesses.

## Roles

- **Admin** — full access to everything for the business
- (More roles come in Phase 5: Cashier, Accountant, Viewer, etc.)

## Test-Only Endpoints (Dev)

- `GET /api/v1/test-auth/any-user` — any authenticated user
- `GET /api/v1/test-auth/admin-only` — Admin role only

These will be removed before production.