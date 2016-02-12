# Battlefield 2 Script Editor

The Battlefield 2 Script Engine converts Con file objects from the popular Battlefield series into modifiable C# objects.

This project is considered as Alpha, and should not be used in a production environment.

### Current Features
  - Can load all .ai files in the "Objects_Server.zip/[Kits, Weapons, Vehicles]/ai" folders
  - Can modify objects and their variables in Type strong C# objects
  - Save modifications to objects back into their file
  - Deep reflection about objects and all entries in a script file
  - No script files needed, just create a Scope and have fun!

### Example #1
```C#
using BF2ScriptingEngine;
using BF2ScriptingEngine.Scripting;

// Loading a file
ConFile file = await ScriptEngine.LoadFileAsync(filePath);

// Getting a loaded object from its defined scope
AiTemplate obj = file.Scope.GetObject<AiTemplate>("Ahe_Ah1z");

// Fetching an object property Value (Strongly Typed)
int value = obj.BasicTemp.Value;

// Setting a value (Unsafe, Exception will be thrown if BasicTemp was never defined)
obj.BasicTemp.Value = 300;

// Setting a value (safely), if "basicTemp" was not defined in file, it will be created here
obj.SetValue("basicTemp", 300);

// ===
// There is even a third option for setting a value on an object, though it is not recommended
// to do it this way due to Reflection costs! For this example, we will use the
// ahe_ah1z helicopter
// ===
// Make sure to set the vehicle as active first by calling activeSafe or create
file.Scope.Execute("ObjectTemplate.create PlayerControlObject ahe_ah1z");
file.Scope.Execute("ObjectTemplate.maxVertRegAngle 35");

// Any object created in scope via Scope.Execute() can be fetched normally
PlayerControlObject Ah1z = file.Scope.GetObject<PlayerControlObject>("ahe_ah1z");
Debug.Assert(Ah1z.MaxVertRegAngle.Value == 35); // is True

// And formating the object back to Con File format
string formated = file.ToFileFormat();

// Saving the Object changes
file.Save();
```

### Example #2
Creating and Modifying an object using only Scopes
```C#
using BF2ScriptingEngine;
using BF2ScriptingEngine.Scripting;

// Create a new empty scope for this object
Scope scope = new Scope();

// First, create our new object
scope.Execute("ObjectTemplate.create PlayerControlObject ahe_ah1z");
scope.Execute("ObjectTemplate.creator BL:Wilson");
scope.Execute("ObjectTemplate.createComponent Armor");
scope.Execute("ObjectTemplate.armor.maxHitPoints 875");

// Even though the object was created in Scope.Execute(), we can access
// it just as if the object was created in C#
PlayerControlObject Ah1z = scope.GetObject<PlayerControlObject>("ahe_ah1z");
bool isNull = Ah1z.Armor?.MaxHitPoints?.Value == null; // is false
if (!isNull) // true
{
    Debug.Assert( Ah1z.Armor.MaxHitPoints.Value == 875 ); // is True
}