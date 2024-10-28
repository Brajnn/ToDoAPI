# ToDoAPI

This project is a simple ToDo API built with ASP.NET Core. It uses Docker for easy deployment and management of the application.

## Requirements

- [Docker](https://www.docker.com/get-started) must be installed on your machine.
- Clone this repository to your local machine.

## Getting Started

### 1. Clone the Repository

Open your terminal or command prompt and run the following command:

```bash
https://github.com/Brajnn/ToDoAPI.git
```

### 2. Navigate to the Project Directory
Change your working directory to the cloned repository:

```bash
cd ToDoAPI
```
### 3. Ensure Docker is Running
Before executing the following commands, make sure that Docker is running on your machine.
### 4. Build and Run the Application
You can run the application using Docker Compose. Use the following command in your terminal:
```bash
docker-compose up --build
```
This command will build the Docker images and start the containers for the application and the database.

### 5. Access the Application
Once the application is running, you can access the API documentation at:

Swagger UI: http://localhost:8080/swagger/index.html
Notes
Ensure Docker is running before executing the above commands.
If you wish to stop the application, press CTRL + C in the terminal where Docker Compose is running.
