FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /app

# Copy everything else and build
COPY . ./build

RUN ./build.sh

# Build runtime image
FROM microsoft/aspnetcore:2.0
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "./build/auth/Guardians.Service.Authentication.dll --usehttps=Certs/TLSCert.pfx --url=http://127.0.0.1:5001"]