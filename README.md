# DataTrack — Networked Banking System

Client-server banking system using TCP sockets, multithreading, 
and JSON persistence.

## Architecture
- **ServerApp** — central server, handles all clients and data
- **ClientApp** — terminal client for users
- **TestApp** — automated test with 3 concurrent simulated clients

## Features
- Login/session management with role-based access (user/admin)
- Balance operations: BALANCE, DEPOSIT, WITHDRAW, TRANSFER
- Operation logs per user
- Admin commands: ADDUSER, DELETEUSER, EDITUSER, LISTUSERS
- SHA-256 password hashing
- Thread-safe concurrent access via locks
- JSON file persistence (users.json)

## Known Limitations
- Username is the primary identifier and can be changed by an administrator. Changing a username while the user has an active session will cause that session to reference a non-existent user, requiring reconnect and re-login.
- SHA-256 without per-user salt is used as an educational demonstration of password hashing. Production systems should use bcrypt or Argon2 with individual salts.
- Deleted users are retained in storage with role "deleted" to preserve their transaction history, as required by the assignment.

## Concurrency
All operations modifying shared state (balances, user records) are 
protected by a global lock in SystemState, ensuring data consistency 
when multiple clients act simultaneously.

## Running
```bash
# Terminal 1 — Server
cd ServerApp && dotnet run

# Terminal 2 — Client
cd ClientApp && dotnet run

# Or run automated test
cd TestApp && dotnet run
```