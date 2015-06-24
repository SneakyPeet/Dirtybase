namespace tests

open NUnit.Framework
open Dirtybase.Versioning
open System.IO
open System.Data.SQLite
open FSharp.Data.Sql

[<TestFixture>]
type InitializationTests() = 

    let sqliteFileName = "dirtybaseIntegrationTest.db"
    let versionTableName = "dirtybase.versions"

    [<SetUp>]
    member this.setup () =
        let dbFileExists = File.Exists sqliteFileName
        match dbFileExists with
        | true -> File.Delete sqliteFileName
        | false -> ()
        SQLiteConnection.CreateFile sqliteFileName
    
    [<Test>]
    member this.``When Initializing a New Dirtybase, Version Tables Should Be Added`` () =
        initialize sqliteFileName

