# SHHhhhh Secrets  

## Development  

### [Secret Manager](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=windows)

#### About the Secret Manager  

- The Secret Manager tool stores sensitive data during the development of an ASP.NET Core project.  
  - In this context, a piece of sensitive data is an app secret.  
- __App secrets are stored in a separate location from the project tree.__  
- The app secrets are associated with a specific project or shared across several projects.  
- __The app secrets aren't checked into source control.__  
- The Secret Manager tool doesn't encrypt the stored secrets and shouldn't be treated as a trusted store.  
- __It's for development purposes only.__  
- The keys and values are stored in a JSON configuration file in the user profile directory.  

#### Enable Secret Storage

1. .NET CLI Command:  
    - ```dotnet user-secrets init```  
      - __The preceding command adds a UserSecretsId element within a PropertyGroup of the project file.__  
      - _By default, the inner text of UserSecretsId is a GUID. The inner text is arbitrary, but is unique to the project._  
2. Visual Studio UI:  
    - ```In Visual Studio, right-click the project in Solution Explorer, and select Manage User Secrets from the context menu.```  
      - __This gesture adds a UserSecretsId element, populated with a GUID, to the project file.__  

#### Set a Secret  

- Define an app secret consisting of a key and its value.  
- The secret is associated with the project's UserSecretsId value.

1. .NET CLI Command:  
    - ```dotnet user-secrets set "Movies:ServiceApiKey" "12345"```  
        - The Secret Manager tool can be used from other directories too.  
    - Use the --project option to supply the file system path at which the project file exists.  
        - ```dotnet user-secrets set "Movies:ServiceApiKey" "12345" --project "C:\apps\WebApp1\src\WebApp1"```  
    - Set multiple secrets
        - A batch of secrets can be set by piping JSON to the set command.  
        - ```type .\input.json | dotnet user-secrets set```  
2. Visual Studio UI:  
    - ```Visual Studio's Manage User Secrets gesture opens a secrets.json file in the text editor.```  
    - Replace / Edit the contents of secrets.json with the key-value pairs to be stored.  
    - _The JSON structure is flattened after modifications via dotnet user-secrets remove or dotnet user-secrets set_  

#### Access a secret

1. Register the user secrets configuration source (just create a new project )  
    - The user secrets configuration provider registers the appropriate configuration source with the .NET Configuration API.
    - ```var builder = WebApplication.CreateBuilder(args);```  
    - WebApplication.CreateBuilder initializes a new instance of the WebApplicationBuilder class with preconfigured defaults.  
    - The initialized WebApplicationBuilder (builder) provides __default configuration__ and calls AddUserSecrets when the __EnvironmentName is Development__  
2. Read the secret via the Configuration API  
    - ```builder.Configuration["Movies:ServiceApiKey"];```  
    - ```var moviesApiKey = _config["Movies:ServiceApiKey"];```  

#### List the secrets

1. .NET CLI Command:  
    - ```dotnet user-secrets list```  
    - from the directory in which the project file exists  

2. Visual Studio UI:  
    - ```In Visual Studio, right-click the project in Solution Explorer, and select Manage User Secrets from the context menu.```  
      - __This gesture loads the secrets.json file.__  

#### Remove a single secret  

1. .NET CLI Command:  
    - ```dotnet user-secrets remove "Movies:ConnectionString"```  
    - from the directory in which the project file exists  

2. Visual Studio UI:  
    - __Delete the key value pair__ you want to remove from the __secrets.json__ file, keeping the JSON structure valid.  

#### Remove all secrets  

1. .NET CLI Command:  
    - ```dotnet user-secrets clear```  
    - from the directory in which the project file exists  

2. Visual Studio UI:  
    - __Modify secrets.json__ to look like ```{}.```  

### Development set up on IIS  

- The Secrets Manager creates and updates the secrets/json file in the logged in user's directory.  
- When the app is set up on IIS the code tries to find the secrets.json file in the directory of the running Application Pool User.  

1. After each update to the secrets.json file the following commands must be run in Powershell.
    1. cd C:\Users  
    2. cd [AppPoolName]  
    3. cd AppData  
    4. cd Roaming  
    5. cd Microsoft  
    6. _mkdir UserSecrets_  
        - _only for First time Set Up_  
    7. cd .\UserSecrets\  
    8. _mkdir [ProjectId]_  
        - _only for First time Set Up_  
    9. cd .\\[ProjectId]\  
    10. Copy-Item -Path "C:\Users\\[LoggedInUserName]\AppData\Roaming\Microsoft\UserSecrets\\[ProjectId]\secrets.json" -Destination "C:\Users\\[AppPoolName]\AppData\Roaming\Microsoft\UserSecrets\\[ProjectId]" -Force

## Production  

### Enviroment Variables  

### Production set up on IIS  

- If __Enviroment variables__ are set for a user and not the whole system, the user must be the __user from the IIS Application Pool__.
