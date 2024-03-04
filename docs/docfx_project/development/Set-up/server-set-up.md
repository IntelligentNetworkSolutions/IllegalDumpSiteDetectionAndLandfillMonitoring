# Windows 11 Development Server Set Up

## .NET 8  

- Download and Install .NET 8 Hosting Bundle  
  - Download from [Latest .NET 8 version](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)  
  - Choose Hosting Bundle  
    - _for IIS Support_  
  - Install

## PostgreSQL 16  

- Download and Install PostgreSQL 16  
  - Download the installer from [Latest PostgreSQL version](https://www.postgresql.org/download/windows/)  
  - Run the installer  
  - The StackBuilder should be included as well  
  - We recommend the default options  
    - _Except for maybe port 5433 :D_  
  - From the StackBuilder:  
    - __Npgsql__  
    - __PostGIS 3.4__  

## VS Code  

- Download and Install Visual Studio Code
  - Download the installer from [Latest VS Code version](https://code.visualstudio.com/Download)  
  - Run the installer  
  - We recommend the default options  

## Visual Studio  

- Download and Install Visual Studio  
  - Download the installer from [Latest Visual Studio version](https://visualstudio.microsoft.com/)  
  - Run the installer  
  - Choose:  
    - .NET Web Development  
      - Development time IIS Suport  
    - Python Development  

## NVIDIA (Optional)  

- Download and Install the latest appropriate NVIDIA driver for your graphic card  
  - <em style="font-weight: 100; font-style: itallic;">psst, don't trust the device manager</em>  
  - Download the Driver installer from [Latest versions of NVIDIA Drivers Choose Yours](https://www.nvidia.com/Download/index.aspx?lang=en-us)  
  - [Example](https://www.nvidia.com/Download/driverResults.aspx/218113/en-us/)  
  - Run the installer  

- Download and Install CUDA  
  - Download the CUDA installer from [Latest CUDA versions Choose Yours](https://developer.nvidia.com/cuda-downloads)  
  - Run the installer  

## MMDetection  

- Follow the [Instructions](https://mmdetection.readthedocs.io/en/latest/get_started.html)  

## Miniconda 3  

- Download and Install Miniconda 3  
  - [Read](https://docs.anaconda.com/free/miniconda/miniconda-install/)  
  - Download the installer from [Latest Miniconda version](https://docs.anaconda.com/free/miniconda/)  
  - __Find Miniconda 3 elsewhere if mayor version goes to 4__  
  - Run the installer  
  - We recommend the default options  

> __GPT Thoughts__  
> When you create a new environment using Miniconda from the command line, it is typically stored in the `envs` directory within your Miniconda installation. For example, if you installed Miniconda in the `C:\Users\userName\Anaconda3` directory, the environments would be stored in `C:\Users\userName\Anaconda3\envs`⁵.
>
> You can create a new environment using the command `conda create --name myenv`, where `myenv` is the name of your new environment¹. Once the environment is created, you can activate it using the command `conda activate myenv`¹.
>
> Please replace `myenv` with the name of your environment. If you're using Windows, you might need to add the Miniconda directory and the `Scripts` subdirectory to your PATH¹. For example, if you installed Miniconda into `C:\Miniconda3`, you would add `C:\Miniconda3` and `C:\Miniconda3\Scripts` to your PATH¹.

## Conda Enviroment  

- Run the Anaconda Prompt App installed in the previous step  
- Create and Activate the MMDetection Enviroment  
- Type in the following commands:  
  - `conda create --name openmmlab python=3.8 -y`  
  - `conda activate openmmlab`  

## PyTorch  

- Get the install command appropriate for your current setup  
  - [Command Builder](https://pytorch.org/get-started/locally/)  
  - PyTorch Build => Stable 2.2.0  
  - Package => Conda  
  - Language => Python  
  - Compute Platform => Different commands for CPU / GPU  
    - CUDA version dependent on Graphics Card  

- Example Command:  
  - `conda install pytorch torchvision torchaudio pytorch-cuda=12.1 -c pytorch -c nvidia`  

## C++ 14 - Visual Studio Build Tools  

- Download and Install Visual Studio Build Tools in order to have C++ 14 
  - [](Visual Studio Build Tools  )