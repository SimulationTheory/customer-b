FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /app
COPY . ./
RUN dotnet publish -c Release -o out --packages ./packages

# Build runtime image
FROM microsoft/aspnetcore:2.0
WORKDIR /app
COPY --from=build-env /app/src/out .
EXPOSE 5000
ENTRYPOINT ["dotnet", "app.dll"]
