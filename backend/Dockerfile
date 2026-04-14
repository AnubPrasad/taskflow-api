FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY TaskFlow-API/TaskFlow-API.csproj ./TaskFlow-API/
RUN dotnet restore ./TaskFlow-API/TaskFlow-API.csproj

COPY TaskFlow-API/ ./TaskFlow-API/
WORKDIR /src/TaskFlow-API
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TaskFlow-API.dll"]
