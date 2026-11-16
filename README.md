# RwaStay

RwaStay is a .NET 8 Windows Forms stay and hotel booking system for Rwanda. It uses MVC architecture, MySqlConnector, and plain parameterized ADO.NET queries.

## Requirements

- Windows
- Visual Studio 2022/2026 with the .NET desktop development workload
- .NET 8 SDK
- XAMPP MySQL and phpMyAdmin

## Database setup

1. Start **MySQL** in XAMPP.
2. Open `http://localhost/phpmyadmin`.
3. Open the **SQL** tab.
4. Paste and execute `database/rwastaydb.sql`.
5. The app expects database `rwastaydb`, MySQL user `root`, and an empty password on `localhost:3306`.

If your MySQL credentials are different, update the connection string in `lab3Rwastay/Data/DbConnection.cs`.

## Run the application

From the repository root:

```powershell
dotnet restore .\lab3Rwastay\lab3Rwastay.csproj
dotnet build .\lab3Rwastay\lab3Rwastay.csproj
dotnet run --project .\lab3Rwastay\lab3Rwastay.csproj
```

You can also open `lab3Rwastay.slnx` in Visual Studio and press **F5**.

## Features

- Host, stay, and guest CRUD management
- Search hosts, stays, and guests
- Booking creation with guest/stay selection
- Automatic total price calculation
- Booking status changes and cancellation
- Required field and email validation
- Check-out date validation
- Overlap prevention for active bookings
- Friendly MySQL connection error messages

## MVC structure

- `Models/` - plain domain classes and `BookingStatus`
- `Data/` - MySQL connection helper and repositories containing SQL
- `Controllers/` - validation and business rules
- `Views/` - WinForms screens; views do not contain SQL

## GitHub submission

```powershell
git init
git add .
git commit -m "Build RwaStay booking system"
git branch -M main
git remote add origin https://github.com/YOUR_USERNAME/YOUR_REPOSITORY.git
git push -u origin main
```

Replace the remote URL with your GitHub repository URL.
