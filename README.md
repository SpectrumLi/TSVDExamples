# TSVDExample

## What is this
This is the artifacts of part of open source projects that TSVD can automatically detect thread-safety violations. We will continue to add more example here. For more details, please check out paper "Efficient and Scalable Thread-Safety Violation Detection --- Finding thousands of concurrency bugs during testing" in SOSP2019.

## How to evaluate
TSVD is under open source administration process. We are sorry for the inconvenience. Please contact us for a private link of TSVD. The following instructions only help to evaluate these examples.

### System Requirement
Visual Studio Community Edition, and .Net Framework 4.5-4.7. Because the examples rely on a specific version of .Net, we need them.

### Prepare the binary
Here is the list of current examples:

+ DataTimeExtensions
+ Sequelocity
+ Linq.Dynamic
+ Thunderstruck

We will add more bugs listed in the paper. But these four already demonstrate the main idea of TSVD. For every example:

+ First, open the .sln file by visual studio then compile the project.
+ Second, rebuild the TSVD project under the .sln project.

The TSVD project is created by us, which contains the buggy unit test. To expose the bug, we only need this unit test.

### Instrumentation
Suppose you can access the TSVD already, please download and compile it. The compiled result is TSVDInstrumenter.exe in the TSVDInstrumenter\bin\Debug.
When downloading it, there is be a "Configurations" directory of default configuration. Then open a powershell:

    & [path to TSVDInstrumenter.exe] [path to "TSVD.exe" in the buggy unit test] [path to Configurations\instrumentation-config.cfg] [path to Configurations\runtime-config.cfg]

ins.ps1 is a template. The expect result is

    Instrumentation result: OK

### Run the test
Go to the unit test output directory ([TSVD]\bin\debug) and .\TSVD.exe. 

### Results:
There will be several TSVD-bugs files which contains the racing accesses.
Here are the bugs in the related examples.

+ [DataTimeExtensions](https://github.com/joaomatossilva/DateTimeExtensions/pull/86)
+ [Sequelocity](https://github.com/AmbitEnergyLabs/Sequelocity.NET/pull/23)
+ [Linq.Dynamic](https://github.com/kahanu/System.Linq.Dynamic/pull/48)
+ [Thunderstruck](https://github.com/19WAS85/Thunderstruck/issues/3)
