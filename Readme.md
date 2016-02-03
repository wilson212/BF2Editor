# Battlefield 2 Script Editor

The Battlefield 2 Script Engine converts Con file objects from the popular Battlefield series into modifiable C# objects.

This project is considered as Alpha, and should not be used in a production environment.

### Current Features
  - Can load all .ai files in the "Objects_Server.zip/[Kits, Weapons, Vehicles]/ai" folders

### Examples
```C#
using BF2ScriptingEngine;
using BF2ScriptingEngine.Scripting;

// Loading a file
ConFile file = await ScriptEngine.LoadFileAsync(filePath);

// Getting a loaded file
AiTemplate obj = ObjectManager.GetObject<AiTemplate>("Ahe_Ah1z");

// Fetching an object property Value (Strongly Typed)
int value = obj.BasicTemp.Value;

// Setting a value (Unsafe, Exception will be thrown if BasicTemp was never defined)
obj.BasicTemp.Value = 300;

// Setting a value (safely)
obj.SetValue("basicTemp", 300);

// And formating the object back to Con File format
string formated = obj.File.ToFileFormat();

// Saving the Object changes
obj.File.Save();
```