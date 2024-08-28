# **EventDriven CQRS Project**

## **Overview**

This project is a reference implementation of a system using the **Command Query Responsibility Segregation (CQRS)** pattern, **Event-Driven Architecture**, and **Domain-Driven Design (DDD)** principles. It showcases how to structure and organize an application with a focus on scalability, separation of concerns, and maintainability. The project includes the use of abstraction classes to define the behavior of the CQRS pattern, event bus, and DDD components.

## **Features**

- **CQRS Pattern**: Separation of command and query responsibilities into distinct classes. Commands are used to change the state, while queries are used to retrieve data. This separation allows for optimized read and write operations.

- **Event-Driven Architecture**: The system uses an event bus with abstraction classes to decouple the event publisher from the event handlers, promoting a scalable and loosely-coupled design. Events are raised in response to state changes, and handlers process these events asynchronously.

- **Domain-Driven Design (DDD)**: The project uaply DDD principles, with an emphasis on the use of abstract classes to define the core domain models. This approach ensures that business logic is encapsulated within the domain layer, making it easier to maintain and evolve over time.

- **Docker Integration**: The project uses Docker to run essential infrastructure components like **RabbitMQ** and **MongoDB**.

## **Components**

1. **CQRS Abstraction Classes**: Defines the structure for commands, queries, and their handlers. These abstractions ensure consistency and reusability across the application.

2. **Event Bus with Abstraction**: Provides an interface for event publication and subscription. The abstraction allows for easy integration with different messaging systems or in-memory event handling.

3. **DDD with Abstract Classes**: The domain layer is built using abstract classes that define the core business rules and entities.

4. **Dockerized Infrastructure**: RabbitMQ is used as the event bus, and MongoDB serves as the primary data store. Both services are run in Docker containers for ease of setup and consistency across different environments.

## **Getting Started**

### **Prerequisites**

- [.NET (8)](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/get-started)


## **Getting Started**

Start services using Docker:
**docker-compose up -d**
