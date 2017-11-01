FROM microsoft/dotnet:2.0-sdk
RUN mkdir app
WORKDIR /app

COPY *.sln .
COPY ./src/AspNetCoreApiUtilities/AspNetCoreApiUtilities.csproj /app/src/AspNetCoreApiUtilities/AspNetCoreApiUtilities.csproj
COPY ./test/AspNetCoreApiUtilities.Test/AspNetCoreApiUtilities.Tests.csproj /app/test/AspNetCoreApiUtilities.Test/AspNetCoreApiUtilities.Tests.csproj

RUN dotnet restore

COPY . .

RUN ["sh", "build-container.sh"]

