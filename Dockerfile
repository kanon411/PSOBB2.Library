FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /app

# Copy everything else and build
COPY . ./

RUN chmod +x ./build.sh
RUN ./build.sh
COPY ./app/src/Server/Guardians.Service.Authentication/bin/Release/netcoreapp2.0/publish/ ./app/build/auth

# Build runtime image
FROM microsoft/aspnetcore:2.0
COPY --from=build-env /app/build/auth .
ENTRYPOINT ["dotnet", "Guardians.Service.Authentication.dll --usehttps=Certs/TLSCert.pfx --url=http://127.0.0.1:5001"]