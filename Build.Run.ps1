dotnet clean
dotnet build

Start-Process "http://localhost:5000/api/blog/"

dotnet .\WebApplication1\bin\Debug\netcoreapp2.0\WebApplication1.dll

#to run with an active story run the following line instead of the line above
#dotnet .\WebApplication1\bin\Debug\netcoreapp2.0\WebApplication1.dll --active-stories:0=1

