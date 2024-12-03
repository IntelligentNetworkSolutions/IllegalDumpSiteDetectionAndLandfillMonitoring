# User Management - Developer Documentation
## Seed users
> [!NOTE]
> - Change usersSeedDatabase.json file according to the needs of the application
> - Manage application.json file according to the needs of the application
#### Users seeding is invoked during the application's initialization phase
1. A <i>Super admin</i> user is seeded with necessary claims and role assignments.
2. Additional <i>Admin users</i> are created and assigned roles as specified in the configuration.

#### Pre-conditions
1. Configuration Settings: Ensure the following configuration keys are correctly set in the application configuration:
- SeedDatabaseFilePaths:SeedDatabaseUsers - Path to the JSON file containing user data.
- SeedDatabaseDefaultValues:SeedDatabaseDefaultPasswordForAdminUsers - Default password for <i>admin users</i>.
- SeedDatabaseDefaultValues:SeedDatabaseDefaultPasswordForSuperAdmin - Default password for the <i>super admin</i> user.
- SeedDatabaseDefaultValues:SeedDatabaseDefaultSuperAdminUserName - The username for the default <i>super admin</i> user.
- Database Initialization: Ensure that the database schema is initialized and tables for Users, Roles, and UserRoles exist.


#### Implementation Details
1. Validate JSON File Path:
- Retrieve the JSON file path.
- Exit the method if the path is missing or empty.

2. Check for Existing Users:
- If there are already users in the database (_db.Users.Any()), skip seeding.

3. Read and Parse JSON Data:
- Load JSON data from the specified file.
- Extract the <i>super admin</i> and <i>initial users</i> sections.

4. Seed <i>super admin</i>:
- Check if the <i>super admin</i> section exists and has valid data.
- Create the <i>super admin</i> user with a default password.
- Assign the <i>super admin</i> role.
- Add default claims, such as SpecialAuthClaim and PreferedLanguageClaim.

5. Seed Additional Users:
- Check if the <i>initial users</i> section exists and contains user data.
- Create each user with a default password.
- Assign roles specified in the JSON file to the user.

6. Save Changes:
- Persist all user, role, and claim assignments to the database.
```csharp
//shortened code snippet
private void SeedUsers()
{
    if (!_db.Users.Any())
    {
        var usersData = File.ReadAllText(usersPath);
        var jsonArray = JArray.Parse(usersData);
        
        //seed super admin
        if (superadminDataJson != null)
        {
            if (string.IsNullOrEmpty(defaultPasswordSuperAdminUser))
            {
                return;
            }
            
            if (superadmin != null)
            {               
                if (resultCreatedSuperAdmin.Succeeded)
                {
                    // Add claims to super admin                   
                    if (string.IsNullOrEmpty(defaultSuperAdminUserName))
                    {
                        return;
                    }                    
                    if (superAdminUser != null)
                    {                       
                        
                        //Assign super admin to role superadmin
                        if (superAdminRole != null && !string.IsNullOrEmpty(superAdminRole.Name))
                        {
                            _db.UserRoles.Add(new IdentityUserRole<string>
                            {
                                UserId = superAdminUser.Id,
                                RoleId = superAdminRole.Id
                            });
                            _db.SaveChanges();
                        }
                    }
                }
            }
        }
        //seed other users
       ...
    }
}
```
