FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY . ./
RUN dotnet restore

RUN dotnet ef migrations add $(date '+%d%m%Y%H') --startup-project AP.Repositories --project AP.Repositories

RUN dotnet ef database update --startup-project AP.Repositories --project AP.Repositories

# Copy everything else and build
RUN dotnet publish AP.Web -c Release -o out

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/AP.Web/out .
ENTRYPOINT ["dotnet", "AP.Web.dll"]
