FROM 484054947536.dkr.ecr.us-west-2.amazonaws.com/pse-aspnetcore-build:latest AS build-env 
WORKDIR /app
COPY . ./
RUN dotnet publish -c Release -o out --packages ./packages

# Build runtime image
FROM 484054947536.dkr.ecr.us-west-2.amazonaws.com/pse-aspnetcore-runtime:latest
WORKDIR /app
COPY --chown=debian --from=build-env /app/src/out .
EXPOSE 5000
HEALTHCHECK --interval=30s --timeout=10s --retries=3 cmd curl --fail -s http://localhost:5000/healthcheck || exit 1
ENTRYPOINT ["dotnet", "app.dll"]