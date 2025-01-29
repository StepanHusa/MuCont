# Paths
$buildOutput = "./build"
$apiPackage = "./MuContAPI"
$desktopSolution = "./MuContDesktop/MuContDesktopSolution.sln"
$desktopProject = "./MuContDesktop/MuCont.Desktop\MuCont.Desktop.csproj"
$juliaSysImage = "$buildOutput/MuContAPIimage.so"
$juliaRuntime = "julia"

# Ensure the build directory exists
if (-not (Test-Path $buildOutput)) {
    New-Item -ItemType Directory -Path $buildOutput -Force | Out-Null
}

Write-Host "Step 1: Precompiling Julia system image..."
# Precompile the API package system image
julia --project=./$apiPackage -e "using PackageCompiler; PackageCompiler.create_sysimage([:MuContAPI], sysimage_path=`"$juliaSysImage`")"

if ($LASTEXITCODE -ne 0) {
    Write-Error "Julia API system image compilation failed."
    exit 1
}

Write-Host "Step 2: Bundling Julia runtime..."
# Bundle the Julia runtime into the C# app output folder
if (Test-Path $juliaRuntime) {
    Copy-Item -Recurse -Force $juliaRuntime $buildOutput
} else {
    Write-Warning "Julia runtime folder not found. Skipping runtime bundling."
}

Write-Host "Step 3: Building C# application..."
# Build the C# application
dotnet publish ./$desktopProject -c Release -r win-x64 --self-contained true -o $buildOutput



if ($LASTEXITCODE -ne 0) {
    Write-Error "C# application build failed."
    exit 1
}

Write-Host "Build completed successfully!"
