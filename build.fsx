// --------------------------------------------------------------------------------------
// FAKE build script
// --------------------------------------------------------------------------------------

#r @"packages/FAKE/tools/FakeLib.dll"
open Fake
open System

let platformTool tool winTool =
    let tool = if isUnix then tool else winTool
    tool
    |> ProcessHelper.tryFindFileOnPath
    |> function Some t -> t | _ -> failwithf "%s not found" tool
let npmTool = platformTool "npm" "npm.cmd"

let ts2fable = platformTool "ts2fable" "ts2fable.cmd"

let run' timeout cmd args dir =
    if execProcess (fun info ->
        info.FileName <- cmd
        if not (String.IsNullOrWhiteSpace dir) then
            info.WorkingDirectory <- dir
        info.Arguments <- args
    ) timeout |> not then
        failwithf "Error while running '%s' with args: %s" cmd args

let run = run' System.TimeSpan.MaxValue

Target "Gen" (fun _ ->
    run npmTool "install ts2fable" ""
    run ts2fable 
                (@".\pixi-typescript-4.5.2\pixi.js.d.ts" 
                + " > " 
                + @"Fable.Import.PIXI.fs") ""
)

RunTargetOrDefault "Gen"