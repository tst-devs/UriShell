# UriShell

UriShell is a lightweight .NET library for resolving and placing UI elements by its URI. 

# UriShell URI format

UriShell follows next convention of URI format:
<scheme>://<placement>:<ownerid>/<module>/<item>?<param1=value1>...

- scheme - *urishell* by default, but could be overridden in your app;
- placement - the name of a virtual region where a new object should appear;
- ownerid - (if specified) ID of the resolved object, where to look for a placement. It's common when the object wants to place other objects inside. 
- module+item - a unique pair that describes a specific object to be resolved by this pair. 
- param1...paramN - parameters of the object. UriShell doesn't care about them. 
