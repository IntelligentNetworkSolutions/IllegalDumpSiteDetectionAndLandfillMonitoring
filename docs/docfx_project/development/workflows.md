# Workflows  

- The project includes GitHub Actions workflows  

## Build  

- Automated Build on multiple OS's  
  - Windows  
  - MacOS  
  - _Linux build is covered in the Testing workflow_  
- [workflow](https://github.com/IntelligentNetworkSolutions/IllegalDumpSiteDetectionAndLandfillMonitoring/actions/workflows/dotnet-build.yml)  

## Documentation  

- Automated documentation generation published to [GitHub Pages](https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/index.html)  
- Markdown Documentation  
- Documentation Builder [Docfx](https://github.com/dotnet/docfx)  
- [workflow](https://github.com/IntelligentNetworkSolutions/IllegalDumpSiteDetectionAndLandfillMonitoring/actions/workflows/Docs-Auto-Docfx.yml)  

## Testing

- Automated unit and integration testing  
- Code coverage reporting  
- Linux only workflow (_required for PostgreSql image_)  
- [workflow](https://github.com/IntelligentNetworkSolutions/IllegalDumpSiteDetectionAndLandfillMonitoring/actions/workflows/test_unit_n_integration_gh.yml)  

## Deployment  

- Self Hosted Runner  
- Install requirements, restore dependencies and publish to virtual enviroment  
- [workflow](https://github.com/IntelligentNetworkSolutions/IllegalDumpSiteDetectionAndLandfillMonitoring/actions/workflows/test_unit_n_integration_gh.yml)  

## Code Security  

- CodeQL  
  - C#  
  - Javascript / Typescipt  
- [workflow](https://github.com/IntelligentNetworkSolutions/IllegalDumpSiteDetectionAndLandfillMonitoring/actions/workflows/test_unit_n_integration_gh.yml)  
