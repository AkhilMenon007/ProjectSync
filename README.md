# ProjectSync

### A simple package to handle Import/Export of packages in Unity

This package is developed mainly to streamline the process of handling commonly imported packages which gets updated frequently. 
It also includes a neat script to store project specific data through a **EditorPropertySaver** script.

 A typical example of this would be a pair of client - server projects which share resources as .unitypackage files.

## How to use

### Installing
Simply download the .unitypackage file from the releases tab and import it into your unity project.

The wizard for handling the imports and exports can be called by going to :
*FYP->Project Synchronizer -> Setup Window*.

#### Exporting

In the setup window you can specify the root of the folder which will be packed into the package which will be exported. Then you can give a name for the package which will be exported by setting the path of the exported package. Finally you can click the big button the export the package.
 
#### Importing

The process to importing is similar to exporting, simply fill in the required fields of the setup window and you are done.

### Usage

After the initial setup you can directly use the import and export buttons to pull and push the packages respectively without having to go through all the other options every single time.
