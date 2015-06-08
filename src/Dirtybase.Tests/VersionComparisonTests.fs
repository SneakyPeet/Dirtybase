namespace tests

open NUnit.Framework
open Dirtybase.VersionComparison

[<TestFixture>]
type VersionComparisonTests() = 

    [<Test>]
    member this.``Given Array of Filenames extract Version Numbers`` () =
        let filenames = [|"v1_foo.sql"; "V2.1_foo.sql"; "va1_foo.sql"|]
        let expectedVersions = [
            {fileName = "v1_foo.sql"; number = "1"};
            {fileName = "V2.1_foo.sql"; number = "2.1"};
            {fileName = "va1_foo.sql"; number = "a1"}]
        let versions = getVersionsFromFileNames filenames
        CollectionAssert.AreEqual(expectedVersions,versions)

    [<Test>]
    member this.``Given list of Versions When Comparing with existing versions Then return list of versions not applied`` () =
        let versions = [
            {fileName = "v1_foo.sql"; number = "1"};
            {fileName = "V2.1_foo.sql"; number = "2.1"};
            {fileName = "v2_foo.sql"; number = "2"};
            {fileName = "va1_foo.sql"; number = "a1"}]
        let appliedVersions = [
            {fileName = "v1_foo.sql"; number = "1"}; 
            {fileName = "va1_foo.sql"; number = "a1"}]
        let expectedVersions = [
            {fileName = "V2.1_foo.sql"; number = "2.1"};
            {fileName = "v2_foo.sql"; number = "2"}]

        let versionsToApply = extractVersionsToApply versions appliedVersions
        CollectionAssert.AreEqual(expectedVersions,versionsToApply)

