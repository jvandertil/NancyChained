// include Fake lib
#r @"packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.FixieHelper
open Fake.AssemblyInfoFile

// ---------------------------------------------------
// Directories
// ---------------------------------------------------

let rootDir = FileUtils.pwd()
let sourceDir = rootDir @@ "src"
let testsDir = rootDir @@ "tests"
let buildDir = rootDir @@ "build"

// ---------------------------------------------------
// Version information
// ---------------------------------------------------

let version = "0.1.0";

let buildNumber() =
    let buildNumber = EnvironmentHelper.environVarOrNone "BUILD_NUMBER"
    match buildNumber with
    | Some x -> x
    | None -> "0"

let versionNumber() = sprintf "%s.%s" version (buildNumber())

// ---------------------------------------------------
// Build steps
// ---------------------------------------------------

// Clean output directory
Target "Clean" (fun _ -> 
    CleanDir buildDir
)

// Version assemblies
Target "Version" (fun _ ->
    let commitHash = Git.Information.getCurrentHash()
    let versionNumber = versionNumber()
    let infoVersion = sprintf "%s-%s" versionNumber commitHash

    CreateCSharpAssemblyInfo (sourceDir @@ "SolutionVersion.cs")
        [ Attribute.Version versionNumber
          Attribute.FileVersion versionNumber
          Attribute.InformationalVersion infoVersion ]
)

// Build solution
Target "Build" (fun _ ->
    !! (rootDir @@ "*.sln")
    |> MSBuildRelease buildDir "Build"
    |> Log ""
)

Target "BuildMinimal" (fun _ -> 
    !!(sourceDir @@ "NancyChains" @@ "*.csproj")
    |> MSBuildRelease buildDir "Build"
    |> Log "" 
)

// Run unit tests
Target "Test" (fun _ ->
    !! (buildDir @@ "*.Tests.dll")
    |> Fixie id
)

// Stub target
Target "Default" (fun _ ->
    ()
)

// Setup chain of dependencies.
"Clean"
==> "Version"
==> "Build"
==> "Test"
==> "Default"

"Clean"
==> "Version"
==> "BuildMinimal"

// start build
RunTargetOrDefault "Default"