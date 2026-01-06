# Ticketing System

System zarządzania zgłoszeniami (ticketing system) zbudowany w .NET 8.0.

## Wymagania

- .NET 8.0 SDK lub nowszy
- System operacyjny: Windows, Linux lub macOS

## Instalacja .NET

### Windows

1. Pobierz instalator .NET 8.0 SDK z [oficjalnej strony Microsoft](https://dotnet.microsoft.com/download/dotnet/8.0)
2. Uruchom instalator i postępuj zgodnie z instrukcjami
3. Zweryfikuj instalację, otwierając terminal i wpisując:
   ```bash
   dotnet --version
   ```
   Powinieneś zobaczyć wersję 8.0.x lub nowszą

### Linux (Ubuntu/Debian)

```bash
# Dodaj repozytorium Microsoft
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Zainstaluj .NET SDK
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0

# Zweryfikuj instalację
dotnet --version
```

### macOS

```bash
# Używając Homebrew
brew install --cask dotnet-sdk

# Lub pobierz instalator z oficjalnej strony Microsoft
# https://dotnet.microsoft.com/download/dotnet/8.0

# Zweryfikuj instalację
dotnet --version
```

## Uruchamianie aplikacji w trybie deweloperskim

### 1. Przywróć zależności NuGet

```bash
dotnet restore
```

### 2. Zbuduj projekt

```bash
dotnet build
```

### 3. Uruchom aplikację

```bash
dotnet run
```

Aplikacja automatycznie otworzy przeglądarkę z interfejsem Swagger.

### 4. Sprawdź, czy aplikacja działa

Po uruchomieniu powinieneś zobaczyć w konsoli informację podobną do:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5211
```

## Dostęp do Swagger UI

### Automatyczne otwarcie

Jeśli uruchomiłeś aplikację za pomocą `dotnet run`, przeglądarka powinna automatycznie otworzyć się na stronie Swagger.

### Ręczne otwarcie

1. Upewnij się, że aplikacja jest uruchomiona
2. Otwórz przeglądarkę i przejdź do jednego z poniższych adresów:

   **HTTP:**
   ```
   http://localhost:5211/swagger
   ```

   **HTTPS (jeśli używasz profilu https):**
   ```
   https://localhost:7180/swagger
   ```

### Funkcje Swagger UI

- **Przeglądanie wszystkich endpointów API** - lista wszystkich dostępnych operacji
- **Testowanie endpointów** - możliwość wywołania API bezpośrednio z interfejsu
- **Dokumentacja schematów** - szczegółowe informacje o modelach danych
- **Walidacja enumów** - dla pól typu string reprezentujących enumy (Category, Priority, Status, itp.) dostępne są listy rozwijane z dostępnymi wartościami
- **Upload plików** - możliwość przesyłania plików przez interfejs Swagger

## Porty aplikacji

- **HTTP:** `http://localhost:5211`
- **HTTPS:** `https://localhost:7180`

## Struktura projektu

```
TicketingSystem/
├── Application/          # Warstwa aplikacji (DTOs, Mappers, Services, Validators)
├── Domain/              # Warstwa domenowa (Aggregates, Enums, Policies, Validators)
├── Infrastructure/      # Warstwa infrastruktury (Middleware, Persistence, Swagger)
├── Presentation/        # Warstwa prezentacji (Controllers)
└── Program.cs           # Punkt wejścia aplikacji
```

## Technologie

- **.NET 8.0** - Framework aplikacji
- **ASP.NET Core** - Framework webowy
- **Swashbuckle.AspNetCore** - Generowanie dokumentacji Swagger/OpenAPI
- **FluentValidation** - Walidacja danych
