**AbySalto – Restaurant Orders (ASP.NET Core Web API)**

**Zahtjevi**
-.NET SDK 9
-SQL Server 

Konfiguracija

U appsettings.Development.json postavite connection string:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AbySalto;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}


Po potrebi zamijenite Server= (npr. .\SQLEXPRESS ili (localdb)\\MSSQLLocalDB).

**Inicijalizacija baze**

Visual Studio → Package Manager Console:

Update-Database


**Pokretanje**

Visual Studio: postaviti AbySalto.Junior kao startup projekt → F5

CLI:

dotnet run --project AbySalto.Junior

**URL-ovi**

Swagger: https://localhost:port

Napomena: Port se prikazuje u konzoli pri pokretanju aplikacije.
