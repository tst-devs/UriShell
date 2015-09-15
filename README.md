# UriShell

UriShell is a lightweight, .NET library for resolving and placing UI elements by its URI. 

# How it works 

Assuming that this two steps are done:
- every object, you want to open with UriShell, must have a unique(inside your application) URI and registered for resolution,
- every area, where the resolved object could be placed, must be registered,

opening is quite simple:
```C#
var uri = "urishell://main-area/core-module/startup-item";
var disposable = this._shell.Resolve(uri).Open();
```
and closing is even simpler:  
```C#
disposable.Dispose();
```

# Object's setup

Sometimes you need to do something before object is opened/closed. Probably you want to subscribe/unsubscribe to/from events. Or propagate some property before start and receive it back when object is closed. UriShell has a superb way to achieve this: 
```C#
var uri = "urishell://main-area/core-module/startup-item";
var disposable = this._shell
    .Resolve(uri)
    .Setup<IStartupView>()
    .OnReady(startupView =>
    {
        startupView.VisibleDocumentCount = 10;
        startupView.DocumentOpened += this.StartupDocumentOpen;
    })
    .OnFinished(startupView => 
    {
        startupView.DocumentOpened -= this.StartupDocumentOpen;
    })
    .Open();
```

*Setup* tries to cast a resolved object to the given type. If cast fails, *OnReady* and *OnFinished* aren't called.

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
But you can directly specify your responsibility for exception handling by calling *OpenOrThrow* instead of *Open*: 
```C#
try
{
    var disposable = this._shell.Resolve(uri).OpenOrThrow();
}
catch(Exception ex)
{
    // handle exception
}
```

# Dependency injection container's support

Currently UriShell supports only Autofac, but it's planned to add support for other popular containers.

## Autofac
If you use [Autofac](http://autofac.org/), you should include UriShell.Autofac assembly to your project and register UriShellModule as module: 

```C#
var builder = new ContainerBuilder();
builder.RegisterModule<UriShellModule>();

// other registrations...

this.Container = builder.Build();
```

If your application uses [nested containers](http://autofac.readthedocs.org/en/latest/lifetime/working-with-scopes.html#creating-a-new-lifetime-scope) you have to resolve AutofacViewModelViewMatcher and call AddContainer method for every container where objects supposed to be resolved are resistered. 
```C#
var builder = new ContainerBuilder();
builder.RegisterModule<UriShellModule>();
this.Container = builder.Build();

var moduleContainer = this.Container.BeginLifetimeScope(b =>
{
	b.RegisterType<...>();
});

var matcher = moduleContainer.Resolve<AutofacViewModelViewMatcher>();
matcher.AddContainer(moduleContainer);
```
