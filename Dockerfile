# Use the official .NET image as a build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy the .csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the entire project and build the release version
COPY . ./
RUN dotnet publish -c Release -o /out

# Use a runtime-only image to keep the final image small
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the published files from the build environment
COPY --from=build-env /out .

# Expose the port that your application listens on
EXPOSE 80

# Run the application
ENTRYPOINT ["dotnet", "Hospitalmicroservices.dll"]
