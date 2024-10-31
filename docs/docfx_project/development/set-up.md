# Windows 11 Development Server Set Up

## .NET 8  

- Download and Install .NET 8 SDK 
  - Download from [Latest .NET 8 version](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)  
  - _If planing to use IIS as a server then also install the runtime Hosting Bundle_    
  - Install

## PostgreSQL 16  

- Download and Install PostgreSQL 16  
  - Download the installer from [Latest PostgreSQL version](https://www.postgresql.org/download/windows/)  
  - Run the installer  
  - The StackBuilder should be included as well  
  - We recommend the default options  
    - _Except for maybe port 5432 :D_  
  - From the StackBuilder:  
    - __Npgsql__  
    - __PostGIS 3.4 or 3.5__  

## Git

- Download Git from the [official site](https://git-scm.com/downloads/win)
- Install

## Node.js

- Download Node.js from the [official site](https://nodejs.org/en/download)
  - We recommend the latest LTS version >= 20.0.0


## Visual Studio Community 2022 (recommended) 

- Download and Install Visual Studio Community 2022 
  - Download the installer from [Latest Visual Studio version](https://visualstudio.microsoft.com/)  
  - Run the installer  
  - Choose:  	
    - ASP.NET and Web Development
    - Python Development
	  - Only the langugage support is required
	- Desktop Developement with C++ (are required later for the MMDetection)
	  - On the right side of the screen make sure the following are selected:
	    - Windows 10 SDK or Windows 11 SDK (depending on your current OS)
		- MSVC v141
		- MSVC v143
		- C++ CMake tools for Windows


## Visual Studio Code (alternative)

- Download and Install Visual Studio Code
  - Download the installer from [Latest VS Code version](https://code.visualstudio.com/Download)  
  - Run the installer  
  - We recommend the default options  
- Install extensions. Read more [here](https://code.visualstudio.com/docs/languages/csharp)
- Make sure to download [Microsoft C++ Build Tools](https://visualstudio.microsoft.com/visual-cpp-build-tools/) and install (are required later for the MMDetection):
  - Windows 10 SDK or Windows 11 SDK (depending on your current OS)
  - MSVC v141
  - MSVC v143
  - C++ CMake tools for Windows


## Miniconda 3  

- Download and Install Miniconda 3  
  - [Read](https://docs.anaconda.com/free/miniconda/miniconda-install/)  
  - Download the installer from [Latest Miniconda version](https://docs.anaconda.com/free/miniconda/)  
  - __Find Miniconda 3 elsewhere if mayor version goes to 4__  
  - Run the installer  
  - __Install for all users__
  - We recommend to leave the default settings for all other install options 


## NVIDIA GPU(Optional)  

- Download and Install the latest appropriate NVIDIA driver for your graphic card  
  - <em style="font-weight: 100; font-style: itallic;">psst, don't trust the device manager</em>  
  - Download the Driver installer from [Latest versions of NVIDIA Drivers Choose Yours](https://www.nvidia.com/Download/index.aspx?lang=en-us)  
  - [Example](https://www.nvidia.com/Download/driverResults.aspx/218113/en-us/)  
  - Run the installer  

- Download and Install CUDA  
  - Download the CUDA installer from [Latest CUDA versions Choose Yours](https://developer.nvidia.com/cuda-downloads)  
  - Run the installer

## MMDetection

You can find the official [instructions here](https://mmdetection.readthedocs.io/en/latest/get_started.html), or you can skip the official instructions and use the following steps that work well for us:

- Run the Anaconda Prompt App installed in one of the previous steps
- Create the environment. Type in the following commands:  
  - `conda create --name openmmlab python=3.8 -y`  
  - `conda activate openmmlab`  
- Install PyTorch 2.2.0 (choose one)
  - installation for CPU:
    - `conda install pytorch==2.2.0 torchvision==0.17.0 torchaudio==2.2.0 cpuonly -c pytorch`
  -	installation for GPU (make sure you have the right version of CUDA):
	- `conda install pytorch==2.2.0 torchvision==0.17.0 torchaudio==2.2.0 pytorch-cuda=12.1 -c pytorch -c nvidia`
  - If you have any issues regarding the pytorch install you can [read more here](https://pytorch.org/get-started/locally/)
- Install openmim:
  - `pip install -U "openmim==0.3.9"`
- Install mmengine:
  - `mim install "mmengine==0.10.3"`
- Install mmcv:
  - `mim install "mmcv==2.1.0"`
- Install mmdetection:
  - `git clone https://github.com/open-mmlab/mmdetection.git`  
  - `cd mmdetection`
  - `pip install -v -e .`
- Verify the installation:
  - `mim download mmdet --config rtmdet_tiny_8xb32-300e_coco --dest .` 
  - `python demo/image_demo.py demo/demo.jpg rtmdet_tiny_8xb32-300e_coco.py --weights rtmdet_tiny_8xb32-300e_coco_20220902_112414-78e30dcc.pth --device cpu`
  - In the "mmdetection/outputs/vis" folder there should be a "demo.jpg" image 
- Install sahi:
  - `pip install -U sahi` 
  
  
## Waste comparison analysis 
- Open Conda command prompt.
- Be sure that you are located on the base level, not in some environment.
- Create new environment for waste comparison reports and analysis, also installing pdal and gdal in the same environment by typing the following command:
	-`conda create --name waste_comparison_env -c conda-forge python pdal gdal=3.8.4`
- Download PotreeConverter and place it in some directory
- Install Gdal globally on your PC 
	- Recommended version `https://build2.gisinternals.com/sdk/downloads/release-1930-x64-gdal-3-8-4-mapserver-8-0-1/gdal-3.8.4-1930-x64-core.msi` 
	- Open enviroment variables window, and in system variables find `Path` and there add the path to GDAL installation directory (ex. C:\ProgramFiles\GDAL)


## GIT CLONE REPO
- Open cmd
- Navigate to the directory where you want to clone the repository by typing the following command
	-`cd C:\path\to\your\directory`
- Then type the following command to clone the repository
	-`git clone https://github.com/IntelligentNetworkSolutions/IllegalDumpSiteDetectionAndLandfillMonitoring.git`

## Restore package.json libs
- Open the solution in VS 
- In the solution explorer, in MainApp.MVC, find the package.json file 
- Right click on package.json file and choose `Restore Packages`

## ENVIRONMENT VARS
- Open enviroment variables window 
	-`Settings > System > About > Advanced system settings > Environment variables`
- In system variables click `New` to add new variables
	- Add the following five variables in order(Variable name Variable value):
	`ASPNETCORE_ConnectionStrings__MasterDatabase Host=localhost;Port=Choose Your Port Number for you server(ex. 5434);Database=Write the database name which must match the exact same name as the created database name (ex. WasteDetection);Username=Write the username for the server (ex. postgres);Password=Write the password for your server (ex. admin);Pooling=true`
	`ASPNETCORE_SeedDatabaseDefaultValues__SeedDatabaseDefaultPasswordForAdminUsers DefaultPassword321$`
	`ASPNETCORE_SeedDatabaseDefaultValues__SeedDatabaseDefaultPasswordForSuperAdmin SuperAdminPass123$`
	`ASPNETCORE_SeedDatabaseDefaultValues__SeedDatabaseDefaultSuperAdminUserName superadmin`
	`OPENCV_IO_MAX_IMAGE_PIXELS 1099511627776`
- Use user secrets instead of system environmet variables for development purposes. https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/development/shhh_secrets.html
	
## CREATE DB (Nema da treba so seed database se kreira bazata)
- Open PgAdmin
- In Object Exporer find your server and open it
- Right click on Databases and choose Create > Database
	- Write the database name and click Save


## Configure app settings
- In appsettings.json and appsettings.Development.json files do the following changes:
	- Change RootDirAbsPath with the correct path to mmdetection folder
	- Change CondaExeAbsPath with the correct path to _conda.exe file
	- Change OpenMMLabAbsPath with the correct path to openmmlab environment folder
	
- In applicationSettingsSeedDatabase.json file located in the DAL project in solution exporer do the following changes:
	- Change GdalCalcAbsPath with the correct path to gdal_calc.py file 
	- Change PdalExePath with the correct path to pdal.exe file
	- Change PotreeConverterFilePath with the correct path to potreeConverter.exe file
	- Change PythonExeAbsPath with the correct path to python.exe file

## SEED DATABASE
- Build you application
- Go in the folder where you MainApp.MVC application is locatated 
	- Open bin/Debug/net8.0 folder
	- Click shipft + right click and choose open PowerShell window here 
	- write the following command `dotnet .\MainApp.MVC.dll /seed /runMigrations /loadModules /module:MMDetectionSetup`	
- After the seed is completed successfully, open pgAdmin, find your database: 
	- Go to trained_models database table copy the model file path for model `ResNet101COCOx1` and paste it as a value for `BackboneCheckpointAbsPath` into appsettings.json and appsettings.development.json 
	

## privilegii do site folderi

## To Be Continued ...  

