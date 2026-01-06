# Plan Implementacji - System Zarządzania Zgłoszeniami

## 📋 Przegląd Projektu

System zarządzania zgłoszeniami oparty na **Domain-Driven Design (DDD)** z architekturą 4-warstwową:
- **Domain Layer** - Logika biznesowa, agregaty, value objects, policies
- **Application Layer** - Services, DTOs, Mappers, Validators
- **Infrastructure Layer** - Repositories (file-based), Middleware
- **Presentation Layer** - Controllers (REST API)

## 🎯 Faza 1: Fundamenty Domain Layer

### 1.1 Base Classes (Domain.Base)
- [ ] `Entity<TId>` - klasa bazowa dla wszystkich encji
  - Id, Create<T>(), ToPrimitive(), FromPrimitive<T>(), Equals(), GetHashCode()
- [ ] `ValueObject` - klasa bazowa dla value objects
  - Equals(ValueObject), Equals(object), GetHashCode()
- [ ] `AggregateRoot<TId>` - klasa bazowa dla agregatów
  - _uncommittedChanges, GetUncommittedChanges(), ClearUncommittedChanges(), RaiseEvent()

### 1.2 Enums
- [ ] `TicketStatus` - NOWE, PRZYPISANE, W_TOKU, OCZEKUJE_NA_ODPOWIEDZ, GOTOWE_DO_WERYFIKACJI, ESKALOWANE, ZAMKNIETE
- [ ] `PriorityLevel` - NISKI, SREDNI, WYSOKI, KRYTYCZNY
- [ ] `ResolutionType` - ROZWIAZANE, OBEJSCIE_PROBLEMU, NIE_MOZNA_ODTWORZYC
- [ ] `TicketCategory` - IT, HR, FINANCE, GENERAL, OTHER
- [ ] `EscalationType` - WORKER_INITIATED, SLA_TIMEOUT, AUTO_ESCALATION, ADMIN_INITIATED
- [ ] `AccountStatusEnum` - ACTIVE, INACTIVE, SUSPENDED
- [ ] `UserType` - WORKER, SPECIALIST, ADMINISTRATOR

### 1.3 Value Objects
- [ ] `TicketNumber` - ValueObject z Value property
- [ ] `Priority` - ValueObject z Level (PriorityLevel)
- [ ] `AccountStatus` - ValueObject z Status (AccountStatusEnum)
- [ ] `Result<T>` - ValueObject dla wyników operacji (IsSuccess, Value, Error)

### 1.4 Domain Exceptions
- [ ] `DomainException` - klasa bazowa (Code, Message, Details)
- [ ] `ForbiddenException` (403)
- [ ] `UnauthorizedException` (401)
- [ ] `NotFoundException` (404)
- [ ] `ValidationException` (400)
- [ ] `ConflictException` (409)
- [ ] `InternalServerException` (500)

## 🎯 Faza 2: Agregaty Domain Layer

### 2.1 Ticket Aggregate
- [ ] `Ticket` - AggregateRoot<string>
  - Properties: id, number, title, description, status, priority, category, assignedTeamId, assignedSpecialistId, createdById
  - Collections: _resolution, _comments, _escalations, _attachments, _history, _satisfaction, _sla
  - Timestamps: createdAt, updatedAt, resolvedAt
  - Methods: ChangeStatus(), AssignTo(), AssignToTeam(), MarkAsReadyForVerification(), Escalate(), AddComment(), AddAttachment(), AddEscalation(), RecordChange(), IsValid(), GetComments(), GetEscalationCount(), WasReproducedBefore()
  - Factory: `Create()` - statyczna metoda z walidacją

- [ ] `Comment` - Entity (id, authorId, content, isInternal, createdAt)
- [ ] `Resolution` - ValueObject (type, description, tags, createdAt) + Create() factory
- [ ] `Escalation` - Entity (id, reason, previousPriority, newPriority, escalatedAt, escalatedBy, escalationType) + Create() factory
- [ ] `Attachment` - Entity (id, fileName, fileSize, mimeType, uploadedAt, uploadedBy) + GetStoragePath()
- [ ] `HistoryChange` - Entity (id, changedAt, changeType, previousValue, newValue, performedBy, description)
- [ ] `Satisfaction` - Entity (id, rating, comment, filledAt, isProblemResolved)
- [ ] `SLA` - ValueObject (reactionTime, resolutionTime, priority) + GetReactionTime(), GetResolutionTime()

### 2.2 User Aggregate
- [ ] `User` - abstract AggregateRoot<string>
  - Properties: id, email, firstName, lastName, passwordHash, accountStatus
  - Methods: GetEmail(), GetFullName(), IsActive(), GetUserType() (abstract)

- [ ] `SupportSpecialist` - dziedziczy z User
  - Properties: teamId, specialization, activeTicketLimit, currentActiveCount
  - Methods: CanAcceptMoreTickets(), IncrementActiveTickets(), DecrementActiveTickets()

- [ ] `Administrator` - dziedziczy z User
  - Methods: EscalateTicket(), ManagePolicies()

- [ ] `Worker` - dziedziczy z User
  - Methods: CanEscalateTicket()

### 2.3 Team Aggregate
- [ ] `Team` - AggregateRoot<string>
  - Properties: id, name, specialization, maxTickets, specialistIds
  - Methods: AddSpecialist(), RemoveSpecialist(), CanAcceptMore(), GetSpecialistCount()

## 🎯 Faza 3: Policies (Domain.Policies)

### 3.1 Policy Base
- [ ] `Policy` - klasa abstrakcyjna z Success() i Failure() helper methods

### 3.2 Business Policies
- [ ] `ResolutionPolicy`
  - `CanAcceptResolution(ticket, resolution, specialist): Result<bool>`
  - Zapobiega "NIE_MOZNA_ODTWORZYC" dla łatwo odtwarzalnych problemów

- [ ] `EscalationPolicy`
  - `ShouldAutoEscalateTicket(ticket, status, deadline): Result<bool>`
  - Auto-eskalacja przy SLA timeout lub 3+ nieudanych próbach

- [ ] `WorkerEscalationPolicy`
  - `CanWorkerEscalate(ticket, worker, reason): Result<bool>`
  - Worker może eskalować TYLKO ze statusu GOTOWE_DO_WERYFIKACJI
  - Tylko creator ticketu może eskalować
  - Wymaga powodu eskalacji

- [ ] `SpecialistResolutionPolicy`
  - `CanMarkAsReadyForVerification(ticket, specialist, resolution): Result<bool>`
  - Specialist może oznaczyć jako READY tylko jeśli jest przypisany
  - Musi być w statusie W_TOKU
  - Używa ResolutionPolicy wewnętrznie

- [ ] `TicketStatusPolicy`
  - `CanTransitionTo(current, target, performedBy): Result<bool>`
  - Waliduje przejścia między statusami

## 🎯 Faza 4: Infrastructure Layer

### 4.1 Repositories
- [ ] `IRepository<T, TId>` - interfejs
  - GetByIdAsync(), SaveAsync(), GetAllAsync(), DeleteAsync()

- [ ] `FileBasedRepository<T, TId>` - klasa abstrakcyjna
  - _dataFilePath, _logger
  - LoadFromFile(), SaveToFile()
  - Implementacja IRepository

- [ ] `TicketRepository` - dziedziczy z FileBasedRepository
  - _dataFilePath: "Data/tickets.json"
  - GetByNumberAsync(), GetByStatusAsync(), GetByAssignedSpecialistAsync(), GetByTeamAsync(), GetByCategoryAsync()

- [ ] `UserRepository` - dziedziczy z FileBasedRepository
  - _dataFilePath: "Data/users.json"
  - GetByEmailAsync(), GetByUserTypeAsync(), GetSpecialistsByTeamAsync()

- [ ] `TeamRepository` - dziedziczy z FileBasedRepository
  - _dataFilePath: "Data/teams.json"
  - GetBySpecializationAsync()

- [ ] `AttachmentRepository`
  - _uploadDirectory: "Data/uploads"
  - SaveFileAsync(), GetFileAsync(), DeleteFileAsync()

### 4.2 Middleware
- [ ] `ExceptionHandlingMiddleware`
  - Przechwytuje DomainException
  - Zwraca JSON response z odpowiednim HTTP status code
  - Loguje błędy

## 🎯 Faza 5: Application Layer

### 5.1 DTOs (Application.DTOs)
- [ ] Request DTOs:
  - `CreateTicketRequest` (title, description, category, priority)
  - `MarkAsReadyForVerificationRequest` (resolutionDescription, resolutionType)
  - `ReviewResolutionRequest` (accepted, reviewComment)
  - `EscalateTicketRequest` (escalationReason)
  - `AddCommentRequest` (content, isInternal)
  - `UploadAttachmentRequest` (file)

- [ ] Response DTOs:
  - `TicketDTO` (id, number, title, status, priority, category, assignedSpecialistId, createdAt, updatedAt)
  - `TicketDetailDTO` : TicketDTO (description, comments, attachments, history, escalations, resolution)
  - `CommentDTO` (id, authorId, content, isInternal, createdAt)
  - `EscalationDTO` (id, reason, escalatedBy, escalationType, createdAt)
  - `UserDTO` (id, email, firstName, lastName, userType, accountStatus)
  - `SupportSpecialistDTO` : UserDTO (teamId, activeTicketCount, activeTicketLimit)
  - `TeamDTO` (id, name, specialization, specialistCount)

### 5.2 Validators (Application.Validators)
- [ ] `CreateTicketRequestValidator` - FluentValidation
  - Title: NotEmpty, MinLength(5), MaxLength(200)
  - Description: NotEmpty, MinLength(10), MaxLength(5000)
  - Category: NotEmpty, IsEnumName
  - Priority: NotEmpty, IsEnumName

- [ ] `MarkAsReadyForVerificationRequestValidator`
  - ResolutionDescription: NotEmpty, MinLength(10)
  - ResolutionType: NotEmpty, IsEnumName

- [ ] `ReviewResolutionRequestValidator`
  - ReviewComment: NotEmpty, MinLength(5), MaxLength(5000)

- [ ] `EscalateTicketRequestValidator`
  - EscalationReason: NotEmpty, MinLength(10), MaxLength(5000)

- [ ] `AddCommentRequestValidator`
  - Content: NotEmpty, MinLength(1), MaxLength(5000)

### 5.3 Mappers (Application.Mappers)
- [ ] `TicketMapper`
  - Map(ticket): TicketDTO
  - MapDetail(ticket, comments): TicketDetailDTO
  - MapList(tickets): List<TicketDTO>

- [ ] `UserMapper`
  - Map(user): UserDTO
  - MapSpecialist(specialist): SupportSpecialistDTO
  - MapList(users): List<UserDTO>

- [ ] `TeamMapper`
  - Map(team): TeamDTO
  - MapList(teams): List<TeamDTO>

- [ ] `CommentMapper`
  - Map(comment): CommentDTO
  - MapList(comments): List<CommentDTO>

### 5.4 Services (Application.Services)
- [ ] `TicketService`
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

- [ ] `UserService`
  - Dependencies: UserRepository, UserMapper
  - Methods:
    - `RegisterUserAsync(...): Task<User>`
    - `GetUserByIdAsync(...): Task<User>`
    - `GetUserByEmailAsync(...): Task<User>`
    - `AuthenticateAsync(...): Task<User>`

- [ ] `TeamService`
  - Dependencies: TeamRepository, UserRepository, TeamMapper
  - Methods:
    - `CreateTeamAsync(...): Task<Team>`
    - `GetTeamByIdAsync(...): Task<Team>`
    - `AddSpecialistToTeamAsync(...): Task`
    - `RemoveSpecialistFromTeamAsync(...): Task`

## 🎯 Faza 6: Presentation Layer

### 6.1 Controllers (Presentation.Controllers)
- [ ] `TicketsController`
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

- [ ] `UsersController`
  - Dependencies: UserService, UserMapper
  - Endpoints:
    - `POST /api/users/register` - RegisterUser
    - `GET /api/users/{id}` - GetUser
    - `POST /api/users/login` - Login

- [ ] `TeamsController`
  - Dependencies: TeamService, TeamMapper
  - Endpoints:
    - `POST /api/teams` - CreateTeam
    - `GET /api/teams/{id}` - GetTeam
    - `GET /api/teams/{id}/members` - GetTeamMembers
    - `POST /api/teams/{id}/specialists` - AddSpecialist

## 🎯 Faza 7: Konfiguracja i Integracja

### 7.1 Dependency Injection (Program.cs)
- [ ] Zarejestruj wszystkie Repositories
- [ ] Zarejestruj wszystkie Services
- [ ] Zarejestruj wszystkie Mappers
- [ ] Zarejestruj wszystkie Policies
- [ ] Zarejestruj FluentValidation validators
- [ ] Zarejestruj ExceptionHandlingMiddleware

### 7.2 Folder Structure
- [ ] Utwórz strukturę folderów:
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
  Data/ (dla plików JSON)
  Data/uploads/ (dla załączników)
  ```

### 7.3 Data Files
- [ ] Utwórz puste pliki JSON:
  - `Data/tickets.json` - []
  - `Data/users.json` - []
  - `Data/teams.json` - []

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
3. **FluentValidation** - Walidacja Request DTOs
4. **Domain Validation** - Wewnętrzna walidacja w `Create()` methods
5. **Services Orchestrate** - Services koordynują: Fetch → Policy → Domain → Persist
6. **Policies Synchronous** - Policies nie mają dependencies na DB
7. **Mappers Transform** - Mappers konwertują Model → DTO
8. **Repositories Abstract** - Repositories ukrywają persystencję
9. **Proste Komentarze** - Komentarze XML tylko przy klasach i interfejsach. Usuwamy jednozdaniowe, krótkie komentarze z metod i właściwości

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

**Status:** Plan gotowy do implementacji ✅
**Następny krok:** Rozpoczęcie implementacji od Fazy 1
