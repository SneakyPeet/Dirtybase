namespace Dirtybase

module VersionComparison =

    type Version = {
        fileName :string;
        number :string;
    }

    let private getVersionFromFileName (file:string) =
        let versionNumber = file.Substring(1,file.IndexOf('_') - 1) //grabs string between v and _
        {fileName= file; number = versionNumber}

    //Assumes Filenames are in the correct format of v[number]_[name].[extention]
    let getVersionsFromFileNames (names:string[]) =
        names
        |> Array.toList
        |> List.map getVersionFromFileName

    //Assumes Duplicates Have Been Removed
    let extractVersionsToApply versions existingVersions =
        (Set.ofList versions) - (Set.ofList existingVersions) //this is awesome -> It gets all items not in both lists