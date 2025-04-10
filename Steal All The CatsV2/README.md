# Steal All The Cats

This is an ASP.NET Core Web API project that interacts with the Cats as a Service (CaaS) API (https://thecatapi.com/) to fetch and store cat images and their associated tags.

## Prerequisites

- .NET 8 SDK
- Microsoft SQL Server
- SQL Server Management Studio (SSMS) or another SQL client (optional, for database management)

## Setup Instructions

1. **Clone or Extract the Project**:
   - Extract the ZIP file to a directory of your choice.

3. Change the Connection String to your own in appsettings.json.

4. **Set Up the Database**:
   - use dotnet ef migrations add 
	- use dotnet ef update

5. Build the solution.

6. Run it. 