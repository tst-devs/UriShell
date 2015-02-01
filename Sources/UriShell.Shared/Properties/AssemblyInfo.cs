using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Markup;

using UriShell;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("UriShell.Shared")]
[assembly: AssemblyDescription("UriShell interface library")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("UriShell")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: NeutralResourcesLanguageAttribute("en")]
[assembly: GuidAttribute("11240D2F-0EE5-43d9-B6E9-5F7EF0CC04E1")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]

// Xmlns aliases for XAML.
[assembly: XmlnsPrefix(PhoenixXmlNamespace.Value, PhoenixXmlNamespace.Prefix)]
[assembly: XmlnsDefinition(PhoenixXmlNamespace.Value, "TST.Phoenix.Arm")]
[assembly: XmlnsDefinition(PhoenixXmlNamespace.Value, "TST.Phoenix.Arm.Data")]
[assembly: XmlnsDefinition(PhoenixXmlNamespace.Value, "TST.Phoenix.Arm.Logging")]
[assembly: XmlnsDefinition(PhoenixXmlNamespace.Value, "TST.Phoenix.Arm.Modularity")]
[assembly: XmlnsDefinition(PhoenixXmlNamespace.Value, "TST.Phoenix.Arm.Shell")]
[assembly: XmlnsDefinition(PhoenixXmlNamespace.Value, "TST.Phoenix.Arm.Shell.Connectors")]
[assembly: XmlnsDefinition(PhoenixXmlNamespace.Value, "TST.Phoenix.Arm.Shell.Events")]