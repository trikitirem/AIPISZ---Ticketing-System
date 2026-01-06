# Plan Implementacji - System Zarządzania Zgłoszeniami

## 📋 Przegląd Projektu

System zarządzania zgłoszeniami oparty na **Domain-Driven Design (DDD)** z architekturą 4-warstwową:
- **Domain Layer** - Logika biznesowa, agregaty, value objects, policies
- **Application Layer** - Services, DTOs, Mappers, Validators
- **Infrastructure Layer** - Repositories (file-based), Middleware
- **Presentation Layer** - Controllers (REST API)

## 🎯 Faza 1: Fundamenty Domain Layer

### 1.1 Base Classes (Domain.Base) ✅
- [x] `Entity<TId>` - klasa bazowa dla wszystkich encji
  - Id, Create<T>(), ToPrimitive(), FromPrimitive<T>(), Equals(), GetHashCode()
- [x] `ValueObject` - klasa bazowa dla value objects
  - Equals(ValueObject), Equals(object), GetHashCode()
- [x] `AggregateRoot<TId>` - klasa bazowa dla agregatów
  - _uncommittedChanges, GetUncommittedChanges(), ClearUncommittedChanges(), RaiseEvent()

### 1.2 Enums ✅
- [x] `TicketStatus` - NOWE, PRZYPISANE, W_TOKU, OCZEKUJE_NA_ODPOWIEDZ, GOTOWE_DO_WERYFIKACJI, ESKALOWANE, ZAMKNIETE
- [x] `PriorityLevel` - NISKI, SREDNI, WYSOKI, KRYTYCZNY
- [x] `ResolutionType` - ROZWIAZANE, OBEJSCIE_PROBLEMU, NIE_MOZNA_ODTWORZYC
- [x] `TicketCategory` - IT, HR, FINANCE, GENERAL, OTHER
- [x] `EscalationType` - WORKER_INITIATED, SLA_TIMEOUT, AUTO_ESCALATION, ADMIN_INITIATED
- [x] `AccountStatusEnum` - ACTIVE, INACTIVE, SUSPENDED
- [x] `UserType` - WORKER, SPECIALIST, ADMINISTRATOR

### 1.3 Value Objects ✅
- [x] `TicketNumber` - ValueObject z Value property
- [x] `Priority` - ValueObject z Level (PriorityLevel)
- [x] `AccountStatus` - ValueObject z Status (AccountStatusEnum)
- [x] `Result<T>` - ValueObject dla wyników operacji (IsSuccess, Value, Error)

### 1.4 Domain Exceptions ✅
- [x] `DomainException` - klasa bazowa (Code, Message, Details)
- [x] `ForbiddenException` (403)
- [x] `UnauthorizedException` (401)
- [x] `NotFoundException` (404)
- [x] `ValidationException` (400)
- [x] `ConflictException` (409)
- [x] `InternalServerException` (500)

## 🎯 Faza 2: Agregaty Domain Layer

### 2.1 Ticket Aggregate ✅
- [x] `Ticket` - AggregateRoot<string>
  - Properties: id, number, title, description, status, priority, category, assignedTeamId, assignedSpecialistId, createdById
  - Collections: _resolution, _comments, _escalations, _attachments, _history, _satisfaction, _sla
  - Timestamps: createdAt, updatedAt, resolvedAt
  - Methods: ChangeStatus(), AssignTo(), AssignToTeam(), MarkAsReadyForVerification(), Escalate(), AddComment(), AddAttachment(), AddEscalation(), RecordChange(), RecordSatisfaction(), IsValid(), GetComments(), GetEscalationCount(), WasReproducedBefore()
  - Factory: `Create()` - statyczna metoda z walidacją FluentValidation

- [x] `Comment` - Entity (id, authorId, content, isInternal, createdAt)
- [x] `Resolution` - ValueObject (type, description, tags, createdAt) + Create() factory
- [x] `Escalation` - Entity (id, reason, previousPriority, newPriority, escalatedAt, escalatedBy, escalationType) + Create() factory
- [x] `Attachment` - Entity (id, fileName, fileSize, mimeType, uploadedAt, uploadedBy) + GetStoragePath()
- [x] `HistoryChange` - Entity (id, changedAt, changeType, previousValue, newValue, performedBy, description)
- [x] `Satisfaction` - Entity (id, rating, comment, filledAt, isProblemResolved)
- [x] `SLA` - ValueObject (reactionTime, resolutionTime, priority) + GetReactionTime(), GetResolutionTime()

### 2.2 User Aggregate ✅
- [x] `User` - abstract AggregateRoot<string>
  - Properties: id, email, firstName, lastName, passwordHash, accountStatus
  - Methods: GetEmail(), GetFullName(), IsActive(), GetUserType() (abstract)

- [x] `SupportSpecialist` - dziedziczy z User
  - Properties: teamId, specialization, activeTicketLimit, currentActiveCount
  - Methods: CanAcceptMoreTickets(), IncrementActiveTickets(), DecrementActiveTickets()

- [x] `Administrator` - dziedziczy z User
  - Methods: EscalateTicket(), ManagePolicies()

- [x] `Worker` - dziedziczy z User
  - Methods: CanEscalateTicket()

### 2.3 Team Aggregate ✅
- [x] `Team` - AggregateRoot<string>
  - Properties: id, name, specialization, maxTickets, specialistIds
  - Methods: AddSpecialist(), RemoveSpecialist(), CanAcceptMore(), GetSpecialistCount()

## 🎯 Faza 3: Policies (Domain.Policies)

### 3.1 Policy Base ✅
- [x] `Policy` - klasa abstrakcyjna z Success() i Failure() helper methods

### 3.2 Business Policies ✅
- [x] `ResolutionPolicy`
  - `CanAcceptResolution(ticket, resolution, specialist): Result<bool>`
  - Zapobiega "NIE_MOZNA_ODTWORZYC" dla łatwo odtwarzalnych problemów

- [x] `EscalationPolicy`
  - `ShouldAutoEscalateTicket(ticket, status, deadline): Result<bool>`
  - Auto-eskalacja przy SLA timeout lub 3+ nieudanych próbach

- [x] `WorkerEscalationPolicy`
  - `CanWorkerEscalate(ticket, worker, reason): Result<bool>`
  - Worker może eskalować TYLKO ze statusu GOTOWE_DO_WERYFIKACJI
  - Tylko creator ticketu może eskalować
  - Wymaga powodu eskalacji

- [x] `SpecialistResolutionPolicy`
  - `CanMarkAsReadyForVerification(ticket, specialist, resolution): Result<bool>`
  - Specialist może oznaczyć jako READY tylko jeśli jest przypisany
  - Musi być w statusie W_TOKU
  - Używa ResolutionPolicy wewnętrznie

- [x] `TicketStatusPolicy`
  - `CanTransitionTo(current, target, performedBy): Result<bool>`
  - Waliduje przejścia między statusami

## 🎯 Faza 4: Infrastructure Layer

### 4.1 Repositories ✅
- [x] `IRepository<T, TId>` - interfejs
  - GetByIdAsync(), SaveAsync(), GetAllAsync(), DeleteAsync()

- [x] `FileBasedRepository<T, TId>` - klasa abstrakcyjna
  - _dataFilePath, _logger
  - LoadFromFile(), SaveToFile()
  - Implementacja IRepository

- [x] `TicketRepository` - dziedziczy z FileBasedRepository
  - _dataFilePath: "Data/tickets.json"
  - GetByNumberAsync(), GetByStatusAsync(), GetByAssignedSpecialistAsync(), GetByTeamAsync(), GetByCategoryAsync()

- [x] `UserRepository` - dziedziczy z FileBasedRepository
  - _dataFilePath: "Data/users.json"
  - GetByEmailAsync(), GetByUserTypeAsync(), GetSpecialistsByTeamAsync()

- [x] `TeamRepository` - dziedziczy z FileBasedRepository
  - _dataFilePath: "Data/teams.json"
  - GetBySpecializationAsync()

- [x] `AttachmentRepository`
  - _uploadDirectory: "Data/uploads"
  - SaveFileAsync(), GetFileAsync(), DeleteFileAsync()

### 4.2 Middleware ✅
- [x] `ExceptionHandlingMiddleware`
  - Przechwytuje DomainException
  - Zwraca JSON response z odpowiednim HTTP status code
  - Loguje błędy

## 🎯 Faza 5: Application Layer

### 5.1 DTOs (Application.DTOs) ✅
- [x] Request DTOs:
  - `CreateTicketRequest` (title, description, category, priority)
  - `MarkAsReadyForVerificationRequest` (resolutionDescription, resolutionType)
  - `ReviewResolutionRequest` (accepted, reviewComment)
  - `EscalateTicketRequest` (escalationReason)
  - `AddCommentRequest` (content, isInternal)
  - `UploadAttachmentRequest` (file)

- [x] Response DTOs:
  - `TicketDTO` (id, number, title, status, priority, category, assignedSpecialistId, createdAt, updatedAt)
  - `TicketDetailDTO` : TicketDTO (description, comments, attachments, history, escalations, resolution)
  - `CommentDTO` (id, authorId, content, isInternal, createdAt)
  - `EscalationDTO` (id, reason, escalatedBy, escalationType, createdAt)
  - `UserDTO` (id, email, firstName, lastName, userType, accountStatus)
  - `SupportSpecialistDTO` : UserDTO (teamId, activeTicketCount, activeTicketLimit)
  - `TeamDTO` (id, name, specialization, specialistCount)
  - `AttachmentDTO` (id, fileName, fileSize, mimeType, uploadedAt, uploadedBy)
  - `HistoryChangeDTO` (id, changedAt, changeType, previousValue, newValue, performedBy, description)
  - `ResolutionDTO` (type, description, tags, createdAt)

### 5.2 Validators (Application.Validators) ✅
- [x] `CreateTicketRequestValidator` - FluentValidation
  - Title: NotEmpty, MinLength(5), MaxLength(200)
  - Description: NotEmpty, MinLength(10), MaxLength(5000)
  - Category: NotEmpty, IsEnumName
  - Priority: NotEmpty, IsEnumName

- [x] `MarkAsReadyForVerificationRequestValidator`
  - ResolutionDescription: NotEmpty, MinLength(10)
  - ResolutionType: NotEmpty, IsEnumName

- [x] `ReviewResolutionRequestValidator`
  - ReviewComment: NotEmpty, MinLength(5), MaxLength(5000)

- [x] `EscalateTicketRequestValidator`
  - EscalationReason: NotEmpty, MinLength(10), MaxLength(5000)

- [x] `AddCommentRequestValidator`
  - Content: NotEmpty, MinLength(1), MaxLength(5000)

### 5.3 Mappers (Application.Mappers) ✅
- [x] `TicketMapper`
  - Map(ticket): TicketDTO
  - MapDetail(ticket, comments): TicketDetailDTO
  - MapList(tickets): List<TicketDTO>

- [x] `UserMapper`
  - Map(user): UserDTO
  - MapSpecialist(specialist): SupportSpecialistDTO
  - MapList(users): List<UserDTO>

- [x] `TeamMapper`
  - Map(team): TeamDTO
  - MapList(teams): List<TeamDTO>

- [x] `CommentMapper`
  - Map(comment): CommentDTO
  - MapList(comments): List<CommentDTO>

### 5.4 Services (Application.Services) ✅
- [x] `TicketService`
  - Dependencies: TicketRepository, UserRepository, TeamRepository, ResolutionPolicy, EscalationPolicy, WorkerEscalationPolicy, SpecialistResolutionPolicy, AttachmentRepository, TicketMapper
  - Methods:
    - `CreateTicketAsync(...): Task<Ticket>`
    - `GetTicketByIdAsync(...): Task<Ticket>`
    - `MarkAsReadyForVerificationAsync(...): Task` ⭐
    - `ReviewResolutionAsync(...): Task` ⭐
    - `EscalateTicketAsync(...): Task` ⭐
    - `AddCommentAsync(...): Task`
    - `UploadAttachmentAsync(...): Task`
    - `AssignTicketAsync(...): Task`
    - `ChangeTicketStatusAsync(...): Task`

- [x] `UserService`
  - Dependencies: UserRepository, UserMapper
  - Methods:
    - `RegisterUserAsync(...): Task<User>`
    - `GetUserByIdAsync(...): Task<User>`
    - `GetUserByEmailAsync(...): Task<User>`
    - `AuthenticateAsync(...): Task<User>`

- [x] `TeamService`
  - Dependencies: TeamRepository, UserRepository, TeamMapper
  - Methods:
    - `CreateTeamAsync(...): Task<Team>`
    - `GetTeamByIdAsync(...): Task<Team>`
    - `AddSpecialistToTeamAsync(...): Task`
    - `RemoveSpecialistFromTeamAsync(...): Task`

## 🎯 Faza 6: Presentation Layer

### 6.1 Controllers (Presentation.Controllers) ✅
- [x] `TicketsController`
  - Dependencies: TicketService, TicketMapper
  - Endpoints:
    - `POST /api/tickets` - CreateTicket
    - `GET /api/tickets/{id}` - GetTicket
    - `POST /api/tickets/{id}/mark-ready-for-verification` ⭐
    - `POST /api/tickets/{id}/review-resolution` ⭐
    - `POST /api/tickets/{id}/escalate` ⭐
    - `POST /api/tickets/{id}/comments` - AddComment
    - `POST /api/tickets/{id}/attachments` - UploadAttachment
    - `PUT /api/tickets/{id}/assign` - AssignTicket
    - `PUT /api/tickets/{id}/status` - ChangeStatus

- [x] `UsersController`
  - Dependencies: UserService, UserMapper
  - Endpoints:
    - `POST /api/users/register` - RegisterUser
    - `GET /api/users/{id}` - GetUser
    - `POST /api/users/login` - Login

- [x] `TeamsController`
  - Dependencies: TeamService, TeamMapper
  - Endpoints:
    - `POST /api/teams` - CreateTeam
    - `GET /api/teams/{id}` - GetTeam
    - `GET /api/teams/{id}/members` - GetTeamMembers
    - `POST /api/teams/{id}/specialists` - AddSpecialist

## 🎯 Faza 7: Konfiguracja i Integracja

### 7.1 Dependency Injection (Program.cs) ✅
- [x] Zarejestruj wszystkie Repositories
- [x] Zarejestruj wszystkie Services
- [x] Zarejestruj wszystkie Mappers
- [x] Zarejestruj wszystkie Policies
- [x] Zarejestruj FluentValidation validators
- [x] Zarejestruj ExceptionHandlingMiddleware

### 7.2 Folder Structure ✅
- [x] Utwórz strukturę folderów:
  ```
  Domain/
    Base/
    Aggregates/
      Ticket/
      User/
      Team/
    Policies/
    Exceptions/
  Application/
    DTOs/
    Validators/
    Mappers/
    Services/
  Infrastructure/
    Persistence/
    Middleware/
  Presentation/
    Controllers/
  Data/ (dla plików JSON) ✅
  Data/uploads/ (dla załączników) ✅
  ```

### 7.3 Data Files ✅
- [x] Utwórz puste pliki JSON:
  - `Data/tickets.json` - [] (tworzone automatycznie przez TicketRepository)
  - `Data/users.json` - [] (tworzone automatycznie przez UserRepository)
  - `Data/teams.json` - [] (tworzone automatycznie przez TeamRepository)
  
  **Uwaga:** Pliki są automatycznie tworzone przez FileBasedRepository przy starcie aplikacji, więc nie wymagają ręcznego utworzenia.

## 🎯 Faza 8: Testowanie i Weryfikacja

### 8.1 Testy End-to-End
- [ ] Test przepływu: Worker tworzy ticket → Specialist przypisuje → Specialist pracuje → Specialist oznacza jako READY → Worker akceptuje
- [ ] Test eskalacji: Worker tworzy ticket → Specialist oznacza jako READY → Worker eskaluje → Admin przejmuje
- [ ] Test auto-eskalacji: SLA timeout → automatyczna eskalacja
- [ ] Test policies: ResolutionPolicy blokuje nieprawidłowe rozwiązanie

### 8.2 Walidacja
- [ ] FluentValidation działa poprawnie
- [ ] Domain validation działa poprawnie
- [ ] Policies działają poprawnie

## 📝 Uwagi Implementacyjne

### Kluczowe Zasady:
1. **Private Constructors** - Wszystkie agregaty mają prywatne konstruktory
2. **Factory Methods** - Używamy `Create()` do tworzenia obiektów domenowych
3. **FluentValidation** - Walidacja Request DTOs oraz walidacja w klasach domenowych (Value Objects, Entities, Aggregates)
4. **Domain Validation** - Wewnętrzna walidacja w `Create()` methods używając FluentValidation
5. **Services Orchestrate** - Services koordynują: Fetch → Policy → Domain → Persist
6. **Policies Synchronous** - Policies nie mają dependencies na DB
7. **Mappers Transform** - Mappers konwertują Model → DTO
8. **Repositories Abstract** - Repositories ukrywają persystencję
9. **Proste Komentarze** - Komentarze XML tylko przy klasach i interfejsach. Usuwamy jednozdaniowe, krótkie komentarze z metod i właściwości
10. **Wyjątki Domenowe** - Używamy tylko wyjątków z Domain/Exceptions. Komunikaty błędów w stylu "XXX_DATA_VALIDATION_ERROR" (np. "RESOLUTION_DATA_VALIDATION_ERROR")

### ⭐ Oznaczone metody są kluczowe dla workflow eskalacji:
- `MarkAsReadyForVerificationAsync()` - Specialist oznacza jako gotowe
- `ReviewResolutionAsync()` - Worker przegląda i akceptuje/eskałuje
- `EscalateTicketAsync()` - Worker inicjuje eskalację

## 🚀 Kolejność Implementacji (Rekomendowana)

1. **Faza 1** - Fundamenty (Base classes, Enums, Exceptions)
2. **Faza 2** - Agregaty (Ticket, User, Team)
3. **Faza 3** - Policies
4. **Faza 4** - Infrastructure (Repositories, Middleware)
5. **Faza 5** - Application (DTOs, Validators, Mappers, Services)
6. **Faza 6** - Presentation (Controllers)
7. **Faza 7** - Konfiguracja i Integracja
8. **Faza 8** - Testowanie

---

**Status:** W trakcie implementacji ✅
**Wykonane fazy:**
- ✅ Faza 1: Fundamenty Domain Layer (1.1 Base Classes, 1.2 Enums, 1.3 Value Objects, 1.4 Domain Exceptions)
- ✅ Faza 2.1: Ticket Aggregate (Ticket + wszystkie powiązane encje i value objects)
- ✅ Faza 2.2: User Aggregate (User, SupportSpecialist, Administrator, Worker)
- ✅ Faza 2.3: Team Aggregate (Team)
- ✅ Faza 3.1: Policy Base (Policy klasa abstrakcyjna)
- ✅ Faza 3.2: Business Policies (ResolutionPolicy, EscalationPolicy, WorkerEscalationPolicy, SpecialistResolutionPolicy, TicketStatusPolicy)
- ✅ Faza 4.1: Repositories (IRepository, FileBasedRepository, TicketRepository, UserRepository, TeamRepository, AttachmentRepository)
- ✅ Faza 4.2: Middleware (ExceptionHandlingMiddleware)
- ✅ Faza 5.1: DTOs (wszystkie Request i Response DTOs)
- ✅ Faza 5.2: Validators (CreateTicketRequestValidator, MarkAsReadyForVerificationRequestValidator, ReviewResolutionRequestValidator, EscalateTicketRequestValidator, AddCommentRequestValidator)
- ✅ Faza 5.3: Mappers (TicketMapper, UserMapper, TeamMapper, CommentMapper)
- ✅ Faza 5.4: Services (TicketService, UserService, TeamService)
- ✅ Faza 6.1: Controllers (TicketsController, UsersController, TeamsController)
- ✅ Faza 7.1: Dependency Injection (Program.cs - wszystkie komponenty zarejestrowane)
- ✅ Faza 7.2: Folder Structure (struktura folderów zgodna z planem, Data/ i Data/uploads/ utworzone)
- ✅ Faza 7.3: Data Files (pliki JSON tworzone automatycznie przez FileBasedRepository)
- ✅ Dodano FluentValidation do walidacji w klasach domenowych
- ✅ Wszystkie klasy używają wyjątków domenowych z komunikatami w stylu "XXX_DATA_VALIDATION_ERROR"
- ✅ Repozytoria automatycznie tworzą foldery i pliki JSON przy starcie aplikacji

**Następny krok:** Faza 8 - Testowanie i Weryfikacja (opcjonalne)
