FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /app
EXPOSE 80

# copy all .csproj files and restore as distinct layers.   Use of the same COPY command
# for every dockerfile in the project to take advantage of docker caching
COPY CityLibraryMicroserviceTemplate.sln CityLibraryMicroserviceTemplate.sln
COPY src/UserServiceApi/UserServiceApi.csproj src/UserServiceApi/UserServiceApi.csproj
COPY src/BookServiceApi/BookServiceApi.csproj src/BookServiceApi/BookServiceApi.csproj
COPY src/BookReservationReportApi/BookReservationReportApi.csproj src/BookReservationReportApi/BookReservationReportApi.csproj
COPY src/GatewayService/GatewayService.csproj src/GatewayService/GatewayService.csproj
COPY src/CityLibrary.Shared/CityLibrary.Shared.csproj src/CityLibrary.Shared/CityLibrary.Shared.csproj
COPY tests/UserServiceApi.UnitTests/UserServiceApi.UnitTests.csproj tests/UserServiceApi.UnitTests/UserServiceApi.UnitTests.csproj
COPY tests/UserServiceApi.IntegrationTests/UserServiceApi.IntegrationTests.csproj tests/UserServiceApi.IntegrationTests/UserServiceApi.IntegrationTests.csproj
COPY tests/BookServiceApi.UnitTests/BookServiceApi.UnitTests.csproj tests/BookServiceApi.UnitTests/BookServiceApi.UnitTests.csproj
COPY tests/BookServiceApi.IntegrationTests/BookServiceApi.IntegrationTests.csproj tests/BookServiceApi.IntegrationTests/BookServiceApi.IntegrationTests.csproj
COPY tests/BookReservationReportApi.UnitTests/BookReservationReportApi.UnitTests.csproj tests/BookReservationReportApi.UnitTests/BookReservationReportApi.UnitTests.csproj
COPY tests/BookReservationReportApi.IntegrationTests/BookReservationReportApi.IntegrationTests.csproj tests/BookReservationReportApi.IntegrationTests/BookReservationReportApi.IntegrationTests.csproj


# Restore package deps
RUN dotnet restore CityLibraryMicroserviceTemplate.sln

# Copy the app folders over
COPY src/GatewayService src/GatewayService
COPY src/CityLibrary.Shared src/CityLibrary.Shared
WORKDIR /app/src/GatewayService
RUN dotnet publish -c Release -o /app/src/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/src/out .
ENTRYPOINT [ "dotnet", "GatewayService.dll" ]