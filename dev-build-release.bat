dotnet restore

dotnet build --configuration Debug
dotnet build --configuration Release


dotnet test -c Debug .\tests\TauCode.Data.Tests\TauCode.Data.Tests.csproj
dotnet test -c Release .\tests\TauCode.Data.Tests\TauCode.Data.Tests.csproj

nuget pack nuget\TauCode.Data.nuspec