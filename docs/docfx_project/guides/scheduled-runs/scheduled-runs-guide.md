# Scheduled Runs  

### Manage Scheduled Runs  

The Manage Scheduled Runs page allows you to view and manage your scheduled detection and training runs.  
Below is an example table displaying all scheduled runs, with essential details about each run.

### Table of Scheduled Runs

| Name        | Description                | Created By | Created On       | Is Completed | Status | Action                                                                                                                                  |
|:-----------------:|:--------------------------------:|:----------------:|:----------------------:|:------------------:|:------------:|:--------------------------------------------------------------------------------------------------------------------------------------------:|
| Run 1           | Detection of potential errors  | User 1       | 12.02.2024 10:00  | True             | Success    |<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 576 512" height="18" width=18 style=" fill: #6e7075"><path d="M408 120c0 54.6-73.1 151.9-105.2 192c-7.7 9.6-22 9.6-29.6 0C241.1 271.9 168 174.6 168 120C168 53.7 221.7 0 288 0s120 53.7 120 120zm8 80.4c3.5-6.9 6.7-13.8 9.6-20.6c.5-1.2 1-2.5 1.5-3.7l116-46.4C558.9 123.4 576 135 576 152l0 270.8c0 9.8-6 18.6-15.1 22.3L416 503l0-302.6zM137.6 138.3c2.4 14.1 7.2 28.3 12.8 41.5c2.9 6.8 6.1 13.7 9.6 20.6l0 251.4L32.9 502.7C17.1 509 0 497.4 0 480.4L0 209.6c0-9.8 6-18.6 15.1-22.3l122.6-49zM327.8 332c13.9-17.4 35.7-45.7 56.2-77l0 249.3L192 449.4 192 255c20.5 31.3 42.3 59.6 56.2 77c20.5 25.6 59.1 25.6 79.6 0zM288 152a40 40 0 1 0 0-80 40 40 0 1 0 0 80z"/></svg>                                                                                                                      |
| Run 2           | Weekly system scan             | User 2     | 25.11.2023 02:15  | False            | Error      | <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512" height="18" width=18 style=" fill: #6e7075"><path d="M135.2 17.7L128 32 32 32C14.3 32 0 46.3 0 64S14.3 96 32 96l384 0c17.7 0 32-14.3 32-32s-14.3-32-32-32l-96 0-7.2-14.3C307.4 6.8 296.3 0 284.2 0L163.8 0c-12.1 0-23.2 6.8-28.6 17.7zM416 128L32 128 53.2 467c1.6 25.3 22.6 45 47.9 45l245.8 0c25.3 0 46.3-19.7 47.9-45L416 128z"/></svg> <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512" height="18" width=18 style=" fill: #6e7075"><path d="M256 32c14.2 0 27.3 7.5 34.5 19.8l216 368c7.3 12.4 7.3 27.7 .2 40.1S486.3 480 472 480L40 480c-14.3 0-27.6-7.7-34.7-20.1s-7-27.8 .2-40.1l216-368C228.7 39.5 241.8 32 256 32zm0 128c-13.3 0-24 10.7-24 24l0 112c0 13.3 10.7 24 24 24s24-10.7 24-24l0-112c0-13.3-10.7-24-24-24zm32 224a32 32 0 1 0 -64 0 32 32 0 1 0 64 0z"/></svg> |
| Run 3           | Security log review            | User 3   | 02.12.2024 09:30  | True             | Success    |<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 576 512" height="18" width=18 style=" fill: #6e7075"><path d="M408 120c0 54.6-73.1 151.9-105.2 192c-7.7 9.6-22 9.6-29.6 0C241.1 271.9 168 174.6 168 120C168 53.7 221.7 0 288 0s120 53.7 120 120zm8 80.4c3.5-6.9 6.7-13.8 9.6-20.6c.5-1.2 1-2.5 1.5-3.7l116-46.4C558.9 123.4 576 135 576 152l0 270.8c0 9.8-6 18.6-15.1 22.3L416 503l0-302.6zM137.6 138.3c2.4 14.1 7.2 28.3 12.8 41.5c2.9 6.8 6.1 13.7 9.6 20.6l0 251.4L32.9 502.7C17.1 509 0 497.4 0 480.4L0 209.6c0-9.8 6-18.6 15.1-22.3l122.6-49zM327.8 332c13.9-17.4 35.7-45.7 56.2-77l0 249.3L192 449.4 192 255c20.5 31.3 42.3 59.6 56.2 77c20.5 25.6 59.1 25.6 79.6 0zM288 152a40 40 0 1 0 0-80 40 40 0 1 0 0 80z"/></svg> |

### Explanation of Columns  

1. **Name**: The name of the scheduled run (e.g., "Run 1", "Run 2").  
2. **Description**: A brief description of what the detection run does (e.g., "Detection of potential errors", "Weekly system scan"). This column only appears for detection runs.  
3. **Created By**: The user who created the scheduled run (e.g., "User 1", "User 2").  
4. **Created On**: The date and time when the run was created (e.g., "02.12.2024 09:30").  
5. **Is Completed**: Indicates whether the run has finished  
   - **True** means the run is complete.  
   - **False** means the run is still ongoing or hasn’t been completed yet.  
6. **Status**: Shows the current status of the run  
   - **Success** means the run finished without issues.  
   - **Error** means something went wrong during the run.  
7. **Action**: Action buttons based on the run's status:
   - <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 576 512" height="18" width=18 style=" fill: #6e7075"><path d="M408 120c0 54.6-73.1 151.9-105.2 192c-7.7 9.6-22 9.6-29.6 0C241.1 271.9 168 174.6 168 120C168 53.7 221.7 0 288 0s120 53.7 120 120zm8 80.4c3.5-6.9 6.7-13.8 9.6-20.6c.5-1.2 1-2.5 1.5-3.7l116-46.4C558.9 123.4 576 135 576 152l0 270.8c0 9.8-6 18.6-15.1 22.3L416 503l0-302.6zM137.6 138.3c2.4 14.1 7.2 28.3 12.8 41.5c2.9 6.8 6.1 13.7 9.6 20.6l0 251.4L32.9 502.7C17.1 509 0 497.4 0 480.4L0 209.6c0-9.8 6-18.6 15.1-22.3l122.6-49zM327.8 332c13.9-17.4 35.7-45.7 56.2-77l0 249.3L192 449.4 192 255c20.5 31.3 42.3 59.6 56.2 77c20.5 25.6 59.1 25.6 79.6 0zM288 152a40 40 0 1 0 0-80 40 40 0 1 0 0 80z"/></svg> **View Detection Run** : Displays if the detection run was completed successfully 
   - <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512" height="18" width=18 style=" fill: #6e7075"><path d="M256 32c14.2 0 27.3 7.5 34.5 19.8l216 368c7.3 12.4 7.3 27.7 .2 40.1S486.3 480 472 480L40 480c-14.3 0-27.6-7.7-34.7-20.1s-7-27.8 .2-40.1l216-368C228.7 39.5 241.8 32 256 32zm0 128c-13.3 0-24 10.7-24 24l0 112c0 13.3 10.7 24 24 24s24-10.7 24-24l0-112c0-13.3-10.7-24-24-24zm32 224a32 32 0 1 0 -64 0 32 32 0 1 0 64 0z"/></svg> **View Training Run Error**: Displays if the detection run has an error.
   - <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512" height="18" width=18 style=" fill: #6e7075"><path d="M135.2 17.7L128 32 32 32C14.3 32 0 46.3 0 64S14.3 96 32 96l384 0c17.7 0 32-14.3 32-32s-14.3-32-32-32l-96 0-7.2-14.3C307.4 6.8 296.3 0 284.2 0L163.8 0c-12.1 0-23.2 6.8-28.6 17.7zM416 128L32 128 53.2 467c1.6 25.3 22.6 45 47.9 45l245.8 0c25.3 0 46.3-19.7 47.9-45L416 128z"/></svg> **Delete Training Run**: Displays if the detection run has an error.  

### Search Bar  

A Search bar is available at the top of the page to help you quickly find a specific run.  
You can search by **Name**, **Created By**, or **Status** to filter the list.  

This table provides an organized way to manage and take action on your scheduled runs.  
You can quickly check their status, view results, or delete problematic runs as needed.  

---  

> [!WARNING]
> Only runs that haven't yet been started can be cancelled  
> Might not display anything in case of manual run deletion  

> [!TIP]
> For more information, check the [**Scheduled Runs Documentation**](../../documentation/scheduled-runs/overview.md) here.  
