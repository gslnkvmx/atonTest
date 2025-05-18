FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["atonTest/atonTest.csproj", "atonTest/"]
RUN dotnet restore "atonTest/atonTest.csproj"
COPY . .
WORKDIR "/src/atonTest"
RUN dotnet build "atonTest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "atonTest.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "atonTest.dll"]
