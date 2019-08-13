using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Transactions;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.SqlServer
{
    [TestFixture]
    public class Examples
    {
        #region TestFixtureSetUp

        private const string CreateSchemaSql = @"

/* Drop Tables */
IF ( EXISTS (	SELECT	* 
				FROM	INFORMATION_SCHEMA.TABLES 
				WHERE	TABLE_SCHEMA = 'dbo' 
						AND	TABLE_NAME = 'EmailAddress' ) )
BEGIN
	DROP TABLE EmailAddress
END

IF ( EXISTS (	SELECT	* 
				FROM	INFORMATION_SCHEMA.TABLES 
				WHERE	TABLE_SCHEMA = 'dbo' 
						AND	TABLE_NAME = 'Customer' ) )
BEGIN
	DROP TABLE Customer
END

/* Create Customer Table */
IF ( NOT EXISTS (	SELECT	* 
					FROM	INFORMATION_SCHEMA.TABLES 
					WHERE	TABLE_SCHEMA = 'dbo' 
							AND	TABLE_NAME = 'Customer') )
BEGIN

	CREATE TABLE Customer
	(
		CustomerId      INT             NOT NULL    IDENTITY(1,1),
		FirstName       NVARCHAR(120)   NOT NULL,
		LastName        NVARCHAR(120)   NOT NULL,
		DateOfBirth     DATETIME        NOT NULL,
		ModifiedDate	DATETIME		NOT NULL,
		ModifiedBy		NVARCHAR(120)	NOT NULL
	);

	ALTER TABLE Customer
	ADD CONSTRAINT [PK_Customer_CustomerId]
	PRIMARY KEY CLUSTERED ( CustomerId );

	ALTER TABLE Customer
	ADD CONSTRAINT [DF_Customer_ModifiedDate] 
	DEFAULT GETDATE() FOR ModifiedDate;

	ALTER TABLE Customer
	ADD CONSTRAINT [DF_Customer_ModifiedBy] 
	DEFAULT SUSER_SNAME() FOR ModifiedBy;

END


/* Create EmailAddress Table */
IF ( NOT EXISTS (	SELECT	* 
					FROM	INFORMATION_SCHEMA.TABLES 
					WHERE	TABLE_SCHEMA = 'dbo' 
							AND	TABLE_NAME = 'EmailAddress') )
BEGIN

	CREATE TABLE EmailAddress
	(
		EmailAddressId	INT             NOT NULL    IDENTITY(1,1),
		CustomerId      INT             NOT NULL,
		EmailAddress    NVARCHAR(256)   NOT NULL,
		IsActive		BIT		        NOT NULL,
		ModifiedDate	DATETIME		NOT NULL,
		ModifiedBy		NVARCHAR(120)	NOT NULL
	);

	ALTER TABLE EmailAddress
	ADD CONSTRAINT [PK_EmailAddress_EmailAddressId]
	PRIMARY KEY CLUSTERED ( EmailAddressId );

	ALTER TABLE EmailAddress
	ADD CONSTRAINT [FK_EmailAddress_CustomerId] 
	FOREIGN KEY ( CustomerId )
	REFERENCES Customer ( CustomerId );

	ALTER TABLE EmailAddress
	ADD CONSTRAINT [DF_EmailAddress_ModifiedDate] 
	DEFAULT GETDATE() FOR ModifiedDate;

	ALTER TABLE EmailAddress
	ADD CONSTRAINT [DF_EmailAddress_ModifiedBy] 
	DEFAULT SUSER_SNAME() FOR ModifiedBy;

	ALTER TABLE EmailAddress
	ADD CONSTRAINT [AK_EmailAddress_EmailAddress] 
	UNIQUE ( EmailAddress );

END";
        //[TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
                .SetCommandText( CreateSchemaSql )
                .ExecuteNonQuery();
        }

        #endregion TestFixtureSetUp

#region Execute Methods

[Test]
public void ExecuteNonQuery_With_No_Parameters_Example()
{
    // Arrange
    const string sql = @"
CREATE TABLE #SuperHero
(
    SuperHeroId     INT             NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
    SuperHeroName	NVARCHAR(120)   NOT NULL
);

/* This insert should trigger 1 row affected */
INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( 'Superman' );";

    // Act
    int rowsAffected = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
        .SetCommandText( sql )
        .ExecuteNonQuery();

    // Assert
    Assert.That( rowsAffected == 1 );
}

[Test]
public void ExecuteNonQuery_With_Parameters_Example()
{
    // Arrange
    const string sql = @"
CREATE TABLE #SuperHero
(
    SuperHeroId     INT             NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
    SuperHeroName	NVARCHAR(120)   NOT NULL
);

INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( @SuperHeroName1 );

INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( @SuperHeroName2 );";

    // Act
    int rowsAffected = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
        .SetCommandText( sql )
        .AddParameter( "@SuperHeroName1", "Superman", DbType.AnsiString )
        .AddParameter( "@SuperHeroName2", "Batman", DbType.AnsiString )
        .ExecuteNonQuery();

    // Assert
    Assert.That( rowsAffected == 2 );
}

[Test]
public void ExecuteReader_Example()
{
    // Arrange
    const string sql = @"
CREATE TABLE #SuperHero
(
    SuperHeroId     INT             NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
    SuperHeroName	NVARCHAR(120)   NOT NULL
);

INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( 'Superman' );

INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( 'Batman' );

SELECT  SuperHeroId,
        SuperHeroName
FROM    #SuperHero;";

    List<object> list = new List<object>();

    // Act
    Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
        .SetCommandText( sql )
        .ExecuteReader( record =>
        {
            var obj = new
            {
                SuperHeroId = record.GetValue( 0 ),
                SuperHeroName = record.GetValue( 1 )
            };

            list.Add( obj );
        } );

    // Assert
    Assert.That( list.Count == 2 );
}

[Test]
public void ExecuteScalar_Example()
{
    // Arrange
    const string sql = @"
CREATE TABLE #SuperHero
(
    SuperHeroId     INT             NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
    SuperHeroName	NVARCHAR(120)   NOT NULL
);

INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( 'Superman' );

SELECT  SuperHeroId, /* This should be the only value returned from ExecuteScalar */
        SuperHeroName
FROM    #SuperHero;";

    // Act
    int superHeroId = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
        .SetCommandText( sql )
        .ExecuteScalar()
        .ToInt(); // Using one of the many handy Sequelocity helper extension methods

    // Assert
    Assert.That( superHeroId == 1 );
}

[Test]
public void ExecuteScalar_Of_Type_T_Example()
{
    // Arrange
    const string sql = @"
CREATE TABLE #SuperHero
(
    SuperHeroId     INT             NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
    SuperHeroName	NVARCHAR(120)   NOT NULL
);

INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( 'Superman' );

SELECT  SuperHeroId, /* This should be the only value returned from ExecuteScalar */
        SuperHeroName
FROM    #SuperHero;";

    // Act
    int superHeroId = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
        .SetCommandText( sql )
        .ExecuteScalar<int>();

    // Assert
    Assert.That( superHeroId == 1 );
}

[Test]
public void DataSet_Example()
{
    // Arrange
    const string sql = @"
CREATE TABLE #SuperHero
(
    SuperHeroId     INT             NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
    SuperHeroName	NVARCHAR(120)   NOT NULL
);

INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( 'Superman' );

INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( 'Batman' );

SELECT  SuperHeroId,
        SuperHeroName
FROM    #SuperHero;";

    // Act
    DataSet dataSet = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
        .SetCommandText( sql )
        .ExecuteToDataSet();

    // Assert
    Assert.That( dataSet.Tables[0].Rows.Count == 2 );
    Assert.That( dataSet.Tables[0].Rows[0][0].ToString() == "1" );
    Assert.That( dataSet.Tables[0].Rows[0][1].ToString() == "Superman" );
    Assert.That( dataSet.Tables[0].Rows[1][0].ToString() == "2" );
    Assert.That( dataSet.Tables[0].Rows[1][1].ToString() == "Batman" );
}

[Test]
public void DataTable_Example()
{
    // Arrange
    const string sql = @"
CREATE TABLE #SuperHero
(
    SuperHeroId     INT             NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
    SuperHeroName	NVARCHAR(120)   NOT NULL
);

INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( 'Superman' );

INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( 'Batman' );

SELECT  SuperHeroId,
        SuperHeroName
FROM    #SuperHero;";

    // Act
    DataTable dataTable = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
        .SetCommandText( sql )
        .ExecuteToDataTable();

    // Assert
    Assert.That( dataTable.Rows.Count == 2 );
    Assert.That( dataTable.Rows[0][0].ToString() == "1" );
    Assert.That( dataTable.Rows[0][1].ToString() == "Superman" );
    Assert.That( dataTable.Rows[1][0].ToString() == "2" );
    Assert.That( dataTable.Rows[1][1].ToString() == "Batman" );
}

[Test]
public void ExecuteToDynamicList_Example()
{
    // Arrange
    const string sql = @"
CREATE TABLE #SuperHero
(
    SuperHeroId     INT             NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
    SuperHeroName	NVARCHAR(120)   NOT NULL
);

INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( 'Superman' );

INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( 'Batman' );

SELECT  SuperHeroId,
        SuperHeroName
FROM    #SuperHero;";

    // Act
    List<dynamic> superHeroes = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
        .SetCommandText( sql )
        .ExecuteToDynamicList();

    // Assert
    Assert.That( superHeroes.Count == 2 );
    Assert.That( superHeroes[0].SuperHeroId == 1 );
    Assert.That( superHeroes[0].SuperHeroName == "Superman" );
    Assert.That( superHeroes[1].SuperHeroId == 2 );
    Assert.That( superHeroes[1].SuperHeroName == "Batman" );
}

[Test]
public void ExecuteToDynamicObject_Example()
{
    // Arrange
    const string sql = @"
CREATE TABLE #SuperHero
(
    SuperHeroId     INT             NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
    SuperHeroName	NVARCHAR(120)   NOT NULL
);

INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( 'Superman' );

INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( 'Batman' );

SELECT  TOP 1
        SuperHeroId,
        SuperHeroName
FROM    #SuperHero;";

    // Act
    dynamic superHero = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
        .SetCommandText( sql )
        .ExecuteToDynamicObject();

    // Assert
    Assert.NotNull( superHero );
    Assert.That( superHero.SuperHeroId == 1 );
    Assert.That( superHero.SuperHeroName == "Superman" );
}

[Test]
public void ExecuteToList_Example()
{
    // Arrange
    const string sql = @"
CREATE TABLE #SuperHero
(
	SuperHeroId     INT             NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
	SuperHeroName	NVARCHAR(120)   NOT NULL
);

INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( 'Superman' );

INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( 'Batman' );

SELECT  SuperHeroId,
		SuperHeroName
FROM    #SuperHero;";

    // Act
    List<SuperHero> superHeroes = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
        .SetCommandText( sql )
        .ExecuteToList<SuperHero>();

    // Assert
    Assert.That( superHeroes.Count == 2 );
    Assert.That( superHeroes[0].SuperHeroId == 1 );
    Assert.That( superHeroes[0].SuperHeroName == "Superman" );
    Assert.That( superHeroes[1].SuperHeroId == 2 );
    Assert.That( superHeroes[1].SuperHeroName == "Batman" );
}

[Test]
public void ExecuteToMap_Example()
{
    // Arrange
    const string sql = @"
CREATE TABLE #SuperHero
(
    SuperHeroId     INT             NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
    SuperHeroName	NVARCHAR(120)   NOT NULL
);

INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( 'Superman' );

INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( 'Batman' );

SELECT  SuperHeroId,
        SuperHeroName
FROM    #SuperHero;";

    // Act
    List<SuperHero> superHeroes = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
        .SetCommandText( sql )
        .ExecuteToMap( record =>
        {
            var obj = new SuperHero
            {
                SuperHeroId = record.GetValue( 0 ).ToLong(),
                SuperHeroName = record.GetValue( 1 ).ToString()
            };

            return obj;
        } );

    // Assert
    Assert.That( superHeroes.Count == 2 );
    Assert.That( superHeroes[0].SuperHeroId == 1 );
    Assert.That( superHeroes[0].SuperHeroName == "Superman" );
    Assert.That( superHeroes[1].SuperHeroId == 2 );
    Assert.That( superHeroes[1].SuperHeroName == "Batman" );
}

[Test]
public void ExecuteToObject_Example()
{
    // Arrange
    const string sql = @"
CREATE TABLE #SuperHero
(
    SuperHeroId     INT             NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
    SuperHeroName	NVARCHAR(120)   NOT NULL
);

INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( 'Superman' );

INSERT INTO #SuperHero ( SuperHeroName )
VALUES ( 'Batman' );

SELECT  TOP 1
        SuperHeroId,
        SuperHeroName
FROM    #SuperHero;";

    // Act
    SuperHero superHero = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
        .SetCommandText( sql )
        .ExecuteToObject<SuperHero>();

    // Assert
    Assert.NotNull( superHero );
    Assert.That( superHero.SuperHeroId == 1 );
    Assert.That( superHero.SuperHeroName == "Superman" );
}

#endregion Execute Methods

#region Generate Insert Methods

public class Customer
{
    public int? CustomerId; // Setting the primary key as nullable
    public string FirstName;
    public string LastName;
    public DateTime DateOfBirth;
}

[Test]
public void GenerateInsertForSqlServer_Example()
{
    // Arrange
    const string sql = @"
IF ( EXISTS (	SELECT	* 
				FROM	INFORMATION_SCHEMA.TABLES 
				WHERE	TABLE_SCHEMA = 'dbo' 
						AND	TABLE_NAME = 'Customer' ) )
BEGIN
	DROP TABLE Customer
END

IF ( NOT EXISTS (	SELECT	* 
					FROM	INFORMATION_SCHEMA.TABLES 
					WHERE	TABLE_SCHEMA = 'dbo' 
							AND	TABLE_NAME = 'Customer') )
BEGIN
	CREATE TABLE Customer
	(
		CustomerId      INT             NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
		FirstName       NVARCHAR(120)   NOT NULL,
		LastName        NVARCHAR(120)   NOT NULL,
		DateOfBirth     DATETIME        NOT NULL
	);
END
";

    Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
        .SetCommandText( sql )
        .ExecuteNonQuery();

    Customer customer = new Customer { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

    // Act
    int customerId = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
        .GenerateInsertForSqlServer( customer )
        .ExecuteScalar<int>();

    // Assert
    Assert.That( customerId == 1 );
}

[Test]
public void GenerateInsertForSqlServer_Example_Using_An_Anonymous_Type()
{
    // Arrange
    const string sql = @"
CREATE TABLE #Customer
(
	CustomerId      INT             NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
	FirstName       NVARCHAR(120)   NOT NULL,
	LastName        NVARCHAR(120)   NOT NULL,
	DateOfBirth     DATETIME        NOT NULL
);";

    DbConnection dbConnection = Sequelocity.CreateDbConnection( ConnectionStringsNames.SqlServerConnectionString );

    Sequelocity.GetDatabaseCommand( dbConnection )
        .SetCommandText( sql )
        .ExecuteNonQuery( true ); // Passing in 'true' to keep the connection open since this example is using a temp table which only exists during the scope / lifetime of this database connection

    // Anonymous Type
    var customer = new { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };

    // Act
    int customerId = Sequelocity.GetDatabaseCommand( dbConnection )
        .GenerateInsertForSqlServer( customer, "#Customer" ) // Specifying table name since Sequelocity can't use the type name as the table name
        .ExecuteScalar<int>();

    // Assert
    Assert.That( customerId == 1 );
}

[Test]
public void GenerateInsertsForSqlServer_Example()
{
    // Arrange
    const string sql = @"
IF ( EXISTS (	SELECT	* 
				FROM	INFORMATION_SCHEMA.TABLES 
				WHERE	TABLE_SCHEMA = 'dbo' 
						AND	TABLE_NAME = 'Customer' ) )
BEGIN

	DROP TABLE Customer

END

IF ( NOT EXISTS (	SELECT	* 
					FROM	INFORMATION_SCHEMA.TABLES 
					WHERE	TABLE_SCHEMA = 'dbo' 
							AND	TABLE_NAME = 'Customer') )
BEGIN

	CREATE TABLE Customer
	(
		CustomerId      INT             NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
		FirstName       NVARCHAR(120)   NOT NULL,
		LastName        NVARCHAR(120)   NOT NULL,
		DateOfBirth     DATETIME        NOT NULL
	);

END
";
    
    Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
        .SetCommandText( sql )
        .ExecuteNonQuery();

    Customer customer1 = new Customer { FirstName = "Clark", LastName = "Kent", DateOfBirth = DateTime.Parse( "06/18/1938" ) };
    Customer customer2 = new Customer { FirstName = "Bruce", LastName = "Wayne", DateOfBirth = DateTime.Parse( "05/27/1939" ) };
    Customer customer3 = new Customer { FirstName = "Peter", LastName = "Parker", DateOfBirth = DateTime.Parse( "08/18/1962" ) };
    List<Customer> list = new List<Customer> { customer1, customer2, customer3 };

    // Act
    List<long> customerIds = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
        .GenerateInsertsForSqlServer( list )
        .ExecuteToList<long>();

    // Assert
    Assert.That( customerIds.Count == 3 );
    Assert.That( customerIds[0] == 1 );
    Assert.That( customerIds[1] == 2 );
    Assert.That( customerIds[2] == 3 );
}

 #endregion Generate Insert Methods

#region Adding Parameter Methods

public class SuperHero
{
    public long SuperHeroId;
    public string SuperHeroName;
}


public void AddParameter_Example()
{
    List<SuperHero> superHeroes = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
        .SetCommandText( "SELECT * FROM SuperHero WHERE SuperHeroName = @SuperHeroName" )
        .AddParameter( "@SuperHeroName", "Superman" )
        .ExecuteToList<SuperHero>();
}

public void AddParameter_Example_Specifying_An_Explicit_DbType()
{
    List<SuperHero> superHeroes = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
        .SetCommandText( "SELECT * FROM SuperHero WHERE SuperHeroName = @SuperHeroName" )
        .AddParameter( "@SuperHeroName", "Superman", DbType.AnsiString )
        .ExecuteToList<SuperHero>();
}

public void AddParameter_Example_Providing_A_DbParameter()
{
    DatabaseCommand databaseCommand = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString );

    var dbParameter = databaseCommand.DbCommand.CreateParameter();
    dbParameter.ParameterName = "SuperHeroName";
    dbParameter.Value = "Superman";
    dbParameter.Direction = ParameterDirection.InputOutput;

    List<SuperHero> superHeroes = databaseCommand
        .SetCommandText( "SELECT * FROM SuperHero WHERE SuperHeroName = @SuperHeroName" )
        .AddParameter( dbParameter )
        .ExecuteToList<SuperHero>();
}

public void AddParameters_Example_Providing_A_List_Of_Parameter_Values_For_Use_In_An_IN_Clause()
{
    List<string> parameterList = new List<string> { "Superman", "Batman", "Spider-Man" };

    List<SuperHero> superHeroes = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
        .SetCommandText( "SELECT * FROM SuperHero WHERE SuperHeroName IN ( @SuperHeroNames )" )
        .AddParameters( "@SuperHeroNames", parameterList, DbType.AnsiString )
        .ExecuteToList<SuperHero>();
}

public void AddParameters_Example_Providing_A_Dictionary_Of_Parameter_Names_And_Values()
{
    const string sql = @"
SELECT  *
FROM    SuperHero
WHERE   SuperHeroId = @SuperHeroId
        OR SuperHeroName = @SuperHeroName
        OR SuperHeroName LIKE '%@SuperHeroPartialName%'";

    IDictionary<string, object> dictionary = new Dictionary<string, object>
    {
        { "@SuperHeroId", 1 },
        { "@SuperHeroName", "Superman" },
        { "@SuperHeroPartialName", "S" }
    };

    List<SuperHero> superHeroes = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
        .SetCommandText( sql )
        .AddParameters( dictionary )
        .ExecuteToList<SuperHero>();
}

public void AddParameters_Example_Supplying_A_Parameter_Array_Of_DbParameters()
{
    const string sql = @"
SELECT  *
FROM    SuperHero
WHERE   SuperHeroId = @SuperHeroId
        OR SuperHeroName = @SuperHeroName
        OR SuperHeroName LIKE '%@SuperHeroPartialName%'";

    DatabaseCommand databaseCommand = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString );

    DbParameter superHeroIdParameter = databaseCommand.CreateParameter( "@SuperHeroId", 1, DbType.Int32 );
    DbParameter superHeroNameParameter = databaseCommand.CreateParameter( "@SuperHeroName", "Superman", DbType.AnsiString );
    DbParameter superHeroPartialNameParameter = databaseCommand.CreateParameter( "@SuperHeroPartialName", "S", DbType.AnsiString );

    List<SuperHero> superHeroes = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
        .SetCommandText( sql )
        .AddParameters( superHeroIdParameter, superHeroNameParameter, superHeroPartialNameParameter )
        .ExecuteToList<SuperHero>();
}

#endregion Adding Parameter Methods

#region Miscellaneous Helper Methods

// AppendCommandText

public void AppendCommandText_Example()
{
    const string sql = "SELECT TOP 1 * FROM SuperHero;";

    DatabaseCommand databaseCommand = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
        .SetCommandText( sql );

    // Imagine there is some conditional logic here where we need to add additional queries to the database command
    const string moreSql = "\nSELECT TOP 1 * FROM AlterEgo;";

    databaseCommand.AppendCommandText( moreSql );
}

// CreateParameter
// SetCommandTimeout
// SetCommandType
// ToDatabaseCommand
// ToDbCommand

#endregion Miscellaneous Helper Methods

#region Transaction Examples

[Test]
public void BeginTransaction_Example()
{
    const string sqlCommand1 = @"
CREATE TABLE #Customer
(
	CustomerId      INT             NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
	FirstName       NVARCHAR(120)   NOT NULL,
	LastName        NVARCHAR(120)   NOT NULL,
	DateOfBirth     DATETIME        NOT NULL
);

INSERT INTO #Customer VALUES ( 'Clark', 'Kent', '06/18/1938' );
INSERT INTO #Customer VALUES ( 'Bruce', 'Wayne', '05/27/1939' );
";

    const string sqlCommand2 = @"
INSERT INTO #Customer VALUES ( 'Peter', 'Parker', '08/18/1962' );
";

    using ( var databaseCommand = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString ) )
    {
        using ( var transaction = databaseCommand.BeginTransaction() )
        {
            var rowsUpdated = databaseCommand
                .SetCommandText( sqlCommand1 )
                .ExecuteNonQuery( keepConnectionOpen: true );

            var nextRowsUpdated = databaseCommand
                .SetCommandText( sqlCommand2 )
                .ExecuteNonQuery( keepConnectionOpen: true );

            Assert.That( rowsUpdated == 2 && nextRowsUpdated == 1 );

            if ( rowsUpdated == 2 && nextRowsUpdated == 1 )
                transaction.Commit();
        }
    }
}

[Test]
public void TransactionScope_Example()
{
    const string sqlCommand1 = @"
IF ( EXISTS (	SELECT	* 
				FROM	INFORMATION_SCHEMA.TABLES 
				WHERE	TABLE_SCHEMA = 'dbo' 
						AND	TABLE_NAME = 'Customer' ) )
BEGIN
	DROP TABLE Customer
END

IF ( NOT EXISTS (	SELECT	* 
					FROM	INFORMATION_SCHEMA.TABLES 
					WHERE	TABLE_SCHEMA = 'dbo' 
							AND	TABLE_NAME = 'Customer') )
BEGIN
	CREATE TABLE Customer
	(
		CustomerId      INT             NOT NULL    IDENTITY(1,1)   PRIMARY KEY,
		FirstName       NVARCHAR(120)   NOT NULL,
		LastName        NVARCHAR(120)   NOT NULL,
		DateOfBirth     DATETIME        NOT NULL
	);
END

INSERT INTO Customer VALUES ( 'Clark', 'Kent', '06/18/1938' );
INSERT INTO Customer VALUES ( 'Bruce', 'Wayne', '05/27/1939' );
";

    const string sqlCommand2 = @"
INSERT INTO Customer VALUES ( 'Peter', 'Parker', '08/18/1962' );
";

    using ( var transaction = new TransactionScope() )
    {
        var rowsUpdated = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
                .SetCommandText( sqlCommand1 )
                .ExecuteNonQuery();

        var nextRowsUpdated = Sequelocity.GetDatabaseCommand( ConnectionStringsNames.SqlServerConnectionString )
            .SetCommandText( sqlCommand2 )
            .ExecuteNonQuery();

        Assert.That( rowsUpdated == 2 && nextRowsUpdated == 1 );

        if ( rowsUpdated == 2 && nextRowsUpdated == 1 )
            transaction.Complete();
    }
} 

#endregion Transaction Examples

#region Event Handler Examples

[Test]
public void PreExecute_Example()
{
    // Arrange
    string commandText = string.Empty;

    Sequelocity.ConfigurationSettings.EventHandlers.DatabaseCommandPreExecuteEventHandlers.Add( command =>
    {
        if ( command.DbCommand.CommandType == CommandType.Text )
        {
            command.DbCommand.CommandText = "/* Application Name: MyAppName */" + Environment.NewLine + command.DbCommand.CommandText;
            commandText = command.DbCommand.CommandText;
        }
    } );

    // Act
    var id = Sequelocity.GetDatabaseCommandForSqlServer( ConnectionStringsNames.SqlServerConnectionString )
        .SetCommandText( "SELECT 1 as Id" )
        .ExecuteScalar<int>();

    // Visual Assertion
    Trace.WriteLine( commandText );

    // Assert
    Assert.That( commandText.StartsWith( "/* Application Name: MyAppName */" ) );
    Assert.That( id == 1 );

    // Cleanup
    Sequelocity.ConfigurationSettings.EventHandlers.DatabaseCommandPreExecuteEventHandlers.Clear();
}

[Test]
public void PostExecute_Example()
{
    // Arrange
    var dictionary = new ConcurrentDictionary<DatabaseCommand,Stopwatch>();
    long elapsedMilliseconds = 0;

    Sequelocity.ConfigurationSettings.EventHandlers.DatabaseCommandPreExecuteEventHandlers.Add( command =>
    {
        dictionary[command] = Stopwatch.StartNew();
    } );

    Sequelocity.ConfigurationSettings.EventHandlers.DatabaseCommandPostExecuteEventHandlers.Add( command =>
    {
        Stopwatch stopwatch;
        if ( dictionary.TryRemove( command, out stopwatch ) )
            elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
    } );

    // Act
    var id = Sequelocity.GetDatabaseCommandForSqlServer( ConnectionStringsNames.SqlServerConnectionString )
        .SetCommandText( "SELECT 1 as Id" )
        .ExecuteScalar<int>();

    // Visual Assertion
    Trace.WriteLine( "Elapsed Milliseconds: " + elapsedMilliseconds );

    // Assert
    Assert.That( elapsedMilliseconds >= 0 );

    // Cleanup
    Sequelocity.ConfigurationSettings.EventHandlers.DatabaseCommandPreExecuteEventHandlers.Clear();
    Sequelocity.ConfigurationSettings.EventHandlers.DatabaseCommandPostExecuteEventHandlers.Clear();
}

[Test]
public void UnhandledException_Example()
{
    // Arrange
    Exception thrownException = null;

    Sequelocity.ConfigurationSettings.EventHandlers.DatabaseCommandUnhandledExceptionEventHandlers.Add( ( exception, command ) =>
    {
        thrownException = exception;
    } );
            
    try
    {
        var id = Sequelocity.GetDatabaseCommandForSqlServer( ConnectionStringsNames.SqlServerConnectionString )
            .SetCommandText( "SELECT asdasdffsdf as Id" )
            .ExecuteScalar<int>();
    }
    catch ( Exception )
    {
        // ignored
    }

    // Visual Assertion
    Trace.WriteLine( thrownException );

    // Assert
    Assert.NotNull( thrownException.Message.Contains( "Invalid column name 'asdasdffsdf'" ) );

    // Cleanup
    Sequelocity.ConfigurationSettings.EventHandlers.DatabaseCommandUnhandledExceptionEventHandlers.Clear();
}

#endregion Event Handler Examples
    }
}