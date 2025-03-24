# Getting Started

## Running the Application

The application uses Docker Compose for easy setup and running. Here's how to get started:

1. Make sure you have Docker and Docker Compose installed on your machine
2. Open a terminal in the project root directory
3. Navigate to the backend folder:
   ```bash
   cd template/backend
   ```
4. Run the following command to start the application:
   ```bash
   docker-compose up
   ```

The application will be available at `http://localhost:5000`

## Default Admin Credentials

The application comes with a default admin user that is created during the initial seed:

- Email: admin@ambev.com
- Password: Admin@123
- Role: Admin
- Status: Active

## API Examples

### Authentication

#### Authenticate User
```http
POST /api/auth
Content-Type: application/json

{
    "email": "admin@ambev.com",
    "password": "Admin@123"
}
```

Response:
```json
{
    "success": true,
    "data": {
        "token": "eyJhbGciOiJIUzI1NiIs...",
        "email": "admin@ambev.com",
        "name": "admin",
        "role": "Admin"
    }
}
```

### Users

#### Create User
```http
POST /api/users
Authorization: Bearer <token>
Content-Type: application/json

{
    "username": "john.doe",
    "email": "john.doe@example.com",
    "password": "Test@123",
    "phone": "+5511999999999",
    "status": "Active",
    "role": "Customer"
}
```

Response:
```json
{
    "success": true,
    "data": {
        "id": "guid-here",
        "name": "john.doe",
        "email": "john.doe@example.com",
        "phone": "+5511999999999",
        "role": "Customer",
        "status": "Active"
    }
}
```

#### Get User
```http
GET /api/users/{id}
Authorization: Bearer <token>
```

Response:
```json
{
    "success": true,
    "data": {
        "id": "guid-here",
        "name": "john.doe",
        "email": "john.doe@example.com",
        "phone": "+5511999999999",
        "role": "Customer",
        "status": "Active"
    }
}
```

#### Delete User
```http
DELETE /api/users/{id}
Authorization: Bearer <token>
```

Response:
```json
{
    "success": true,
    "message": "User deleted successfully"
}
```

### Sales

#### Create Sale
```http
POST /api/sales
Authorization: Bearer <token>
Content-Type: application/json

{
    "customerName": "John Doe",
    "branchName": "Store A",
    "items": [
        {
            "productName": "Product 1",
            "quantity": 5,
            "unitPrice": 10.00
        },
        {
            "productName": "Product 2",
            "quantity": 3,
            "unitPrice": 15.00
        }
    ]
}
```

Response:
```json
{
    "success": true,
    "data": {
        "id": "guid-here",
        "saleNumber": "SALE-001",
        "date": "2024-03-24T12:00:00Z",
        "customerName": "John Doe",
        "branchName": "Store A",
        "totalAmount": 95.00,
        "items": [
            {
                "productName": "Product 1",
                "quantity": 5,
                "unitPrice": 10.00,
                "discount": 5.00,
                "totalAmount": 45.00
            },
            {
                "productName": "Product 2",
                "quantity": 3,
                "unitPrice": 15.00,
                "discount": 0.00,
                "totalAmount": 45.00
            }
        ],
        "cancelled": false
    }
}
```

#### Get Sale
```http
GET /api/sales/{id}
Authorization: Bearer <token>
```

Response:
```json
{
    "success": true,
    "data": {
        "id": "guid-here",
        "saleNumber": "SALE-001",
        "date": "2024-03-24T12:00:00Z",
        "customerName": "John Doe",
        "branchName": "Store A",
        "totalAmount": 95.00,
        "items": [
            {
                "productName": "Product 1",
                "quantity": 5,
                "unitPrice": 10.00,
                "discount": 5.00,
                "totalAmount": 45.00
            },
            {
                "productName": "Product 2",
                "quantity": 3,
                "unitPrice": 15.00,
                "discount": 0.00,
                "totalAmount": 45.00
            }
        ],
        "cancelled": false
    }
}
```

#### List Sales
```http
GET /api/sales
Authorization: Bearer <token>
```

Response:
```json
{
    "success": true,
    "data": {
        "items": [
            {
                "id": "guid-here",
                "saleNumber": "SALE-001",
                "date": "2024-03-24T12:00:00Z",
                "customerName": "John Doe",
                "branchName": "Store A",
                "totalAmount": 95.00,
                "cancelled": false
            }
        ],
        "currentPage": 1,
        "totalPages": 1,
        "totalCount": 1
    }
}
```

#### Cancel Sale
```http
DELETE /api/sales/{id}
Authorization: Bearer <token>
```

Response:
```json
{
    "success": true,
    "message": "Sale cancelled successfully"
}
```

#### Cancel Sale Item
```http
DELETE /api/sales/{id}/items/{itemId}
Authorization: Bearer <token>
```

Response:
```json
{
    "success": true,
    "message": "Sale item cancelled successfully"
}
```

## Authentication

All endpoints except the authentication endpoint require a valid JWT token. Include the token in the Authorization header:

```http
Authorization: Bearer <your-token-here>
```

## Validation Rules

### User Creation
- Username: 3-50 characters
- Email: Valid email format
- Password: Must meet security requirements (minimum 8 characters, at least one uppercase letter, one lowercase letter, one number, and one special character)
- Phone: Must match international format (+X XXXXXXXXXX)
- Status: Cannot be Unknown
- Role: Cannot be None

### Sale Creation
- Customer Name: Required
- Branch Name: Required
- Items: At least one item required
- Product Name: Required
- Quantity: Between 1 and 20
- Unit Price: Greater than 0
- Discounts are automatically applied based on quantity:
  - 4+ items: 10% discount
  - 10-20 items: 20% discount
  - Below 4 items: No discount

## Error Responses

The API returns standardized error responses in the following format:

```json
{
    "success": false,
    "message": "Error message here"
}
```

Common HTTP status codes:
- 200: Success
- 201: Created
- 400: Bad Request
- 401: Unauthorized
- 404: Not Found
- 500: Internal Server Error 