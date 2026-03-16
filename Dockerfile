# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj và restore
COPY backend/src/VocabToeic.API/VocabToeic.API.csproj backend/src/VocabToeic.API/
COPY backend/src/VocabToeic.Application/VocabToeic.Application.csproj backend/src/VocabToeic.Application/
COPY backend/src/VocabToeic.Domain/VocabToeic.Domain.csproj backend/src/VocabToeic.Domain/
COPY backend/src/VocabToeic.Infrastructure/VocabToeic.Infrastructure.csproj backend/src/VocabToeic.Infrastructure/

RUN dotnet restore backend/src/VocabToeic.API/VocabToeic.API.csproj

# Copy source và publish
COPY NuGet.Config .
COPY backend/ backend/

RUN dotnet publish backend/src/VocabToeic.API/VocabToeic.API.csproj \
    -c Release \
    -o /app/publish 

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "VocabToeic.API.dll"]
