# TSVDExample

## What is this
This is a part of opensouce projects that TSVD can automatically detect thread-safety violations. We will continue to add more example here. For more details, please check out paper "Efficient and Scalable Thread-Safety Violation Detection --- Finding thousands of concurrency bugs during testing" in SOSP2019.

## How to use
For the general manual of TSVD, please reference to the TSVD github page. The following instructions only help to evaluate these examples.

### System Requirement
Visual Studio Community Edition, and .Net Farmework 3.5-4.7. Because the examples rely on a specific version of .Net, we need them.

### Prepare the binary
Here is the list of current examples:

+ DataTimeExtensions
+ Sequelocity
+ Linq.Dynamic
+ Thunderstruck

We will add more bugs explained in the paper. But these four already demostrate the main idea of TSVD. For every example:

+ First, open the .sln file by visual studio then compile the project.
+ Second, rebuild the TSVD project under the .sln project.

The TSVD project is created by us, which contains the buggy unit test. To expose the bug, we only need this unit test.

### Instrucmentation
Supppose you can access the TSVD already, please download and compile it. The compiled result is TSVDInstrumenter.exe in the TSVDInstrumenter\bin\Debug.
When downloading it, there is be a "Configurations" directory of default configuration. Then open a powershell:

    & [path to TSVDInstrumenter.exe] [path to "TSVD.exe" in the buggy unit test] [path to Configurations\instrumentation-config.cfg] [path to Configurations\runtime-config.cfg]

ins.ps1 is a template. The expect result is

    Instrumetation result: OK


### Run the test
Go to the unit test output directory ([TSVD]\bin\debug) and .\TSVD.exe. 

### Results:
Here are the bugs in the related examples.

+ [DataTimeExtensions](https://github.com/joaomatossilva/DateTimeExtensions/pull/86)
+ [Sequelocity](https://github.com/AmbitEnergyLabs/Sequelocity.NET/pull/23)
+ [Linq.Dynamic](https://github.com/kahanu/System.Linq.Dynamic/pull/48)
+ [Thunderstruck](https://github.com/19WAS85/Thunderstruck/issues/3)