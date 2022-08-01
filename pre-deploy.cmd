dotnet restore

dotnet build --configuration Debug
dotnet build --configuration Release

dotnet test -c Debug .\test\TauCode.Data.Tests\TauCode.Data.Tests.csproj
dotnet test -c Release .\test\TauCode.Data.Tests\TauCode.Data.Tests.csproj

nuget pack nuget\TauCode.Data.nuspec