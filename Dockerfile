FROM microsoft/aspnetcore-build:2.0 AS build-env

RUN build.dat

# Copy everything else and build
COPY . /build

# Build runtime image
FROM microsoft/aspnetcore:2.0
COPY --from=build-env /app/out .
ENTRYPOINT ["run", "run.bat"]