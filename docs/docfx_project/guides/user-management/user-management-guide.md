# User Management  

In our web application, user management is how we control who can do what inside the app.  

### What Are Users?  

A user is simply anyone who has an account in the app.  
Every user has certain details, like their name, email, and role.  
These details are stored as claims, which help the system understand what the user can access or do.  
For example, a user might be able to view documents but not edit them, or they might have full access to manage everything in the app.

| Username   | Email   | Role   | Enabled   |   Action  |
|:------------:|:------------:|:------------:|:------------:|:------------:|
|username1|username1@yahoo.com|SuperAdmin|True|<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512" height="18" width=18 style=" fill: #6e7075"><path d="M471.6 21.7c-21.9-21.9-57.3-21.9-79.2 0L362.3 51.7l97.9 97.9 30.1-30.1c21.9-21.9 21.9-57.3 0-79.2L471.6 21.7zm-299.2 220c-6.1 6.1-10.8 13.6-13.5 21.9l-29.6 88.8c-2.9 8.6-.6 18.1 5.8 24.6s15.9 8.7 24.6 5.8l88.8-29.6c8.2-2.7 15.7-7.4 21.9-13.5L437.7 172.3 339.7 74.3 172.4 241.7zM96 64C43 64 0 107 0 160L0 416c0 53 43 96 96 96l256 0c53 0 96-43 96-96l0-96c0-17.7-14.3-32-32-32s-32 14.3-32 32l0 96c0 17.7-14.3 32-32 32L96 448c-17.7 0-32-14.3-32-32l0-256c0-17.7 14.3-32 32-32l96 0c17.7 0 32-14.3 32-32s-14.3-32-32-32L96 64z"/></svg>  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512" height="18" width=18 style=" fill: #6e7075"><path d="M135.2 17.7C140.6 6.8 151.7 0 163.8 0L284.2 0c12.1 0 23.2 6.8 28.6 17.7L320 32l96 0c17.7 0 32 14.3 32 32s-14.3 32-32 32L32 96C14.3 96 0 81.7 0 64S14.3 32 32 32l96 0 7.2-14.3zM32 128l384 0 0 320c0 35.3-28.7 64-64 64L96 512c-35.3 0-64-28.7-64-64l0-320zm96 64c-8.8 0-16 7.2-16 16l0 224c0 8.8 7.2 16 16 16s16-7.2 16-16l0-224c0-8.8-7.2-16-16-16zm96 0c-8.8 0-16 7.2-16 16l0 224c0 8.8 7.2 16 16 16s16-7.2 16-16l0-224c0-8.8-7.2-16-16-16zm96 0c-8.8 0-16 7.2-16 16l0 224c0 8.8 7.2 16 16 16s16-7.2 16-16l0-224c0-8.8-7.2-16-16-16z"/></svg> <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 576 512" height="18" width=18 style=" fill: #6e7075"><path d="M249.6 471.5c10.8 3.8 22.4-4.1 22.4-15.5l0-377.4c0-4.2-1.6-8.4-5-11C247.4 52 202.4 32 144 32C93.5 32 46.3 45.3 18.1 56.1C6.8 60.5 0 71.7 0 83.8L0 454.1c0 11.9 12.8 20.2 24.1 16.5C55.6 460.1 105.5 448 144 448c33.9 0 79 14 105.6 23.5zm76.8 0C353 462 398.1 448 432 448c38.5 0 88.4 12.1 119.9 22.6c11.3 3.8 24.1-4.6 24.1-16.5l0-370.3c0-12.1-6.8-23.3-18.1-27.6C529.7 45.3 482.5 32 432 32c-58.4 0-103.4 20-123 35.6c-3.3 2.6-5 6.8-5 11L304 456c0 11.4 11.7 19.3 22.4 15.5z"/></svg>  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512" height="18" width=18 style=" fill: #6e7075"><path d="M224 256A128 128 0 1 0 224 0a128 128 0 1 0 0 256zm-45.7 48C79.8 304 0 383.8 0 482.3C0 498.7 13.3 512 29.7 512l388.6 0c16.4 0 29.7-13.3 29.7-29.7C448 383.8 368.2 304 269.7 304l-91.4 0z"/></svg>|
|username2|username2@gmail.com|Viewer|False|<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512" height="18" width=18 style=" fill: #6e7075"><path d="M471.6 21.7c-21.9-21.9-57.3-21.9-79.2 0L362.3 51.7l97.9 97.9 30.1-30.1c21.9-21.9 21.9-57.3 0-79.2L471.6 21.7zm-299.2 220c-6.1 6.1-10.8 13.6-13.5 21.9l-29.6 88.8c-2.9 8.6-.6 18.1 5.8 24.6s15.9 8.7 24.6 5.8l88.8-29.6c8.2-2.7 15.7-7.4 21.9-13.5L437.7 172.3 339.7 74.3 172.4 241.7zM96 64C43 64 0 107 0 160L0 416c0 53 43 96 96 96l256 0c53 0 96-43 96-96l0-96c0-17.7-14.3-32-32-32s-32 14.3-32 32l0 96c0 17.7-14.3 32-32 32L96 448c-17.7 0-32-14.3-32-32l0-256c0-17.7 14.3-32 32-32l96 0c17.7 0 32-14.3 32-32s-14.3-32-32-32L96 64z"/></svg>  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512" height="18" width=18 style=" fill: #6e7075"><path d="M135.2 17.7C140.6 6.8 151.7 0 163.8 0L284.2 0c12.1 0 23.2 6.8 28.6 17.7L320 32l96 0c17.7 0 32 14.3 32 32s-14.3 32-32 32L32 96C14.3 96 0 81.7 0 64S14.3 32 32 32l96 0 7.2-14.3zM32 128l384 0 0 320c0 35.3-28.7 64-64 64L96 512c-35.3 0-64-28.7-64-64l0-320zm96 64c-8.8 0-16 7.2-16 16l0 224c0 8.8 7.2 16 16 16s16-7.2 16-16l0-224c0-8.8-7.2-16-16-16zm96 0c-8.8 0-16 7.2-16 16l0 224c0 8.8 7.2 16 16 16s16-7.2 16-16l0-224c0-8.8-7.2-16-16-16zm96 0c-8.8 0-16 7.2-16 16l0 224c0 8.8 7.2 16 16 16s16-7.2 16-16l0-224c0-8.8-7.2-16-16-16z"/></svg> <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 576 512" height="18" width=18 style=" fill: #6e7075"><path d="M249.6 471.5c10.8 3.8 22.4-4.1 22.4-15.5l0-377.4c0-4.2-1.6-8.4-5-11C247.4 52 202.4 32 144 32C93.5 32 46.3 45.3 18.1 56.1C6.8 60.5 0 71.7 0 83.8L0 454.1c0 11.9 12.8 20.2 24.1 16.5C55.6 460.1 105.5 448 144 448c33.9 0 79 14 105.6 23.5zm76.8 0C353 462 398.1 448 432 448c38.5 0 88.4 12.1 119.9 22.6c11.3 3.8 24.1-4.6 24.1-16.5l0-370.3c0-12.1-6.8-23.3-18.1-27.6C529.7 45.3 482.5 32 432 32c-58.4 0-103.4 20-123 35.6c-3.3 2.6-5 6.8-5 11L304 456c0 11.4 11.7 19.3 22.4 15.5z"/></svg>  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512" height="18" width=18 style=" fill: #6e7075"><path d="M224 256A128 128 0 1 0 224 0a128 128 0 1 0 0 256zm-45.7 48C79.8 304 0 383.8 0 482.3C0 498.7 13.3 512 29.7 512l388.6 0c16.4 0 29.7-13.3 29.7-29.7C448 383.8 368.2 304 269.7 304l-91.4 0z"/></svg>

### What Are Claims?  

Authorization is based on claims which are information about each user, role, permissions, and other details.  
It helps the system know what each person should be able to see or do.  
Roles as well as special user claims are attached to users, the system uses them to control what features each user can access.  
When you add or edit a user or role, the system updates the claims associated with each.  

<i>
For example:
When a user is assigned a role like Admin, the system adds a claim to their account saying, “Role: Admin.”  
This claim allows the system to recognize that this user has special permissions to manage the app.
When a user is assigned the Editor role, their claims will reflect the permissions they have, such as being able to edit content.
These claims are checked whenever a user interacts with the app.  
If their claims give them permission to access a certain feature, they can use it.  
If not, the system will block access, ensuring that users only see and do what they are allowed to._
</i>

### What Are Roles?  

A role defines what kind of user you are.  
For example, a user might be assigned one of these roles  

#### Admin  

Has full control over the app, including managing other users and changing settings.

#### Editor  

Can create, edit, or delete content, but can't manage users.

#### Viewer  

Can only see content, but can't make changes.

| Role name   | Actions             |
|:-------------:|:---------------------:|
| Admin       | <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512" height="18" width=18 style=" fill: #6e7075"><path d="M471.6 21.7c-21.9-21.9-57.3-21.9-79.2 0L362.3 51.7l97.9 97.9 30.1-30.1c21.9-21.9 21.9-57.3 0-79.2L471.6 21.7zm-299.2 220c-6.1 6.1-10.8 13.6-13.5 21.9l-29.6 88.8c-2.9 8.6-.6 18.1 5.8 24.6s15.9 8.7 24.6 5.8l88.8-29.6c8.2-2.7 15.7-7.4 21.9-13.5L437.7 172.3 339.7 74.3 172.4 241.7zM96 64C43 64 0 107 0 160L0 416c0 53 43 96 96 96l256 0c53 0 96-43 96-96l0-96c0-17.7-14.3-32-32-32s-32 14.3-32 32l0 96c0 17.7-14.3 32-32 32L96 448c-17.7 0-32-14.3-32-32l0-256c0-17.7 14.3-32 32-32l96 0c17.7 0 32-14.3 32-32s-14.3-32-32-32L96 64z"/></svg>  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512" height="18" width=18 style=" fill: #6e7075"><path d="M135.2 17.7C140.6 6.8 151.7 0 163.8 0L284.2 0c12.1 0 23.2 6.8 28.6 17.7L320 32l96 0c17.7 0 32 14.3 32 32s-14.3 32-32 32L32 96C14.3 96 0 81.7 0 64S14.3 32 32 32l96 0 7.2-14.3zM32 128l384 0 0 320c0 35.3-28.7 64-64 64L96 512c-35.3 0-64-28.7-64-64l0-320zm96 64c-8.8 0-16 7.2-16 16l0 224c0 8.8 7.2 16 16 16s16-7.2 16-16l0-224c0-8.8-7.2-16-16-16zm96 0c-8.8 0-16 7.2-16 16l0 224c0 8.8 7.2 16 16 16s16-7.2 16-16l0-224c0-8.8-7.2-16-16-16zm96 0c-8.8 0-16 7.2-16 16l0 224c0 8.8 7.2 16 16 16s16-7.2 16-16l0-224c0-8.8-7.2-16-16-16z"/></svg> <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 576 512" height="18" width=18 style=" fill: #6e7075"><path d="M249.6 471.5c10.8 3.8 22.4-4.1 22.4-15.5l0-377.4c0-4.2-1.6-8.4-5-11C247.4 52 202.4 32 144 32C93.5 32 46.3 45.3 18.1 56.1C6.8 60.5 0 71.7 0 83.8L0 454.1c0 11.9 12.8 20.2 24.1 16.5C55.6 460.1 105.5 448 144 448c33.9 0 79 14 105.6 23.5zm76.8 0C353 462 398.1 448 432 448c38.5 0 88.4 12.1 119.9 22.6c11.3 3.8 24.1-4.6 24.1-16.5l0-370.3c0-12.1-6.8-23.3-18.1-27.6C529.7 45.3 482.5 32 432 32c-58.4 0-103.4 20-123 35.6c-3.3 2.6-5 6.8-5 11L304 456c0 11.4 11.7 19.3 22.4 15.5z"/></svg>        |
| Editor  |<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512" height="18" width=18 style=" fill: #6e7075"><path d="M471.6 21.7c-21.9-21.9-57.3-21.9-79.2 0L362.3 51.7l97.9 97.9 30.1-30.1c21.9-21.9 21.9-57.3 0-79.2L471.6 21.7zm-299.2 220c-6.1 6.1-10.8 13.6-13.5 21.9l-29.6 88.8c-2.9 8.6-.6 18.1 5.8 24.6s15.9 8.7 24.6 5.8l88.8-29.6c8.2-2.7 15.7-7.4 21.9-13.5L437.7 172.3 339.7 74.3 172.4 241.7zM96 64C43 64 0 107 0 160L0 416c0 53 43 96 96 96l256 0c53 0 96-43 96-96l0-96c0-17.7-14.3-32-32-32s-32 14.3-32 32l0 96c0 17.7-14.3 32-32 32L96 448c-17.7 0-32-14.3-32-32l0-256c0-17.7 14.3-32 32-32l96 0c17.7 0 32-14.3 32-32s-14.3-32-32-32L96 64z"/></svg>  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512" height="18" width=18 style=" fill: #6e7075"><path d="M135.2 17.7C140.6 6.8 151.7 0 163.8 0L284.2 0c12.1 0 23.2 6.8 28.6 17.7L320 32l96 0c17.7 0 32 14.3 32 32s-14.3 32-32 32L32 96C14.3 96 0 81.7 0 64S14.3 32 32 32l96 0 7.2-14.3zM32 128l384 0 0 320c0 35.3-28.7 64-64 64L96 512c-35.3 0-64-28.7-64-64l0-320zm96 64c-8.8 0-16 7.2-16 16l0 224c0 8.8 7.2 16 16 16s16-7.2 16-16l0-224c0-8.8-7.2-16-16-16zm96 0c-8.8 0-16 7.2-16 16l0 224c0 8.8 7.2 16 16 16s16-7.2 16-16l0-224c0-8.8-7.2-16-16-16zm96 0c-8.8 0-16 7.2-16 16l0 224c0 8.8 7.2 16 16 16s16-7.2 16-16l0-224c0-8.8-7.2-16-16-16z"/></svg> <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 576 512" height="18" width=18 style=" fill: #6e7075"><path d="M249.6 471.5c10.8 3.8 22.4-4.1 22.4-15.5l0-377.4c0-4.2-1.6-8.4-5-11C247.4 52 202.4 32 144 32C93.5 32 46.3 45.3 18.1 56.1C6.8 60.5 0 71.7 0 83.8L0 454.1c0 11.9 12.8 20.2 24.1 16.5C55.6 460.1 105.5 448 144 448c33.9 0 79 14 105.6 23.5zm76.8 0C353 462 398.1 448 432 448c38.5 0 88.4 12.1 119.9 22.6c11.3 3.8 24.1-4.6 24.1-16.5l0-370.3c0-12.1-6.8-23.3-18.1-27.6C529.7 45.3 482.5 32 432 32c-58.4 0-103.4 20-123 35.6c-3.3 2.6-5 6.8-5 11L304 456c0 11.4 11.7 19.3 22.4 15.5z"/></svg> 

### How Can You Manage Users and Roles?  

In the web app, administrators (or users with special permissions) can manage both users and roles.  
This means they can add new users, assign roles to them, edit their details, or even delete them if needed.

### Adding a New User  

To add a new user, you can click the "Add User" button.  
This will allow you to create an account for someone who needs access to the app.  
You’ll need to enter basic information, like their name, email, and the role you want to assign to them.  
Once added, this user will have access to the app according to the permissions tied to their role.

### Adding a New Role  

Roles are essential for determining what users can and can’t do.  
To create a new role, you can click the "Add Role" button.  
When you add a role, you’ll define what permissions that role has — such as whether they can edit content, manage users, or just view information.  
After creating a new role, you can assign it to any user, depending on their responsibilities.  

### Editing a User or Role  

If you need to change a user’s details or update their role, you can click the "Edit" button next to their name.  
This will allow you to modify their information (like their name, email, or assigned role) or change their permissions.

- _For example, if someone’s role needs to be updated from Viewer to Editor, you can change their role so they can start editing content in the app._

You can also edit roles to adjust the permissions attached to them. 

- _For example, you could add a permission to a role that allows users to delete content or access advanced settings._

### Deleting a User or Role  

If a user no longer needs access to the app, you can click the "Delete" button next to their name to remove them. Deleting a user will revoke their access, and they will no longer be able to log in.  

Similarly, if a role is no longer needed or you want to remove it from the app, you can delete the role. This will ensure that no one can be assigned that role in the future.  

### Summary  

In this web app, user management based on claims makes it easy to control who can access what and what they can do.  
By using roles (like Admin, Editor, or Viewer) and claims (such as permissions and attributes), the system makes sure that users have the right access to features and content.  
As an admin, you can add new users, assign roles, edit user details, and delete users or roles when needed.  

This approach helps keep the app organized, secure, and easy to manage, ensuring everyone has the right access to do their work efficiently!  

> [!IMPORTANT]
> In order for any changes to take effect you must log out and log in again !  

> [!TIP] 
> For more information, check the [**User Management Documentation**](../../documentation/user-management/overview.md) here.  
