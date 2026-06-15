# Finance Server (Go Migration)

This project is a migration of the original C# finance server to Go, following Clean Architecture principles.

## Tech Stack
- **Framework**: Gin
- **ORM**: GORM
- **Database**: SQLite (Multi-tenant, one `.db` file per account)
- **Architecture**: Clean Architecture (Domain, Usecase, Repository, Delivery)

## API Documentation

### 1. Authentication
- **URL**: `/user/login`
- **Method**: `POST`
- **Request Body**:
```json
{
    "Content": {
        "UserName": "admin",
        "PassWord": "encrypted_base64_password",
        "Tid": 1
    }
}
```
- **Response**:
```json
{
    "Result": 0,
    "UserId": 13594,
    "UserName": "admin",
    "Token": "base64_token"
}
```

### 2. Voucher Save
- **URL**: `/voucher/save?Token={token}`
- **Method**: `POST`
- **Request Body**:
```json
{
    "Content": {
        "header": {
            "id": 0,
            "word": "记",
            "year": 2024,
            "period": 3,
            "date": "2024-03-05T00:00:00Z"
        },
        "entries": [
            {
                "index": 1,
                "accountSubjectId": 18,
                "amount": 100.0,
                "direction": 1
            },
            {
                "index": 2,
                "accountSubjectId": 19,
                "amount": 100.0,
                "direction": -1
            }
        ]
    }
}
```
- **Response**:
```json
{
    "Result": 0,
    "id": 233
}
```

### 3. Voucher List
- **URL**: `/voucher/list?Token={token}`
- **Method**: `POST`
- **Response**:
```json
{
    "Result": 0,
    "Content": [...]
}
```

### 4. Voucher Delete
- **URL**: `/voucher/delete?Token={token}&id={id}`
- **Method**: `POST`
- **Response**:
```json
{
    "Result": 0
}
```

## Setup and Running
1. Install Go 1.25+
2. Run `go mod tidy`
3. Run `go build -o fin ./cmd/main.go`
4. Execute `./fin`
