# Release Notes

## 3.6.0-beta.1

* Support `extern` data and enum types.

## 3.5.0

* Update dependencies.
* Add .NET 7; remove .NET Core 3.1 and .NET 5.

## 3.4.0

* Support nullable fields.

## 3.3.3

* Fix build that didn't work properly on .NET 5 or .NET Core 3.1.

## 3.3.0

* Update Facility.

## 3.2.0

* Support .NET 6.
* Update dependencies.

## 3.1.0

* Support .NET 5.
* Update dependencies.

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
