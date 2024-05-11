## Microservice Boilerplate Repository

This repository has been created in order to demonstrate a microservice structure.
The repository includes api calls by using Grpc and Refit.
It includes also localization, global error handling mechanism, automatic updating audit fields in db for each api services.
In addition, there are unit and integration tests for main services.

There are three main services exist in the solution: UserServiceApi, BookServiceApi, and BookReservationReportApi.
Furthermore, there is a gateway service for reverse proxy (Gateway service port: 6001).

#### User Service Api

UserServiceApi handles authentication process for getting jwt and registering, updating user information etc.
It includes also refresh token mechanism with Redis. It stores data in postgresql and has automatic migration mechanism.
If a user is created or updated, it sends related commands and events for other apis by using MassTransit (RabbitMQ).
It uses local port: 5101. The endpoints can be observed at "/swagger/index.html".
You can make request with same endpoints with gateway service without using "/api" except AuthController.

#### Book Service Api

BookServiceApi is responsible for handling main book data in system.
It stores also some user data to validate user's data in assignment or unassignment process.
BookServiceApi uses mysql for data storage and has automatic migration mechanism.
It makes Grpc and Http calls to BookReservationReportApi in assignment or unassignment process.
Moreover, it sends related commands to BookReservationReportApi if books are updated.
It uses local port: 5102. The endpoints can be observed at "/swagger/index.html".
You can make request with same endpoints with gateway service without using "/api".

#### Book Reservation Report Api

BookReservationReportApi is responsible for reporting data to the client (Admin Role only).
It stores data in MongoDB.
It uses local port: 5103. The endpoints can be observed at "/swagger/index.html".
You can make request with same endpoints with gateway service without using "/api" except RecordController.

#### To Build the services locally on your computer by running (This may take several minutes to complete):
```
docker compose build
```
#### Once this completes you can use the following to run the services:
```
docker compose up -d
```
#### To stop the containers the following command can be used (-v option is removing docker volumes):
```
docker compose down -v
```