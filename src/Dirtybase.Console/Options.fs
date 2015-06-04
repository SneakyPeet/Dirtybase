namespace Dirtybase

module Options =

    type CommandType = Init | Migrate | Help | Unknown

    type T = {
        command: CommandType;
    }

    let private defaultOptions =
        {command = Help}

    let rec private parse' optionsSoFar args=
        match args with
        | [] ->
            optionsSoFar
        | "-c"::xs ->
            match xs with
            | "init"::xss ->
                parse' {optionsSoFar with command=Init} xss
            | "migrate"::xss ->
                parse' {optionsSoFar with command=Migrate} xss
            | _ -> {optionsSoFar with command=Unknown}
        | _ -> optionsSoFar

    let parse (args:string) =
        args.Split(' ')
        |> Array.toList
        |> parse' defaultOptions

