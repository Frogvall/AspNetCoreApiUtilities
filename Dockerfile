FROM microsoft/dotnet:2.0-sdk
RUN mkdir app
WORKDIR /app

COPY *.sln .
COPY ./src/AspNetCoreApiUtilities/AspNetCoreApiUtilities.csproj /app/src/AspNetCoreApiUtilities/AspNetCoreApiUtilities.csproj
COPY ./test/AspNetCoreApiUtilities.Tests/AspNetCoreApiUtilities.Tests.csproj /app/test/AspNetCoreApiUtilities.Tests/AspNetCoreApiUtilities.Tests.csproj

RUN dotnet restore

COPY . .

RUN ["sh", "build-container.sh"]

