FROM microsoft/aspnetcore-build:2.0 AS build-env

RUN /build.sh

# Copy everything else and build
COPY . /build

# Build runtime image
FROM microsoft/aspnetcore:2.0
COPY --from=build-env /out .
ENTRYPOINT ["dotnet", "/build/auth/Guardians.Service.Authentication.dll --usehttps=Certs/TLSCert.pfx --url=http://127.0.0.1:5001"]