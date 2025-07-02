//===============================================================================
// SmartExpert Base Class Libraries (BCL)
//===============================================================================
// Copyright © 2008-2011 Andreas Börcsök.  All rights reserved.
//===============================================================================
#region Using directives

using System;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security;

#endregion

[assembly:ComVisible(false)]

// Set the version, company name, copyrigt & trademarks fields (solution wide scope)
[assembly: AssemblyCompany("Andreas B\x00f6rcs\x00f6k")]
[assembly: AssemblyCopyright("Copyright \x00a9 Andreas B\x00f6rcs\x00f6k 2008-2011")]
[assembly:AssemblyTrademark("")]

// Set the ProductName & ProductVersion fields
[assembly:AssemblyProduct("SmartExpert Framework")]

// Set the AssemblyVersion field
[assembly: AssemblyVersion("1.0.0.0")]
// Set the Assembly Version field for the Win32 file resource field
[assembly: AssemblyFileVersion("1.0.0.0")]
// Set the Assembly Manifest Version field
[assembly: AssemblyInformationalVersion("1.0.0.0")]


// Set the Language field
[assembly: AssemblyCulture("")]
// Neutral culture definition
[assembly: NeutralResourcesLanguage("en-US")]

// Allow this strong name assembly to be called by partial trust code.
[assembly: AllowPartiallyTrustedCallers]

// Set the Common Language Specification field
[assembly: CLSCompliant(false)]

#if DEBUG
[assembly:AssemblyConfiguration("Debug")]
#else
[assembly:AssemblyConfiguration("Release")]
#endif

