# AspNetCoreApiUtilities

[![CircleCI](https://circleci.com/gh/schwamster/HttpService.svg?style=shield&circle-token)](https://circleci.com/gh/Frogvall/AspNetCoreApiUtilities)
![#](https://img.shields.io/nuget/v/Frogvall.AspNetCore.ApiUtilities.svg)

AspNetCore Api Utilities for asp.net core that include things like an Exception Handler middleware, modelstate validation by attribute, RequireNonDefault attribute for controller models, and swagger operation filters for 400 and 500.

## Getting started

### Install the package
Install the nuget package from [nuget](https://www.nuget.org/packages/Frogvall.AspNetCore.ApiUtilities/)

Either add it with the PM-Console:

        Install-Package Frogvall.AspNetCore.ApiUtilities

Or add it to csproj file ->
```xml
        <ItemGroup>
                ...
                <PackageReference Include="Frogvall.AspNetCore.ApiUtilities" Version="x.y.z" />
                ...
        </ItemGroup>
```
### Using the utilites

Edit your Startup.cs ->

        ConfigureServices(IServiceCollection services)
        {
          ...

          services.AddExceptionMapper();

          ...
        }

        Configure()
        {
           ...

           app.UseApiExceptionHandler();

           ...
        }

Create an exeption that inherits BaseApiException ->

        public class MyException : BaseApiException

Create one or more exception mapper profiles anywhere in your project. Add mappings in the constructor of the profile ->

        public class MyMappingProfile : ExceptionMappingProfile
        {
          public MyMappingProfile()
          {
             AddMapping<MyException>(ExceptionReturnType.Error, 1337);
          }
        }

Throw when returning non 2xx ->

        throw new MyException("Some message.", new { AnyProperty = "AnyValue."});

Add to controller method ->1

        [ValidateModel(ErrorCode = 123)]

Add to controller model (dto) property ->

        [RequireNonDefault]

Add to swagger spec ->

        options.OperationFilter<ValidateModelOperationFilter>();
        options.OperationFilter<InternalServerErrorOperationFilter>();

## Build and Publish

### Prequisites

* docker, docker-compose
* dotnet core 2.0 sdk  [download core](https://www.microsoft.com/net/core)

The package is build in docker so you will need to install docker to build and publish the package.
(Of course you could just build it on the machine you are running on and publish it from there.
I prefer to build and publish from docker images to have a reliable environment, plus make it easier
to build this on circleci).

### build

run:
        docker-compose -f docker-compose-build.yml up

this will build & test the code. The testresult will be in folder ./testresults and the package in ./package

! *if you clone the project on a windows machine you will have to change the file endings on the build-container.sh to LF*

### publish

run: (fill in the api key):

        docker run --rm -v ${PWD}/package:/data/package schwamster/nuget-docker push /data/package/*.nupkg <your nuget api key> -Source nuget.org

this will take the package from ./package and push it to nuget.org

### build on circleci

The project contains a working circle ci yml file. All you have to do is to configure the Nuget Api Key in the build projects environment variables on circleci (Nuget_Api_Key)


