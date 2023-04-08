dotnet restore

dotnet build TauCode.Data.sln -c Debug
dotnet build TauCode.Data.sln -c Release

dotnet test TauCode.Data.sln -c Debug
dotnet test TauCode.Data.sln -c Release

nuget pack nuget\TauCode.Data.nuspec