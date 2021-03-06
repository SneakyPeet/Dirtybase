﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Dirtybase")]
[assembly: AssemblyDescription("Dirtybase is a persistence version/migration command line tool. Dirtybase will compare version files in a folder to the version of a data store, then apply the version files in order to migrate the data store to the latest version. This is useful for keeping data stores on different environments up to date and automating deployments.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Pieter Koornhof")]
[assembly: AssemblyProduct("Dirtybase")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("d2154d74-9be9-4bc4-9a21-85b01d961167")]

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
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: InternalsVisibleTo("Dirtybase.Tests")]
