# UriShell

UriShell is a lightweight, .NET library for resolving and placing UI elements by its URI. 

# How it works 

Assuming that this two steps are done:
- every object, you want to open with UriShell, must have a unique(inside your application) URI and registered for resolution,
- every area, where the resolved object could be placed, must be registered,
usage is quite simple. 
```
var uri = "urishell://main-area/core-module/startup-item";
this._shell.Resolve(uri).Open();
```


# UriShell URI format

UriShell follows next convention of URI format:
```
<scheme>://<placement>:<ownerid>/<module>/<item>?<param1=value1>...
```

- scheme - *urishell* by default, but could be overridden in your app.
- placement - the name of a virtual area where a new object should appear.
- ownerid - (if specified) ID of the resolved object, where to look for a placement. It's common when the object wants to place other objects inside. 
- module+item - a unique pair that describes a specific object to be resolved by this pair. 
- param1...paramN - parameters of the object being resolved. UriShell doesn't care about them. 

# Error handling
By default all exceptions thrown during resolution are handled by UriShell and logged to Trace. 
But you can directly specify that your responsibility for exception handling by calling *OpenOrThrow* instead of *Open*: 
```
try
{
    this._shell.Resolve(uri).OpenOrThrow();
}
catch(Exception ex)
{
    // handle exception
}
```
