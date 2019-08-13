Sequelocity.NET
===============

<a href="https://www.nuget.org/packages/Sequelocity"><img src="https://img.shields.io/nuget/v/Sequelocity.svg" alt="NuGet Version" /></a> 
<a href="https://www.nuget.org/packages/Sequelocity"><img src="https://img.shields.io/nuget/dt/Sequelocity.svg" alt="NuGet Download Count" /></a>
<a href="https://ci.appveyor.com/project/AmbitEnergyLabs/sequelocity-net">
	<img src="https://ci.appveyor.com/api/projects/status/github/ambitenergylabs/sequelocity.net?svg=true" alt="AppVeyor Build Status" />
</a>

*Note that this project is currently in a stable pre-release state and is already being used in production successfully.*
*We currently plan to bump to version 1.0 once PostgreSQL support is added.*

###What is it?###

Sequelocity.NET is a simple data access library for the Microsoft .NET Framework providing lightweight ADO.NET wrapper, object mapper, and helper functions.

It enables you to write fluent style syntax creating a more elegant and concise way of writing data access code. 

It can be used as a [C# single file drop in](https://raw.githubusercontent.com/AmbitEnergyLabs/Sequelocity.NET/master/src/SequelocityDotNet/SequelocityDotNet.cs) or referenced as a [NuGet Package](https://www.nuget.org/packages/Sequelocity).

###Documentation###

- [Sequelocity Wiki](https://github.com/AmbitEnergyLabs/Sequelocity.NET/wiki) - We have a growing number of wiki articles including an overview of the project, detailed examples, and more.
- XML Comments - All of the methods in the codebase our heavily XML commented which should give you plenty of intellisense description feel-goodness.
- API Documentation - We will also soon have MSDN-style API documentation to explore.

###Why Use It?###

Here is the super awesome list of reasons to use Sequelocity:
- Simplifies your project by eliminating repetitive boilerplate code
- Offers a very readable and concise fluent ( method chaining ) syntax
- Automatically handles all database connection opening, closing, and disposing of unneeded resources
- Can map results back to strongly typed objects, dynamic objects, DataSets or DataTables
- Can generate SQL insert statements given a strongly typed object, anonymous object, or dynamic object
- Is backed by over 500 tests

###Usage###

One of the design goals of Sequelocity is provide a very simple fluent ( method chaining ) programming interface enabling much more concise and readable code than standard ADO.NET code is typically written.

Common Sequelocity code would be implemented like this:

```csharp
using SequelocityDotNet;

public static SuperHero GetSuperHero( string superHeroName )
{
	const string sqlQuery = @"
SELECT	TOP 1 *
FROM	SuperHero
WHERE	SuperHeroName = @SuperHeroName
";

	var superHero = Sequelocity.GetDatabaseCommand()
		.SetCommandText( sqlQuery )
		.AddParameter( "@SuperHeroName", superHeroName, DbType.AnsiString )
		.ExecuteToList<SuperHero>()
		.FirstOrDefault();
	
	return superHero;
}
```

Please see the [Examples](https://github.com/AmbitEnergyLabs/Sequelocity.NET/wiki/Examples) wiki page or explore the project unit tests for many more examples.

###The DatabaseCommand###

Sequelocity introduces the DatabaseCommand as a thin layer of abstraction upon the native Microsoft .NET Framework [System.Data.Common.DbCommand](http://msdn.microsoft.com/en-us/library/System.Data.Common.DbCommand(v=vs.110).aspx) in order to introduce several beneficial features such as automatic connection handling, event handlers, and extension points.

#####Automatic Connection Handling#####

When utilizing the DatabaseCommand object, all of its execute methods automatically open and close the underlying database connection and dispose of the underlying ADO.NET DbCommand which removes the need for writing the common and repetitive boilerplate code of opening connections, closing connections, and C# using statements.

Through experience, we've found that the majority of database code simply needs to issue a single query and return the results so we've optimized the library to handle this most common scenario with ease by handling all connection-related concerns and disposing of no longer needed resources.

Of course if you are in need to keep a connection open after issuing a command, there are optional parameters that allow you to specify that giving you complete control of the connection handling.

#####Event Handlers aka Interception Points aka Hooks#####

When utilizing the DatabaseCommand object, all of its execute methods will automatically invoke / call all registered event handlers before calling the database ( Pre-Invoke ), after the database has been called and the results returned ( Post-Invoke ), and upon any unhandled exceptions.

It is very easy to register to one of these events in order to facilitate injecting your own behavior, to assist in debugging, or to enable logging queries, results, or exceptions.

#####Extension Points#####

If you take a look at the code base for Sequelocity, you will find that almost all of the behavior for the DatabaseCommand object is actually implemented as extension methods. What this means is that new behavior can be easily added by you or your team by simply implementing your own extension methods.

This architectural decision was very much intentional and demonstrates that Sequelocity is not really a framework but a utility and helper library for helping you connect and interact with your ADO.NET supported database of choice. The DatabaseCommand object is just a very simple lightweight wrapper around the .NET framework provided DbCommand in order to enable additional functionality and features.

*You can visit the [DatabaseCommand Overview](https://github.com/AmbitEnergyLabs/Sequelocity.NET/wiki/DatabaseCommand-Overview) wiki page for a more detail on it and examples of working with the DatabaseCommand.*

###Unit Tests and Integration Tests###

Sequelocity is backed by a large test suite comprising over 500 unit and integration tests. These tests serve as a contract specifying the behavior we expect out of each and every method which leads to a lot of tests which appear to be near duplicates of other tests and indeed many of the DatabaseCommand tests are 95% the same code as their DbCommand test counterparts but this is intentional and again is meant to serve as validation of expected method behavior. We hope that such a large test suite will give you confidence and 'the warm and fuzzies' knowing that many, many hours were put into writing tests that cover every inch of code in the Sequelocity code base and it is our hope that this will ensure a low number of bugs, increased quality, and the ability to add new features without breaking existing functionality.

#####Unit Test vs Integration Test#####
- We define unit tests as code that attempts to isolate a single unit, which we typically consider a method, and confirm its expected behavior attempting to not exercise any external resources. 
- We define an integration test as basically the same thing as a unit test except the goal is to also exercise the external resources such as running the method against an actual SQLite or Sql Server database. *Note that 'integration test' usually means more than this meaning but for this project this simplified meaning helps to differentiate the two types of testing being performed.

#####Test Projects Breakdown:#####
- [SequelocityDotNet](https://github.com/AmbitEnergyLabs/Sequelocity.NET/tree/master/src/SequelocityDotNet) - This is the main project and contains only a single file named SequelocityDotNet.cs.
- [SequelocityDotNet.Tests](https://github.com/AmbitEnergyLabs/Sequelocity.NET/tree/master/src/SequelocityDotNet.Tests) - These are unit tests as defined above, are the most abundant, and are lightning fast to run.
- [SequelocityDotNet.Tests.MySql](https://github.com/AmbitEnergyLabs/Sequelocity.NET/tree/master/src/SequelocityDotNet.Tests.MySql) - These are integration tests against a MySql database kindly provided by AppHarbor. *Note that we do not share the ConnectionString to our test MySQL server for obvious reasons so the tests won't be immediately runnable but you can simply replace the ConnectionString with your own and the tests should run just fine as all tests are self contained and will conveniently create any database objects that they need to satisfy the test.
- [SequelocityDotNet.Tests.SQLite](https://github.com/AmbitEnergyLabs/Sequelocity.NET/tree/master/src/SequelocityDotNet.Tests.SQLite) - These are integration tests against an in-memory SQLite database and are lightning fast to run.
- [SequelocityDotNet.Tests.SqlServer](https://github.com/AmbitEnergyLabs/Sequelocity.NET/tree/master/src/SequelocityDotNet.Tests.SqlServer) - These are integration tests against a Sql Server database kindly provided by AppHarbor. *Note that we do not share the ConnectionString to our test Sql Server for obvious reasons so the tests won't be immediately runnable but you can simply replace the ConnectionString with your own and the tests should run just fine as all tests are self contained and will conveniently create any database objects that they need to satisfy the test.

###Database Support###

Sequelocity works with all ADO.NET providers including SQL Server, SQLite, SQL CE, Oracle, MySQL, PostgreSQL, Firebird, etc.

**Database Provider Specific Implementations**

Sequelocity does include a few database provider specific implementation methods listed below. If you would like to contribute additional database provider specific implementations, please feel free to do so by submitting a pull request with accompanying integration tests.

MySQL
- GenerateInsertForMySQL - Generates a parameterized MySQL INSERT statement from the given object
- GenerateInsertsForMySQL - Generates a list of concatenated parameterized MySQL INSERT statements from the given list of objects
- GetDatabaseCommandForMySql - Gets a DatabaseCommand that interacts with a MySQL database.

SQLite
- GenerateInsertForSQLite - Generates a parameterized SQLite INSERT statement from the given object
- GenerateInsertsForSQLite - Generates a list of concatenated parameterized SQLite INSERT statements from the given list of objects
- GetDatabaseCommandForSQLite - Gets a DatabaseCommand that interacts with a SQLite database.

SQL Server
- GenerateInsertForSqlServer - Generates a parameterized SQLite INSERT statement from the given object
- GenerateInsertsForSqlServer - Generates a list of concatenated parameterized SQLite INSERT statements from the given list of objects
- GetDatabaseCommandForSqlServer - Gets a DatabaseCommand that interacts with a Microsoft SQL Server database.

###Open Source###

Sequelocity is open source software licensed under the MIT License. We love open source and use it extensively in all of our projects so it is with great pleasure that we can contribute back to the developer community with Sequelocity. We encourage you to explore the project, use it to your hearts content, provide feedback, open issue or feature requests, contribute to the project and / or fork it for your own use.

If you do find Sequelocity useful, we would love to hear about your experience and add your name to the list of organizations and/or projects that use Sequelocity.

###License###

MIT License:

Copyright (c) 2015 Ambit Energy. All rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
associated documentation files (the "Software"), to deal in the Software without restriction, including 
without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the 
following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial 
portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN 
NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
