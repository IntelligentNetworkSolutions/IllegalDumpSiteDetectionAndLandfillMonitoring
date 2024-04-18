# Testing  

- _Refactor at your discretion when needed_  

## XUnit

- Arrange  

- Act  

- Assert  

## Moq  

## _Coverlet_ Code Coverage  

- run settings file  
  - ExcludeCodeCoverage.runsettings.xml  
    - Exclude Migrations folder  
    - Exclude Infratructure folder for now  
    - Exclude Views _(cshtml syntax is appearing as untested)_
- test command:  
  - `dotnet test --settings ExcludeCodeCoverage.runsettings.xml --collect:"XPlat Code Coverage"`  
  - reportgenerator -reports:".Tests/TestResults/guid-from-previous-Comand/coverage.xml" -targetdir:"./Tests/TestResults/guid-from-previous-Comand" -reportypes:Html

## CI  

- GitHub Actions
  - .NET-test workflow  

## Thoghts  

- A few_(2-3)_ "Unit" tests might actually be integration tests.  
- A few_(4-5)_ "Unit" tests might not be completelly isolated.  
