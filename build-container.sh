#!bin/bash
set -e
cd test/AspNetCoreApiUtilities.Test
dotnet restore
dotnet xunit -xml ${pwd}/../../testresults/out.xml
cd -
dotnet pack src/AspNetCoreApiUtilities/AspNetCoreApiUtilities.csproj -c release -o ${pwd}/package 