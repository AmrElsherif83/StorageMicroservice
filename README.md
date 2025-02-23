# StorageMicroservice

1)	The function and nonfunctional requirement for this microservice
 - Functional Requirements (FRs)
These define what the system should do.

🔹 File Management
Upload File

Users can upload files via an API endpoint.
The system stores files in the configured storage provider (AWS S3, Azure Blob, Local).
Metadata (e.g., filename, URL) is stored in a database.

Download File

Users can download files using a unique identifier or filename.
The system fetches the file from the respective storage provider.
List Files

Users can retrieve a paginated list of stored files.
Search and filtering options are available based on filename or metadata.

Delete File (Optional)

Users can delete files from storage and remove their metadata from the database.

🔹 Storage Provider Support
Support for Multiple Storage Providers
The microservice should support:
AWS S3
Azure Blob Storage
Local Storage
Storage provider selection should be dynamic using IOptionsMonitor.

🔹 Event-Driven Architecture
Message Queue for Asynchronous Processing

RabbitMQ (or another queue) is used for event-driven communication.
Publishing events when a file is uploaded or deleted.
Retry Mechanism for Messaging

Messages should have retry logic using Polly (e.g., AsyncRetryPolicy).

🔹 Security & Access Control
Authentication & Authorization

Secure endpoints using JWT authentication or API keys.
Role-Based Access Control (RBAC)

Admins can manage all files, but regular users can only access their own files.
File Access Restrictions

Generate pre-signed URLs for time-limited access to private files.

- Non-Functional Requirements (NFRs)
These define how the system should perform.

🔹 Performance
Scalability

Must support horizontal scaling using Docker & Kubernetes.
Use Redis caching for frequently accessed metadata.
Low Latency

Uploads and downloads should be optimized for minimal delay.
Use CDN (Content Delivery Network) for faster file access.

2)	The high level and low-level design for the microservice

High-Level and Low-Level Design for the Storage Microservice

1️⃣ High-Level Design (HLD)
The High-Level Design (HLD) provides an architectural overview of the system, focusing on components, modules, and interactions.

🔹 Architecture Overview
The Storage Microservice follows a Clean Architecture + Hexagonal Architecture approach.
It uses CQRS (Command Query Responsibility Segregation) and Event-Driven Communication.
Supports multiple storage providers (AWS S3, Azure Blob, Local Storage).
Uses RabbitMQ for messaging, MongoDB for metadata storage, and Polly for resilience.
🔹 Key Components
API Layer (Controllers)

Handles HTTP requests for file uploads, downloads, and metadata retrieval.
Uses RESTful API endpoints.
Application Layer (Services, Handlers)

Implements business logic using MediatR (CQRS pattern).
Implements command handlers (Upload, Delete) and query handlers (List, Download).
Uses DTOs (Data Transfer Objects) for communication.
Infrastructure Layer (Storage Providers, Messaging, Database)

Storage Providers: AWS S3, Azure Blob, Local Storage.
Message Queue: RabbitMQ for event-driven processing.
Database: MongoDB for storing file metadata.
Domain Layer (Entities, Interfaces)

Defines core domain models (e.g., FileMetadata).
Defines interfaces for IStorageProvider, IEventBus, IFileRepository.
🔹 High-Level Architecture Diagram
pgsql
Copy
Edit
             +-------------------------------------------------+
             |        Storage Microservice API                |
             |        (ASP.NET Core, MediatR, Swagger)        |
             +-----------------------+------------------------+
                                     |
  +---------------------------+---------------------------+
  |         Application Layer (CQRS Handlers)            |
  |  - UploadFileHandler       - DownloadFileHandler     |
  |  - ListFilesHandler        - DeleteFileHandler      |
  |  - Uses DTOs and Services                           |
  +---------------------------+---------------------------+
                                     |
  +---------------------------+---------------------------+
  |        Infrastructure Layer (Storage, DB, MQ)        |
  |  - Storage Providers: AWS S3, Azure Blob, Local      |
  |  - RabbitMQ for Event Messaging                     |
  |  - MongoDB for File Metadata Storage                |
  +---------------------------+---------------------------+
                                     |
  +---------------------------+---------------------------+
  |        Domain Layer (Entities & Interfaces)         |
  |  - FileMetadata (MongoDB Document)                  |
  |  - IStorageProvider (Storage Abstraction)           |
  |  - IEventBus (Messaging Abstraction)                |
  +------------------------------------------------------+

2️⃣ Low-Level Design (LLD)
The Low-Level Design (LLD) provides detailed implementation and class-level architecture.

🔹 API Endpoints
HTTP Method	Endpoint	Description
POST	/api/storage/upload	Uploads a file
GET	/api/storage/download/{filename}	Downloads a file
GET	/api/storage/list	Lists stored files
DELETE	/api/storage/delete/{filename}	Deletes a file

Conclusion
✅ High-Level Design (HLD) focuses on architecture, modules, and major components.
✅ Low-Level Design (LLD) focuses on class-level details and implementation logic.
This microservice follows best practices like:

Clean Architecture
CQRS + Event-Driven
MongoDB + RabbitMQ
Storage Abstraction for Multi-Cloud Support


3)	The type of the storage that will be used and why, and how the files will be saved and retrieved 

Storage Type Selection and File Handling Strategy
1️⃣ Storage Types and Their Use Cases
The Storage Microservice supports multiple storage backends based on configuration. The choice of which storage type to use depends on business needs and scalability requirements. The supported storage options include:

Storage Type	Use Case	Why Use It?
Amazon S3	Cloud storage for distributed, scalable applications	Highly available, cost-effective, and supports lifecycle policies
Azure Blob Storage	Enterprise cloud applications using Microsoft Azure	High scalability, integration with Azure ecosystem
Local File System	Small-scale or on-premise storage	Simple, fast, and requires no cloud dependency
MongoDB GridFS (optional)	Storing large files within MongoDB	Allows querying file metadata directly in MongoDB
2️⃣ How Files Will Be Saved and Retrieved
Regardless of the storage provider, the file handling workflow follows a consistent process:

📌 File Upload Process
Receive File: The API receives a file via HTTP POST (multipart/form-data).
Choose Storage Provider: The system dynamically selects a storage provider (S3, Azure, Local).
Save File: The provider uploads the file and returns the file URL.
Save Metadata: The metadata (file name, storage location, timestamp) is stored in MongoDB.
Emit Event: A message is published to RabbitMQ for further processing.
📌 File Retrieval Process
Find Metadata: The system queries MongoDB to find the storage URL.
Retrieve File: The storage provider downloads the file from S3/Azure/Local.
Return File Stream: The API serves the file as a streamed response.

3️⃣ Why This Approach?
✔ Scalable: Cloud storage (S3/Azure) can handle large-scale file uploads.
✔ Flexible: The system supports multiple storage backends without code changes.
✔ Efficient: Metadata is stored in MongoDB, allowing fast queries without scanning storage.
✔ Resilient: Uses RabbitMQ for event-driven processing (e.g., notify services when a file is uploaded).


4)	How the other microservice will communicate with the new designed one?


Communication with Storage Microservice 🚀
The Storage Microservice will support multiple communication mechanisms to allow seamless interaction with other microservices. The key communication patterns are:

1️⃣ Synchronous Communication (REST API) 🛠️
🔹 Use Case: When other microservices need to directly interact with the Storage Microservice for file uploads, downloads, and metadata retrieval.
🔹 Protocol: HTTP (REST API)
🔹 Data Format: JSON

📌 REST API Endpoints
Method	Endpoint	Description
POST	/api/storage/upload	Uploads a file and returns metadata
GET	/api/storage/download/{fileName}	Downloads a file
GET	/api/storage/list?page=1&pageSize=10	Retrieves paginated file metadata
DELETE	/api/storage/delete/{fileName}	Deletes a file

2️⃣ Asynchronous Communication (Event-Driven with RabbitMQ) 📨
🔹 Use Case: When other microservices need event notifications about file uploads (e.g., Logging, Analytics, AI Processing).
🔹 Protocol: RabbitMQ (Message Broker)
🔹 Data Format: JSON (Serialized Event Messages)

📌 Events Published by Storage Microservice
Event	Exchange	Routing Key	Subscribers
FileUploadedEvent	storage-exchange	file.uploaded	Logging, AI Processing
FileDeletedEvent	storage-exchange	file.deleted	Cleanup Service
