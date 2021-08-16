FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY WebApplication1/*.csproj ./WebApplication1/
COPY DomainLogic/*.csproj ./DomainLogic/
COPY Implementions/*.csproj ./Implementions/
COPY Implementions.Mq/*.csproj ./Implementions.Mq/
COPY Implementions.YandexDiskAPI/*.csproj ./Implementions.YandexDiskAPI/
COPY WorkerService/*.csproj ./WorkerService/
RUN dotnet restore

# copy everything else and build app
COPY WebApplication1/. ./WebApplication1/
COPY DomainLogic/. ./DomainLogic/
COPY Implementions/. ./Implementions/
COPY Implementions.Mq/. ./Implementions.Mq/
COPY Implementions.YandexDiskAPI/. ./Implementions.YandexDiskAPI/
COPY WorkerService/. ./WorkerService/

WORKDIR /app/WebApplication1
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app
COPY --from=build /app/WebApplication1/out ./

CMD ASPNETCORE_URLS=http://*:$PORT dotnet WebApplication1.dll