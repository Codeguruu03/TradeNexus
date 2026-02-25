# API Usage Documentation

## Authentication

All API endpoints require Basic Authentication. Include credentials in the `Authorization` header:

```
Authorization: Basic <base64-encoded-credentials>
```

### Available Users

| Username | Password   | Role   |
|----------|-----------|--------|
| admin    | admin123  | Admin  |
| trader   | trader123 | Trader |
| risk     | risk123   | Risk   |

## API Endpoints

### 1. Clients API

#### Get All Clients
- **Endpoint**: `GET /api/clients`
- **Roles**: Admin, Trader, Risk
- **Response**: Array of clients

#### Get Client by ID
- **Endpoint**: `GET /api/clients/{id}`
- **Roles**: Admin, Trader, Risk
- **Response**: Single client object

#### Create Client
- **Endpoint**: `POST /api/clients`
- **Roles**: Admin
- **Request Body**:
```json
{
  "clientName": "John Doe",
  "subBrokerId": 1,
  "tradingLimit": 100000.00
}
```

#### Update Client
- **Endpoint**: `PUT /api/clients/{id}`
- **Roles**: Admin
- **Request Body**: Complete client object

#### Delete Client
- **Endpoint**: `DELETE /api/clients/{id}`
- **Roles**: Admin
- **Response**: 204 No Content

---

### 2. SubBrokers API

#### Get All SubBrokers
- **Endpoint**: `GET /api/subbrokers`
- **Roles**: Admin, Trader, Risk
- **Response**: Array of sub-brokers

#### Get SubBroker by ID
- **Endpoint**: `GET /api/subbrokers/{id}`
- **Roles**: Admin, Trader, Risk
- **Response**: Single sub-broker object

#### Create SubBroker
- **Endpoint**: `POST /api/subbrokers`
- **Roles**: Admin
- **Request Body**:
```json
{
  "brokerName": "XYZ Securities",
  "exposureLimit": 5000000.00,
  "isActive": true
}
```

#### Update SubBroker
- **Endpoint**: `PUT /api/subbrokers/{id}`
- **Roles**: Admin
- **Request Body**: Complete sub-broker object

#### Delete SubBroker
- **Endpoint**: `DELETE /api/subbrokers/{id}`
- **Roles**: Admin
- **Response**: 204 No Content

---

### 3. Trades API

#### Get Trades
- **Endpoint**: `GET /api/trades?clientId={clientId}&subBrokerId={subBrokerId}`
- **Roles**: Admin, Trader, Risk
- **Query Parameters** (optional):
  - `clientId`: Filter by client
  - `subBrokerId`: Filter by sub-broker
- **Response**: Array of trades

#### Get Trade by ID
- **Endpoint**: `GET /api/trades/{id}`
- **Roles**: Admin, Trader, Risk
- **Response**: Single trade object

#### Create Trade
- **Endpoint**: `POST /api/trades`
- **Roles**: Admin
- **Request Body**: Complete trade object with all required fields

#### Update Trade
- **Endpoint**: `PUT /api/trades/{id}`
- **Roles**: Admin
- **Request Body**: Complete trade object

#### Delete Trade
- **Endpoint**: `DELETE /api/trades/{id}`
- **Roles**: Admin
- **Response**: 204 No Content

---

### 4. Risk Calculation API

#### Calculate Risk for Client
- **Endpoint**: `POST /api/risk/{clientId}`
- **Roles**: Admin, Risk
- **Response**:
```json
{
  "clientId": 1,
  "clientName": "John Doe",
  "tradeCount": 10,
  "totalExposure": 250000.00,
  "riskResult": "Risk calculation output from Python engine"
}
```

---

## Example Usage with curl

### Login as Admin and Get Clients
```bash
curl -X GET https://localhost:5001/api/clients \
  -H "Authorization: Basic YWRtaW46YWRtaW4xMjM="
```

### Create a New Client
```bash
curl -X POST https://localhost:5001/api/clients \
  -H "Authorization: Basic YWRtaW46YWRtaW4xMjM=" \
  -H "Content-Type: application/json" \
  -d '{
    "clientName": "Jane Smith",
    "subBrokerId": 2,
    "tradingLimit": 150000.00
  }'
```

### Calculate Risk for Client
```bash
curl -X POST https://localhost:5001/api/risk/123 \
  -H "Authorization: Basic cmlzazpyaXNrMTIz"
```

---

## Role-Based Access Control

- **Admin**: Full access to all endpoints (GET, POST, PUT, DELETE)
- **Trader**: Read-only access to clients, sub-brokers, and trades
- **Risk**: Read-only access plus risk calculation capabilities

---

## Error Responses

- **401 Unauthorized**: Missing or invalid authentication
- **403 Forbidden**: Insufficient role permissions
- **404 Not Found**: Resource not found
- **400 Bad Request**: Invalid request data
