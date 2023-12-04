# Unity Project Tool

## Description

This tool is designed to analyze Unity projects and extract information about every scene hierarchy inside project, as well as identify and list unused scripts within the project.

## Usage
```
./tool.exe unity_project_path output_folder_path
```
**unity_project_path**: The path to the Unity project directory.

**output_folder_path**: The path to the folder where the tool will output the results.

## Output
The tool generates two types of **outputs**:

#### 1. Unity Scene Hierarchy Dump
The scene hierarchy information is saved in .unity.dump files. Each file corresponds to a Unity scene and contains the hierarchy structure.

Example filename: **Scene1.unity.dump**

#### 2. Unused Scripts
Unused scripts are identified and listed in the **UnusedScripts.txt** file. Each line in the file contains relative path and GUID of an unused script.