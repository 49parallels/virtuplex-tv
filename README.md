# virtuplex-tv

**Specs:**

Unity 2019.4.8f1.  
AR Foundation 3.1.6.  
AR Subsystems 3.1.6.  
ARKit/ARCore XR Plugin 3.1.6.  

**Testing device:**  
iPhone X

Please switch to desired platform in **FILE > BUILD SETTINGS**.  

The root directory contains two relevant directories:  
*Externals/* - contains external packages from asset store, in this case Another Color Picker.   
*Project/* - contains all of relevant project files including the CarShowroom in *Project/Scenes/* which you should enter and build

*ARSessionOrigin* has component *PlaceOnPlane* that servers for all touch input and spawns the *CarPlaceholder* prefab

*UIControl* controls all Canvas elements and their behaviours

There is one scriptable object *ControlState* which holds the current scene state and is shared with other controllers (UIControl for instance)
In *CarPlaceholder* there is *DoorControl* prefab that consumes touches per car area (box colliders). The box colliders have attached component *DoorOpenClose* that 
references to actual meshes in car model (Left Door, Right Door etc.) and open/closes them when tapped.

The color is controlled through global variable "_GlobalColor" that resides in *GlobalShader* that we switched with the *Standart* shader formerly attached to *carpaint* material.
This way we don't have to get references to all meshes with carpaint material we can change their color at once using shader.

TV
