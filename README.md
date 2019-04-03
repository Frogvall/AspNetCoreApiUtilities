**Not happy about naming and scope. This package is therefore discontinued and moved to [a new repo](https://github.com/Frogvall/aspnetcore-exceptionhandler), where it is divided into several packages and names reflect what the package is actually about.**

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
```cs
        public void ConfigureServices(IServiceCollection services)
        {
          //...

          services.AddExceptionMapper();
          services.AddMvc(options =>
             {
                options.Filters.Add<ApiExceptionFilter>();
             });

          //...
        }

        public void Configure()
        {
           //...

           app.UseApiExceptionHandler();

           //...
        }
```
Create an exeption that inherits BaseApiException ->
```cs
        public class MyException : BaseApiException
```

Create an enum that describes your error codes ->
```cs
        public enum MyErrorEnum
        {
           MyErrorCode = 1337
        }
```

Create one or more exception mapper profiles anywhere in your project. Add mappings in the constructor of the profile ->
```cs
        public class MyMappingProfile : ExceptionMappingProfile<MyErrorEnum>
        {
          public MyMappingProfile()
          {
             AddMapping<MyException>(HttpStatusCode.BadRequest, MyErrorEnum.MyErrorCode);
          }
        }
```
Throw when returning non 2xx ->
```cs
        throw new MyException("Some message.", new { AnyProperty = "AnyValue."});
```
Either add to controller or controller method ->
```cs
        [ValidateModelFilter(ErrorCode = 123)]
```
Or add to the filters of MVC ->
```cs
        services.AddMvc(options =>
           {
               options.Filters.Add(new ValidateModelFilter { ErrorCode = 123 } );
           });
```

Add to controller model (dto) property ->
```cs
        [RequireNonDefault]
```
Add to swagger spec ->
```cs
        options.OperationFilter<ValidateModelOperationFilter>();
        options.OperationFilter<InternalServerErrorOperationFilter>();
```

To consume the api error from another service using this package in an asynchronous context ->
```cs
        var response = await _client.PostAsync(...);
        var error = await response.ParseApiError();
        if (error != null)
        {
            //Handle api error here
        }
        else
        {
            //Handle non-api error here
        }
```

To consume the api error from another service using this package in a synchronous context ->
```cs
        if (response.TryParseApiError(out var error))
        {
            //Handle api error here
        }
        else
        {
            //Handle non-api error here
        }
```

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


