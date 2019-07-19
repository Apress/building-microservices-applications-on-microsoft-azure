FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY ["employee/employee.csproj", "employee/"]

RUN dotnet restore "employee/employee.csproj"
COPY . .
WORKDIR "/src/employee"
RUN dotnet build "employee.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "employee.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "employee.dll"]