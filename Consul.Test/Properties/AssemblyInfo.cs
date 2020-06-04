﻿using System.Reflection;
using System.Runtime.InteropServices;
using Xunit;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Consul.NET Tests")]
[assembly: AssemblyDescription("Consul.NET Integration Tests")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("G-Research Limited")]
[assembly: AssemblyProduct("Consul.NET Tests")]
[assembly: AssemblyCopyright("Copyright 2015 PlayFab, Inc.; Copyright 2020 G-Research Limited")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("0728d942-1598-4fad-94b7-950f4777cb27")]

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
[assembly: AssemblyVersion("1.6.1")]
[assembly: AssemblyFileVersion("1.6.1")]
//[assembly: CollectionBehavior(DisableTestParallelization = false)]
[assembly: CollectionBehavior(MaxParallelThreads = 1024)]
