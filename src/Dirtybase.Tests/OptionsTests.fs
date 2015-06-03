open NUnit.Framework
open FsUnit
open Dirtybase

[<TestFixture>]
type OptionsTests() = 

    [<Test>]
    member this.``Given init Command When Parsing Options Command Type Should Be Init`` () =
        let options = Options.parse "-c init"
        options.command |> should equal Options.CommandType.Init

    [<Test>]
    member this.``Given migrate Command When Parsing Options Command Type Should Be Migrate`` () =
        let options = Options.parse "-c migrate"
        options.command |> should equal Options.CommandType.Migrate

    [<Test>]
    member this.``Given unknown Command When Parsing Options Command Type Should Be Unknown`` () =
        let options = Options.parse "-c foo"
        options.command |> should equal Options.CommandType.Unknown

    [<Test>]
    member this.``Given no Command When Parsing Options Command Type Should Be Help`` () =
        let options = Options.parse ""
        options.command |> should equal Options.CommandType.Help
