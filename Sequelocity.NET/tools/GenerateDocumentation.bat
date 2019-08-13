:: Generates project documentation

:: Create the temporary directory
set tempDirectory="temp"
mkdir %tempDirectory%

:: Copy the required files to the temporary directory
copy ImmDoc.NET\ImmDocNet.exe %tempDirectory%
copy ..\src\SequelocityDotNet\bin\Debug\SequelocityDotNet.dll %tempDirectory%
copy ..\src\SequelocityDotNet\bin\Debug\SequelocityDotNet.XML %tempDirectory%

:: Generate the documentation
cd %tempDirectory%
ImmDocNet.exe

:: Copy the documentation to the "docs" folder under the project root
robocopy "doc" "..\..\docs" /mir

:: Delete the temporary directory
cd ..\
rd %tempDirectory% /s /q
