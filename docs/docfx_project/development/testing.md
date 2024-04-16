# Testing  

## Code Coverage  

- run settings file  
  - ExcludeCodeCoverage.runsettings.xml  
    - Exclude Migrations folder  
- test command:  
  - `dotnet test --settings ExcludeCodeCoverage.runsettings.xml --collect:"XPlat Code Coverage"`  
  - reportgenerator -reports:".Tests/TestResults/guid-from-previous-Comand/coverage.xml" -targetdir:"./Tests/TestResults/guid-from-previous-Comand" -reportypes:Html
