FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY ./TimeTracker.API/TimeTracker.API.csproj ./TimeTracker.API/
COPY ./TimeTracker.DataBase/TimeTracker.DataBase.csproj ./TimeTracker.DataBase/
COPY ./TImeTracker.DataBase.Models/TImeTracker.DataBase.Models.csproj ./TImeTracker.DataBase.Models/
COPY ./TimeTracker.Interfaces/TimeTracker.Interfaces.csproj ./TimeTracker.Interfaces/
COPY ./TimeTracker.Models/TimeTracker.Models.csproj ./TimeTracker.Models/
COPY ./TimeTracker.Adapter/TimeTracker.Adapter.csproj ./TimeTracker.Adapter/
COPY ./TimeTracker.Common/TimeTracker.Common.csproj ./TimeTracker.Common/
COPY ./GitLabServices/GitLabServices.csproj ./GitLabServices/
RUN dotnet restore TimeTracker.API/TimeTracker.API.csproj

COPY . .

WORKDIR /app/TimeTracker.API
RUN dotnet build -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app

COPY --from=build /app/build .

ENTRYPOINT ["dotnet", "TimeTracker.API.dll"]