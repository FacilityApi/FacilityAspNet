# Release Notes

## 3.0.0

* **Breaking:** Don't catch exceptions from `UseFacilityHttpHandler` (`FacilityAspNetCoreMiddleware`).
* **Breaking:** Don't handle exceptions in `FacilityActionFilter`.
* Support `UseFacilityExceptionHandler`, which returns the expected JSON for the error, only including possibly insecure error details if requested. It calls `UseExceptionHandler` to leverage standard exception handling logic, including logging.

## 2.1.1

* Support scoped dependencies with `UseFacilityHttpHandler`.

## 2.1.0

* Update `Facility.Definition`. (Supports shorthand for required attribute, e.g. `string!`.)

## 2.0.2

* Depend on `Microsoft.AspNetCore.Mvc.Core` instead of `Microsoft.AspNetCore.Mvc`.

## 2.0.1

* Upgrade to .NET Standard 2.0. Upgrade NuGet dependencies.
* Convert `fsdgenaspnet` to a .NET Core Global Tool.
* Support static `AspNetGenerator.GenerateAspNet` for C# build scripts.
* Leverage `FacilityConformance` tool.

## 1.2.0

* Start tracking version history.
