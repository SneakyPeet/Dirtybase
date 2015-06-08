namespace tests

open NUnit.Framework
open Dirtybase

[<TestFixture>]
type OptionsTests() = 

    [<Test>]
    member this.``Given init Command When Parsing Options Command Type Should Be Init`` () =
        let options = Options.parse "-c init"
        Assert.AreEqual(Options.CommandType.Init,options.command)

    [<Test>]
    member this.``Given migrate Command When Parsing Options Command Type Should Be Migrate`` () =
        let options = Options.parse "-c migrate"
        Assert.AreEqual(Options.CommandType.Migrate,options.command)

    [<Test>]
    member this.``Given unknown Command When Parsing Options Command Type Should Be Unknown`` () =
        let options = Options.parse "-c foo"
        Assert.AreEqual(Options.CommandType.Unknown, options.command)

    [<Test>]
    member this.``Given no Command When Parsing Options Command Type Should Be Help`` () =
        let options = Options.parse ""
        Assert.AreEqual(Options.CommandType.Help, options.command)
