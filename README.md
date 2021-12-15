# Facility ASP.NET Support

[ASP.NET support](https://facilityapi.github.io/generate/aspnet) for the [Facility API Framework](https://facilityapi.github.io/).

[![Build](https://github.com/FacilityApi/FacilityAspNet/workflows/Build/badge.svg)](https://github.com/FacilityApi/FacilityAspNet/actions?query=workflow%3ABuild)

Name | Description | NuGet
--- | --- | ---
Facility.AspNetCore | Helpers for using Facility with ASP.NET Core. | [![NuGet](https://img.shields.io/nuget/v/Facility.AspNetCore.svg)](https://www.nuget.org/packages/Facility.AspNetCore)
fsdgenaspnet | A tool that generates an ASP.NET controller for a Facility Service Definition. | [![NuGet](https://img.shields.io/nuget/v/fsdgenaspnet.svg)](https://www.nuget.org/packages/fsdgenaspnet)
Facility.CodeGen.AspNet | A library that generates an ASP.NET controller for a Facility Service Definition. | [![NuGet](https://img.shields.io/nuget/v/Facility.CodeGen.AspNet.svg)](https://www.nuget.org/packages/Facility.CodeGen.AspNet)

## Documentation

[Documentation](https://facilityapi.github.io/) | [Release Notes](ReleaseNotes.md) | [Contributing](CONTRIBUTING.md)

## Conformance

To run conformance tests, first start one of the conformance servers:

```powershell
dotnet run --project .\conformance\CoreControllerServer
dotnet run --project .\conformance\CoreMiddlewareServer
dotnet run --project .\conformance\WebApiControllerServer
dotnet run --project .\conformance\WebApiMiddlewareServer
```

Then run the conformance tool against the running service.

```powershell
dotnet FacilityConformance test
```
