# Sheyaaka

## Description

This API provides a set of endpoints for managing brands, stores, and related functionalities such as product management, user authentication, and address handling. It allows users to perform CRUD operations on brands, stores, and manage store addresses and products. The application also features user registration, login, email confirmation, and password management.

### Bonus Points: 
1. **In-memory caching** was used in the solution for the Brands and Products controllers.
2. **JWT IP token binding** was implemented, with a custom middleware added to validate incoming requests.
3. **Swagger** was used to document the application.
    - **Note**: Swagger has been configured to indicate which endpoints require authorization. Endpoints that require authorization are marked with a lock icon on the right.

---

## **User Endpoints**

- **POST /api/users/register**  
  Registers a new user.

- **POST /api/users/login**  
  Authenticates a user and returns a JWT token.

- **GET /api/users/confirm-email**  
  Confirms a user's email using a token sent during registration.

- **GET /api/users/reset-password-request**  
  Sends a password reset request to the user's email.

- **POST /api/users/reset-password**  
  Resets the user's password using a token and new password.

- **PUT /api/users/change-password**  
  Changes the user's password (authenticated).

- **PUT /api/users/update-profile**  
  Updates the user's profile details (authenticated).

- **GET /api/users/{id}**  
  Gets a user's profile details.

---

## **Store Endpoints**

- **GET /api/stores**  
  Retrieves a list of all stores (anonymous access).

- **GET /api/stores/{id}**  
  Retrieves a specific store by ID.

- **POST /api/stores**  
  Creates a new store (authenticated user only).

- **PUT /api/stores/{id}**  
  Updates an existing store. The user must be authenticated.

- **DELETE /api/stores/{id}**  
  Deletes a store by ID. The user must be authenticated.

- **GET /api/stores/mylist**  
  Retrieves a list of stores owned by the authenticated user.

- **GET /api/stores/{storeId}/products**  
  Retrieves a list of products for a specific store (anonymous access)  
  **[THIS ENDPOINT USES CACHING; IT GETS INVALIDATED WHENEVER A STORE IS DELETED, OR A PRODUCT IS MODIFIED (ADD/UPDATE/DELETE)]**

- **GET /api/stores/{storeId}/products/{isDeleted}**  
  Retrieves a list of store products by status (active or deleted); the user must be authenticated.

---

## **Brand Endpoints**

- **GET /api/brands**  
  Retrieves a list of all brands.  
  **[THIS ENDPOINT USES CACHING; IT GETS INVALIDATED WHENEVER A BRAND IS MODIFIED]**

- **GET /api/brands/{id}**  
  Retrieves a specific brand by ID.

- **POST /api/brands**  
  Creates a new brand.

- **PUT /api/brands/{id}**  
  Updates an existing brand.

- **DELETE /api/brands/{id}**  
  Deletes a brand by ID.

- **DELETE /api/brands/store/assignment**  
  Deletes a brand-store assignment; the user must be authenticated.

- **POST /api/brands/store/assignment**  
  Creates a brand-store assignment; the user must be authenticated.

---

## **Address Endpoints**

- **GET /api/addresses/{storeId}**  
  Retrieves a list of addresses for a specific store. The user must be authenticated.

- **POST /api/addresses**  
  Creates a new address. The user must be authenticated.

- **PUT /api/addresses/{id}**  
  Updates an existing address by ID. The user must be authenticated.

- **DELETE /api/addresses/{id}**  
  Deletes an existing address by ID. The user must be authenticated.

---

## **Product Endpoints**

- **POST /api/products**  
  Creates a new product. The user must be authenticated.

- **PUT /api/products/{id}**  
  Updates an existing product by ID. The user must be authenticated.

- **PUT /api/products/{id}/deletionstatus**  
  Changes the deletion status of a product. Set to true to delete the product, or false to recover it. The user must be authenticated.
