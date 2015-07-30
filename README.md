CertiPay.Common
===================

## Description

CertiPay.Common is a basic library that contains utilities and functionality for our internal applications. It
was built for use in our next-generation suite of payroll products for [CertiPay.com](http://www.certipay.com), a payroll
processing engine and service.

## Usage

Please see the [LICENSE](License.md) file for information on using the code in this library.

For the most part, enumerations and classes found in this library are commented and self-explanatory to use.

For any questions or comments, please open an issue in our GitHub Issue tracker for this repository.

## NuGet Package

You can find the packages built for this project on NuGet by searching for `CertiPay` or from the package manager console.

i.e. `Install-Package CertiPay.Common`

## Continuous Integration System

![Build Status](https://ci.appveyor.com/api/projects/status/gvq9mhbakoq2srjq/branch/master?svg=true)

## Contributing

Please submit a pull request or issue to the GitHub repository with the appropriate context and tests, if possible.

## CertiPay.Common.Logging

CertiPay.Common.Logging provides an abstraction similar to `Common.Logging`. By default it logs to `C:\Logs\{Environment}\{Application}\{Data}.log`, but can be configured.

Under the covers, it uses Serilog for it's logging. To enable additional sinks, use the app.config like below:

```
    <add key="serilog:using" value="Serilog.Sinks.Email" />
    <add key="serilog:write-to:Email.toEmail" value="Errors@CertiPay.com" />
    <add key="serilog:write-to:Email.fromEmail" value="Errors@CertiPay.com" />
    <!-- An empty value here will drop back to using system.net.mailSettings.smtp.network.host -->
    <!-- However, not including this key will cause it to fail to send altogether -->
    <add key="serilog:write-to:Email.mailServer" value="" />
```

## CertiPay.PDF

CertiPay.PDF takes a dependency on ABCPDF.Gecko which will end up copying a lot of files into your project.

Instead, you can remove them and reference them directly from the package folder is your project file.

```
<Content Include="..\packages\ABCpdf.ABCGecko.10.0.0.0\content\XULRunner21_0\**\*.*">
    <Link>XULRunner21_0\%(RecursiveDir)\%(FileName)%(Extension)</Link>
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
</Content>
```