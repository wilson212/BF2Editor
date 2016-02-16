# Battlefield 2 Script Editor

The Battlefield 2 Script Engine converts Con file objects from the popular Battlefield series into mutable C# objects.

This project is considered as Alpha, and should not be used in a production environment.

### Current Features
  - All objects loaded asynchronously using Async/Await 
  - Very fast parsing (roughly 15,000 lines per second, depending on hardware)
  - Loads all .ai and .con files in the "Objects_Server.zip/[Kits, Weapons, Vehicles]" 
	folders (.tweak files are a WIP still)
  - The ability modify objects and their variables in Type strong C# objects
  - Save modifications to objects back into their file
  - Deep reflection about objects and all entries in a script file
  - No script files needed, it is totally possible to just create a Scope and have fun!
  - Extendable and Scalable. Possible to add or override object types, 
	and object reference types outside of the script engine library

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
```
### Custom Object Reference Types
In the Bf2 Script Engine, it is possible to define custom object ReferenceType's that are not supported by default. In this example, we will use a class called "GameSettings" that is created outside of the BF2ScriptingEngine.dll library.
```C#
using BF2ScriptingEngine;
using BF2ScriptingEngine.Scripting;

// Create a new Reference type for the script engine to use
var type = new ReferenceType("GameSettings", typeof(MyProject.GameSettings));

// Map the custom object creation methods (granted the ReferenceType is not a static entity)
type.Mappings.Add("create", MyProject.GameSettings.Create);

// Add new ReferenceType to the ReferenceManager. This tells the ScriptEngine that "GameSettings"
// is a valid ReferenceType withing a Con file.
ReferenceManager.AddType(type);

// === Now we can use the custom ReferenceType in con file scripts
// Create a new empty scope for this object
Scope scope = new Scope();

// Create our new object using the mapping we supplied above
scope.Execute("GameSettings.create {type?} myGameSettings"); // Creates a new GameSettings object named "myGameSettings"
scope.Execute("GameSettings.hudRBGColor 32/65/96"); // Example custom property
```

### Custom Object Types
As with Reference Types, it is also possible to define custom derived objects that may or may not be supported by default. In this example, we will override the "GenericFireArm" type that derives from the ObjectTemplate Reference Type, that is created outside of the BF2ScriptingEngine.dll library.
```C#
using BF2ScriptingEngine;
using BF2ScriptingEngine.Scripting;

// Create a bare minimum object, with no properties
namespace MyProject
{
    // It is required that the new object derives from ConFileObject, or a class that derives
    // from ConFileObject
    public class MyGenericFireArm : ConFileObject
    {
        // It is required to pass these arguments to ConFileObject
        public MyGenericFireArm(string name, Token token) : base(name, token) { }
    }
}

// Now, override the default ObjectTemplate GenericFireArm type with our custom defined type.
// You can use this method to add new object types that are not supported by default.
ObjectTemplate.ObjectTypes["GenericFireArm"] = typeof(MyGenericFireArm);
```
Now whenever a new command is found, attempting to create a new GenericFireArm type, MyGenericFireArm will be created instead of the default GenericFireArm type.