FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TestPlagiarismCA.csproj", "./"]
RUN dotnet restore "TestPlagiarismCA.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "TestPlagiarismCA.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestPlagiarismCA.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestPlagiarismCA.dll"]
