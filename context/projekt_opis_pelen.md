# System Zarządzania Zgłoszeniami (Ticketing System) — Opis Projektu

## CZĘŚĆ I: OPIS WYSOKOPOZIOMOWY (Dla Eksperta Dziedzinowego)

### 📋 Czym Jest System?

System zarządzania zgłoszeniami to aplikacja wspierająca obsługę zgłoszeń (ticketów) zgłaszanych przez pracowników (workerów) do działów supportu. System automatyzuje przepływ zgłoszeń od momentu utworzenia do rozwiązania, zapewniając transparentność, efektywność i jakość obsługi.

### 🎯 Główne Aktorzy Systemu

1. **Worker (Pracownik)**
   - Tworzy zgłoszenia
   - Dodaje komentarze do swoich zgłoszeń
   - Widzi status i postępy
   - Ocenia satysfakcję po rozwiązeniu
   - Może **zgłosić, że rozwiązanie jest nieprawidłowe**
   - Może **eskalować zgłoszenie** jeśli nie jest zadowolony

2. **Support Specialist (Specjalista Supportu)**
   - Przypisuje sobie zgłoszenia
   - Pracuje nad zgłoszeniami
   - Dodaje komentarze wewnętrzne i publiczne
   - Rozwiązuje problemy
   - Wskazuje typ rozwiązania
   - **Nie może eskalować** — tylko Worker i Admin

3. **Administrator**
   - Zarządza zespołami
   - Zarządza użytkownikami
   - **Przejmuje eskalowane zgłoszenia**
   - Monitoruje polityki i SLA
   - Przejmuje trudne zgłoszenia
   - Może **eskalować do wyższego kierownictwa**

### 📊 Przepływ Zgłoszenia (Cykl Życia)

```
┌─────────────────────────────────────────────────────────────┐
│                    NOWE (NOWE)                              │
│         Worker tworzy zgłoszenie                            │
│  Tytuł, opis, kategoria, priorytet                          │
└─────────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────────┐
│                  PRZYPISANE (PRZYPISANE)                    │
│   Zgłoszenie przypisane do Specialisty lub Zespołu         │
│      Specialist może zmienić status na W_TOKU              │
└─────────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────────┐
│                   W TOKU (W_TOKU)                           │
│         Specialist pracuje nad zgłoszeniem                  │
│  Może dodawać komentarze, załączniki, zmienić priorytet    │
│  Worker może obserwować postępy                            │
└─────────────────────────────────────────────────────────────┘
                          ↓
            ┌─────────────┴──────────────┐
            ↓                            ↓
 ┌──────────────────────┐   ┌──────────────────────┐
 │ OCZEKUJE NA          │   │ GOTOWE DO WERYFIKACJI│
 │ ODPOWIEDŹ            │   │ (Specialist zaznaczył│
 │ (worker musi         │   │  jako rozwiązane)    │
 │  coś dodać)          │   └──────────────┬───────┘
 └────────┬─────────────┘                  ↓
          ↓                    (Worker przegląda)
 (worker odpisuje)                    ↓
          ↓                    ┌──────┴──────┐
          └────────┬───────────┘             ↓
                   ↓            ┌──────────────────────┐
         ┌─────────────────┐   │ Worker nie zadowolony│
         │ ROZWIĄZANE      │   │ Może ESKALOWAĆ ⬆️   │
         │ (zaakceptowane) │   └────────┬─────────────┘
         │       ↓         │            ↓
         │   ZAMKNIĘTE     │     ESKALOWANE
         │  (satysfakcja)  │    (do Admina)
         └─────────────────┘
```

### 🏢 Zespoły i Specjalizacje

- Każdy **Specialist** należy do **Zespołu**
- Każdy **Zespół** ma **specjalizację** (np. IT, HR, Finance, General)
- Zespół ma **limit max zgłoszeń** które może przyjąć
- Każdy Specialist ma **limit aktywnych zgłoszeń**

### ⏱️ SLA (Service Level Agreement)

Każde zgłoszenie ma **SLA** zależy od **priorytetu**:
- **NISKI**: Reaktion 48h, Rozwiązanie 7 dni
- **ŚREDNI**: Reaktion 24h, Rozwiązanie 3 dni
- **WYSOKI**: Reaktion 4h, Rozwiązanie 1 dzień
- **KRYTYCZNY**: Reaktion 1h, Rozwiązanie 4 godziny

System może **automatycznie eskalować** zgłoszenie jeśli SLA się minie.

### 💬 Komentarze i Komunikacja

- Każde zgłoszenie może mieć **wiele komentarzy**
- Komentarze mogą być:
  - **Publiczne** (widoczne dla workera)
  - **Wewnętrzne** (tylko dla supportu)
- Historia zmian jest **zawsze rejestrowana**

### 📎 Załączniki

- Workerowie mogą **dodawać załączniki** przy tworzeniu
- Specialiści mogą **dodawać załączniki** podczas pracy
- System przechowuje **ścieżkę dostępu** do pliku

### ⭐ Satysfakcja i Weryfikacja

Po oznaczeniu jako rozwiązane:
- Worker może **zaakceptować** rozwiązanie ✓ → ZAMKNIĘTE
- Worker może **eskalować** problem ⬆️ → ESKALOWANE
- Worker może **dać rating** (1-5 gwiazdek) gdy zaakceptuje
- Worker mówi czy **problem jest rzeczywiście rozwiązany**

**Przy eskalacji:** Zgłoszenie przechodzi do statusu **ESKALOWANE** i trafia do Administratora.

### 📈 Priorytet i Eskalacja

**Priorytet**:
- Automatycznie przydzielony przy tworzeniu (Worker lub Admin)
- Może być **zmieniony** przez Specialisty
- Wpływa na **SLA**

**Eskalacja — Kiedy?**

Zgłoszenie trafia do **ESKALOWANE** w następujących przypadkach:

1. **🚨 Worker eskaluje po weryfikacji**
   - Specialist oznaczył jako GOTOWE DO WERYFIKACJI
   - Worker przegląda i mówi: "To nie jest rozwiązane" lub "Nie spełnia mojego wymagania"
   - Worker kliknie **"Eskaluj"** ⬆️
   - Status zmienia się na ESKALOWANE
   - Admin przejmuje zgłoszenie

2. **⏰ Deadline SLA się mija**
   - Ticket jest od 4h+ w statusie W_TOKU (priorytet KRYTYCZNY)
   - System automatycznie eskaluje do admina
   - Admin może przypisać bardziej doświadczonego specialisty

3. **❌ Specialist podaje wątpliwe rozwiązanie (Policy)**
   - Specialist zaznacza: "NIE_MOŻNA_ODTWORZYĆ"
   - Ale problem jest łatwo odtwarzalny (Policy sprawdza)
   - System **nie pozwala** na to rozwiązanie
   - Specialist musi wybrać inne rozwiązanie LUB Worker eskaluje

4. **⬆️ Admin eskaluje do wyższego kierownictwa**
   - Po kilku nieudanych próbach
   - Admin może ręcznie oznaczyć jako wymaga wyższej eskalacji
   - Zgłoszenie idzie do VIP management team

5. **🔁 Nieudana próba kilkakrotnie**
   - Jeśli to samo zgłoszenie było GOTOWE DO WERYFIKACJI i ESKALOWANE 3+ razy
   - System proponuje strategie innych podejścia
   - Admin otrzymuje alert

### 🎓 Workflow Eskalacji

```
┌─────────────────────────────────────────────────────────────┐
│  WORKER INITIATES ESCALATION                                │
│  Status: GOTOWE DO WERYFIKACJI → ESKALOWANE               │
│  Worker kliknie "Eskaluj" przycisk                         │
│  Musi podać powód:                                          │
│  • "Problem nadal istnieje"                                │
│  • "Rozwiązanie nie spełnia wymagań"                       │
│  • "Muszę szybciej rozwiązanie"                            │
│                      ↓                                       │
│  ADMIN PRZEJMUJE                                            │
│  • Przegląda historię i uwagi workera                      │
│  • Analizuje co Specialist zrobił                          │
│  • Decyduje o dalszych krokach:                            │
│                                                             │
│  A) Przypisz innemu Specialiście                           │
│     (bardziej doświadczonemu w tej kategorii)              │
│                                                             │
│  B) Zmień priorytet                                        │
│     (na wyższy jeśli jest ważne)                           │
│                                                             │
│  C) Rozwiąż sam (jeśli Admin potrafi)                      │
│                                                             │
│  D) Eskaluj do Management                                  │
│     (jeśli wielokrotnie się nie udało)                     │
│                                                             │
│              Status → W_TOKU (znowu)                       │
│                      ↓                                       │
│    Nowy specialist (lub sam Admin) pracuje                 │
│                                                             │
│    Jeśli znowu fail → Eskalacja do Management             │
└─────────────────────────────────────────────────────────────┘
```

---

## CZĘŚĆ II: OPIS TECHNICZNY (Dla Developerów)

### 🏗️ Architektura: Domain-Driven Design (DDD)

System zbudowany jest na **Domain-Driven Design**, co oznacza że logika biznesowa jest **oddzielona** od logiki technicznej.

#### 4 Warstwy Architektury:

```
┌─────────────────────────────────────┐
│   PRESENTATION (Prezentacja)        │ ← REST API Controllers
├─────────────────────────────────────┤
│   APPLICATION (Aplikacja)           │ ← Services, DTOs, Mappers
├─────────────────────────────────────┤
│   DOMAIN (Domena)                   │ ← Business Logic, Agregaty, Policies
├─────────────────────────────────────┤
│   INFRASTRUCTURE (Infrastruktura)   │ ← Repositories, File System, Middleware
└─────────────────────────────────────┘
```

### 🎯 Domain Layer — Serce Aplikacji

#### Agregaty (Aggregate Roots)

**Agregat** to grupa obiektów które zawsze są spójne razem.

**3 Agregaty w systemie:**

1. **Ticket Aggregate**
   ```csharp
   public class Ticket : AggregateRoot<string> {
       public TicketNumber Number { get; private set; }
       public string Title { get; private set; }
       public string Description { get; private set; }
       public TicketStatus Status { get; private set; }
       public Priority Priority { get; private set; }
       
       // Child entities (wewnątrz agregatu)
       private List<Comment> _comments;
       private List<Escalation> _escalations;
       private List<Attachment> _attachments;
       private List<HistoryChange> _history;
       
       // Value Objects
       private Resolution _resolution;
       private Satisfaction _satisfaction;
       private SLA _sla;
   }
   ```

2. **User Aggregate**
   ```csharp
   public abstract class User : AggregateRoot<string> {
       public string Email { get; private set; }
       public string FirstName { get; private set; }
       public string LastName { get; private set; }
       public AccountStatus AccountStatus { get; private set; }
   }
   
   // Subtypy:
   public class SupportSpecialist : User { /* ... */ }
   public class Administrator : User { /* ... */ }
   public class Worker : User { /* ... */ }
   ```

3. **Team Aggregate**
   ```csharp
   public class Team : AggregateRoot<string> {
       public string Name { get; private set; }
       public TicketCategory Specialization { get; private set; }
       private List<string> _specialistIds;
   }
   ```

#### Value Objects

Value Objects reprezentują **wartości** które są niezmienne i porównywane po wartości (nie po ID).

```csharp
// Przykład: TicketNumber
public class TicketNumber : ValueObject {
    public string Value { get; private set; }
    
    public override bool Equals(ValueObject other) {
        if (other is not TicketNumber otherNumber) return false;
        return Value == otherNumber.Value;
    }
}

// Przykład: Priority
public class Priority : ValueObject {
    public PriorityLevel Level { get; private set; }
}
```

#### Policies — Pure Business Logic

Policies to **synchroniczne funkcje** które zawierają logikę biznesową bez pobierania danych z bazy.

```csharp
public class ResolutionPolicy {
    /// <summary>
    /// Sprawdza czy resolution jest wiarygodne
    /// Zapobiega "NIE_MOŻNA_ODTWORZYĆ" dla łatwo odtwarzalnych problemów
    /// </summary>
    public Result<bool> CanAcceptResolution(
        Ticket ticket,
        Resolution resolution,
        SupportSpecialist specialist) {
        
        if (resolution.Type == ResolutionType.NIE_MOZNA_ODTWORZYC) {
            // Sprawdzić czy problem był wcześniej odtwarzalny
            if (ticket.WasReproducedBefore()) {
                return Result<bool>.CreateFailure(
                    "Cannot mark as NOT_REPRODUCIBLE - problem was reproducible before"
                );
            }
        }
        
        return Result<bool>.CreateSuccess(true);
    }
}

public class EscalationPolicy {
    /// <summary>
    /// Sprawdza czy zgłoszenie powinno być automatycznie eskalowane
    /// (np. jeśli SLA się mija lub wielokrotnie się nie udało)
    /// </summary>
    public Result<bool> ShouldAutoEscalateTicket(
        Ticket ticket,
        TicketStatus currentStatus,
        DateTime slaDeadline) {
        
        // Auto-eskalacja jeśli deadline się mija
        if (DateTime.UtcNow > slaDeadline && currentStatus == TicketStatus.W_TOKU) {
            return Result<bool>.CreateSuccess(true);
        }
        
        // Auto-eskalacja jeśli było wiele nieudanych prób (3+)
        if (ticket.GetEscalationCount() >= 3) {
            return Result<bool>.CreateSuccess(true);
        }
        
        return Result<bool>.CreateSuccess(false);
    }
}

public class WorkerEscalationPolicy {
    /// <summary>
    /// Sprawdza czy Worker może eskalować zgłoszenie
    /// Worker może eskalować TYLKO ze statusu GOTOWE DO WERYFIKACJI
    /// </summary>
    public Result<bool> CanWorkerEscalate(
        Ticket ticket,
        Worker worker,
        string escalationReason) {
        
        // Sprawdzenie statusu
        if (ticket.Status != TicketStatus.GOTOWE_DO_WERYFIKACJI) {
            return Result<bool>.CreateFailure(
                $"Can only escalate from GOTOWE_DO_WERYFIKACJI status, current status is {ticket.Status}"
            );
        }
        
        // Sprawdzenie czy to właściwy Worker (creator)
        if (ticket.CreatedById != worker.Id) {
            return Result<bool>.CreateFailure(
                "Only ticket creator can escalate"
            );
        }
        
        // Sprawdzenie powodu eskalacji
        if (string.IsNullOrWhiteSpace(escalationReason)) {
            return Result<bool>.CreateFailure(
                "Escalation reason is required"
            );
        }
        
        return Result<bool>.CreateSuccess(true);
    }
}

public class SpecialistResolutionPolicy {
    /// <summary>
    /// Sprawdza czy Specialist może oznaczyć ticket jako GOTOWE DO WERYFIKACJI
    /// Zapobiega wychodzeniu z błędnymi rozwiązaniami
    /// </summary>
    public Result<bool> CanMarkAsReadyForVerification(
        Ticket ticket,
        SupportSpecialist specialist,
        Resolution resolution) {
        
        // Musi być przypisany do tego specialisty
        if (ticket.AssignedToId != specialist.Id) {
            return Result<bool>.CreateFailure(
                "Cannot mark ticket as ready - not assigned to you"
            );
        }
        
        // Musi być w statusie W_TOKU
        if (ticket.Status != TicketStatus.W_TOKU) {
            return Result<bool>.CreateFailure(
                $"Can only mark as ready from W_TOKU status, current is {ticket.Status}"
            );
        }
        
        // Call ResolutionPolicy - sprawdzenie czy resolution jest ok
        var resolutionPolicy = new ResolutionPolicy();
        var resolutionResult = resolutionPolicy.CanAcceptResolution(
            ticket,
            resolution,
            specialist
        );
        
        if (!resolutionResult.IsSuccess) {
            return Result<bool>.CreateFailure(resolutionResult.Error);
        }
        
        return Result<bool>.CreateSuccess(true);
    }
}
```

**Korzyści Policies:**
- ✅ Łatwe testowanie (brak dependencies na DB)
- ✅ Szybkie (synchroniczne, bez I/O)
- ✅ Czytelne (czysty kod biznesowy)
- ✅ Reusable (mogą być używane w wielu miejscach)

#### Exceptions

System ma **hierarchię domeny-specific exceptions**:

```csharp
public abstract class DomainException : Exception {
    public int Code { get; }
    public string Details { get; }
    
    protected DomainException(
        int code,
        string message,
        string details) {
        Code = code;
        Details = details;
    }
}

// Subtypy:
public class ForbiddenException : DomainException { /* 403 */ }
public class UnauthorizedException : DomainException { /* 401 */ }
public class NotFoundException : DomainException { /* 404 */ }
public class ValidationException : DomainException { /* 400 */ }
public class ConflictException : DomainException { /* 409 */ }
public class InternalServerException : DomainException { /* 500 */ }
```

### ⚙️ Application Layer — Orchestration

#### Services

Services **orchestrują** operacje - koordynują między Repositories, Policies i Domain Models.

```csharp
public class TicketService {
    private readonly TicketRepository _ticketRepository;
    private readonly UserRepository _userRepository;
    private readonly TeamRepository _teamRepository;
    private readonly ResolutionPolicy _resolutionPolicy;
    private readonly EscalationPolicy _escalationPolicy;
    private readonly WorkerEscalationPolicy _workerEscalationPolicy;
    private readonly SpecialistResolutionPolicy _specialistResolutionPolicy;
    private readonly TicketMapper _ticketMapper;
    
    /// <summary>
    /// Specialist oznacza zgłoszenie jako GOTOWE DO WERYFIKACJI
    /// Worker będzie mógł je przejrzeć i zaakceptować lub eskalować
    /// </summary>
    public async Task MarkAsReadyForVerificationAsync(
        string ticketId,
        string specialistId,
        string resolutionDescription,
        ResolutionType resolutionType) {
        
        // 1. FETCH DATA
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket == null) {
            throw new NotFoundException(404, "Ticket not found", ticketId);
        }
        
        var specialist = await _userRepository.GetByIdAsync(specialistId);
        if (specialist is not SupportSpecialist supportSpecialist) {
            throw new ValidationException(400, "User is not a specialist", specialistId);
        }
        
        // 2. CREATE RESOLUTION OBJECT
        var resolution = Resolution.Create(
            type: resolutionType,
            description: resolutionDescription
        );
        
        if (!resolution.IsSuccess) {
            throw new ValidationException(400, "Invalid resolution", resolution.Error);
        }
        
        // 3. CALL POLICY - sprawdzenie czy można oznaczyć jako ready
        var policyResult = _specialistResolutionPolicy.CanMarkAsReadyForVerification(
            ticket,
            supportSpecialist,
            resolution.Value
        );
        
        if (!policyResult.IsSuccess) {
            throw new ConflictException(409, "Cannot mark as ready", policyResult.Error);
        }
        
        // 4. EXECUTE DOMAIN LOGIC
        ticket.MarkAsReadyForVerification(
            resolutionDescription: resolutionDescription,
            resolutionType: resolutionType
        );
        ticket.RecordChange(
            changeType: "READY_FOR_VERIFICATION",
            previousValue: TicketStatus.W_TOKU.ToString(),
            newValue: TicketStatus.GOTOWE_DO_WERYFIKACJI.ToString(),
            performedBy: specialistId,
            description: $"Marked as ready for worker review: {resolutionType}"
        );
        
        // 5. PERSIST
        await _ticketRepository.SaveAsync(ticket);
    }
    
    /// <summary>
    /// Worker przegląda rozwiązanie i może go zaakceptować lub eskalować
    /// </summary>
    public async Task ReviewResolutionAsync(
        string ticketId,
        string workerId,
        bool accepted,
        string reviewComment) {
        
        // 1. FETCH DATA
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket == null) {
            throw new NotFoundException(404, "Ticket not found", ticketId);
        }
        
        if (ticket.Status != TicketStatus.GOTOWE_DO_WERYFIKACJI) {
            throw new ConflictException(409, "Ticket is not ready for verification", ticketId);
        }
        
        var worker = await _userRepository.GetByIdAsync(workerId);
        if (worker is not Worker workerUser) {
            throw new ValidationException(400, "User is not a worker", workerId);
        }
        
        // 2. REVIEW LOGIC
        if (accepted) {
            // Worker zaakceptował
            ticket.ChangeStatus(TicketStatus.ZAMKNIETE);
            ticket.RecordSatisfaction(
                rating: 0, // Worker może dać rating dopiero przy zamknięciu
                comment: reviewComment
            );
            ticket.RecordChange(
                changeType: "RESOLUTION_ACCEPTED",
                previousValue: TicketStatus.GOTOWE_DO_WERYFIKACJI.ToString(),
                newValue: TicketStatus.ZAMKNIETE.ToString(),
                performedBy: workerId,
                description: $"Worker accepted resolution: {reviewComment}"
            );
        } else {
            // Worker ESKALUJE
            await EscalateTicketAsync(
                ticketId: ticketId,
                escalatedBy: workerId,
                escalationReason: reviewComment
            );
        }
        
        // 3. PERSIST
        await _ticketRepository.SaveAsync(ticket);
    }
    
    /// <summary>
    /// Worker eskaluje zgłoszenie (może to zrobić TYLKO ze statusu GOTOWE DO WERYFIKACJI)
    /// </summary>
    public async Task EscalateTicketAsync(
        string ticketId,
        string escalatedBy,
        string escalationReason) {
        
        // 1. FETCH DATA
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket == null) {
            throw new NotFoundException(404, "Ticket not found", ticketId);
        }
        
        var worker = await _userRepository.GetByIdAsync(escalatedBy);
        if (worker is not Worker workerUser) {
            throw new ValidationException(400, "User is not a worker", escalatedBy);
        }
        
        // 2. CALL POLICY - sprawdzenie czy Worker może eskalować
        var policyResult = _workerEscalationPolicy.CanWorkerEscalate(
            ticket,
            workerUser,
            escalationReason
        );
        
        if (!policyResult.IsSuccess) {
            throw new ForbiddenException(403, "Cannot escalate ticket", policyResult.Error);
        }
        
        // 3. EXECUTE DOMAIN LOGIC
        ticket.ChangeStatus(TicketStatus.ESKALOWANE);
        
        var escalation = Escalation.Create(
            reason: escalationReason,
            escalatedBy: escalatedBy,
            escalationType: EscalationType.WORKER_INITIATED,
            previousPriority: ticket.Priority.Level
        );
        
        ticket.AddEscalation(escalation.Value);
        ticket.RecordChange(
            changeType: "ESCALATED",
            previousValue: TicketStatus.GOTOWE_DO_WERYFIKACJI.ToString(),
            newValue: TicketStatus.ESKALOWANE.ToString(),
            performedBy: escalatedBy,
            description: $"Worker escalated: {escalationReason}"
        );
        
        // 4. PERSIST
        await _ticketRepository.SaveAsync(ticket);
    }
}
```

#### DTOs (Data Transfer Objects)

DTOs to **kontrakty API** - nie pokazują wewnętrznych szczegółów domeny.

```csharp
// Request DTOs
public class CreateTicketRequest {
    public string Title { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public string Priority { get; set; }
}

public class MarkAsReadyForVerificationRequest {
    public string ResolutionDescription { get; set; }
    public string ResolutionType { get; set; } // ROZWIAZANE, OBEJSCIE_PROBLEMU, NIE_MOZNA_ODTWORZYC
}

public class ReviewResolutionRequest {
    public bool Accepted { get; set; }
    public string ReviewComment { get; set; }
}

public class EscalateTicketRequest {
    public string EscalationReason { get; set; }
}

// Response DTOs
public class TicketDTO {
    public string Id { get; set; }
    public string Number { get; set; }
    public string Title { get; set; }
    public string Status { get; set; }
    public string Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class TicketDetailDTO : TicketDTO {
    public string Description { get; set; }
    public List<CommentDTO> Comments { get; set; }
    public List<AttachmentDTO> Attachments { get; set; }
    public List<HistoryChangeDTO> History { get; set; }
    public List<EscalationDTO> Escalations { get; set; }
    public ResolutionDTO Resolution { get; set; }
}

public class EscalationDTO {
    public string Id { get; set; }
    public string Reason { get; set; }
    public string EscalatedBy { get; set; }
    public string EscalationType { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

#### Mappers

Mappers transformują **Domain Models ↔ DTOs**.

```csharp
public class TicketMapper {
    public TicketDTO Map(Ticket ticket) {
        return new TicketDTO {
            Id = ticket.Id,
            Number = ticket.Number.Value,
            Title = ticket.Title,
            Status = ticket.Status.ToString(),
            Priority = ticket.Priority.Level.ToString(),
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt
        };
    }
}
```

### 🔧 Infrastructure Layer

#### Repositories

Repositories abstrakcyjnie obsługują **persystencję** danych.

```csharp
public interface IRepository<T, TId> where T : AggregateRoot<TId> {
    Task<T> GetByIdAsync(TId id);
    Task SaveAsync(T aggregate);
    Task<List<T>> GetAllAsync();
    Task DeleteAsync(TId id);
}
```

### 🎨 Presentation Layer

#### Controllers

Controllers to **thin layer** - parsują request i mapują response.

```csharp
[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase {
    private readonly TicketService _ticketService;
    private readonly TicketMapper _ticketMapper;
    
    [HttpPost("{id}/mark-ready-for-verification")]
    public async Task<IActionResult> MarkAsReadyForVerification(
        string id,
        [FromBody] MarkAsReadyForVerificationRequest request) {
        
        var userId = GetCurrentUserId(); // z context'u
        
        await _ticketService.MarkAsReadyForVerificationAsync(
            ticketId: id,
            specialistId: userId,
            resolutionDescription: request.ResolutionDescription,
            resolutionType: Enum.Parse<ResolutionType>(request.ResolutionType)
        );
        
        var ticket = await _ticketService.GetTicketByIdAsync(id);
        var dto = _ticketMapper.Map(ticket);
        return Ok(dto);
    }
    
    [HttpPost("{id}/review-resolution")]
    public async Task<IActionResult> ReviewResolution(
        string id,
        [FromBody] ReviewResolutionRequest request) {
        
        var userId = GetCurrentUserId();
        
        await _ticketService.ReviewResolutionAsync(
            ticketId: id,
            workerId: userId,
            accepted: request.Accepted,
            reviewComment: request.ReviewComment
        );
        
        var ticket = await _ticketService.GetTicketByIdAsync(id);
        var dto = _ticketMapper.Map(ticket);
        return Ok(dto);
    }
    
    [HttpPost("{id}/escalate")]
    public async Task<IActionResult> EscalateTicket(
        string id,
        [FromBody] EscalateTicketRequest request) {
        
        var userId = GetCurrentUserId();
        
        await _ticketService.EscalateTicketAsync(
            ticketId: id,
            escalatedBy: userId,
            escalationReason: request.EscalationReason
        );
        
        var ticket = await _ticketService.GetTicketByIdAsync(id);
        var dto = _ticketMapper.Map(ticket);
        return Ok(dto);
    }
}
```

---

## CZĘŚĆ III: PRAKTYCZNE IMPLEMENTACYJNE DETALE

### 🔒 Konstruktory Domain Objects — Private!

**Reguła:** Nigdy nie tworzymy obiektów domenowych przez konstruktor publiczny. Zawsze używamy **statycznej funkcji `Create()`**.

```csharp
// ❌ ŹLE:
var ticket = new Ticket {
    Id = "T-001",
    Title = "",
    Priority = null
};

// ✅ DOBRZE:
var ticketResult = Ticket.Create(
    id: "T-001",
    title: "Computer not starting",
    description: "My laptop won't turn on",
    category: TicketCategory.IT,
    priority: PriorityLevel.WYSOKI,
    createdById: "user-123"
);

if (!ticketResult.IsSuccess) {
    throw new ValidationException(400, "Invalid ticket data", ticketResult.Error);
}

var ticket = ticketResult.Value;
```

### ✅ Fluent Validation — Walidacja Input'ów

Używamy **FluentValidation** do walidowania request'ów (DTOs).

```csharp
public class CreateTicketRequestValidator : AbstractValidator<CreateTicketRequest> {
    public CreateTicketRequestValidator() {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MinimumLength(5).WithMessage("Title must be at least 5 characters")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");
        
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MinimumLength(10).WithMessage("Description must be at least 10 characters")
            .MaximumLength(5000).WithMessage("Description must not exceed 5000 characters");
    }
}

public class MarkAsReadyForVerificationRequestValidator : AbstractValidator<MarkAsReadyForVerificationRequest> {
    public MarkAsReadyForVerificationRequestValidator() {
        RuleFor(x => x.ResolutionDescription)
            .NotEmpty().WithMessage("Resolution description is required")
            .MinimumLength(10).WithMessage("Description must be at least 10 characters");
        
        RuleFor(x => x.ResolutionType)
            .NotEmpty().WithMessage("Resolution type is required")
            .IsEnumName(typeof(ResolutionType)).WithMessage("Invalid resolution type");
    }
}

public class ReviewResolutionRequestValidator : AbstractValidator<ReviewResolutionRequest> {
    public ReviewResolutionRequestValidator() {
        RuleFor(x => x.ReviewComment)
            .NotEmpty().WithMessage("Review comment is required")
            .MinimumLength(5).WithMessage("Comment must be at least 5 characters")
            .MaximumLength(5000).WithMessage("Comment must not exceed 5000 characters");
    }
}

public class EscalateTicketRequestValidator : AbstractValidator<EscalateTicketRequest> {
    public EscalateTicketRequestValidator() {
        RuleFor(x => x.EscalationReason)
            .NotEmpty().WithMessage("Escalation reason is required")
            .MinimumLength(10).WithMessage("Reason must be at least 10 characters")
            .MaximumLength(5000).WithMessage("Reason must not exceed 5000 characters");
    }
}
```

---

## 📋 Przepływ Eskalacji: Szczegółowy Przykład

```
┌─────────────────────────────────────────────────────────────┐
│  1. SPECIALIST MARKS AS READY FOR VERIFICATION              │
│     Status: W_TOKU → GOTOWE_DO_WERYFIKACJI                 │
│     ResolutionType: "ROZWIĄZANE"                            │
│     Description: "Bug fixed in version 2.3.1"              │
└─────────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────────┐
│  2. WORKER REVIEWS RESOLUTION                               │
│     Worker przegląda opisaną zmianę                        │
│     POST /api/tickets/{id}/review-resolution                │
│     Może:                                                    │
│     A) Zaakceptować: accepted=true → ZAMKNIĘTE             │
│     B) Eskalować: accepted=false                           │
└─────────────────────────────────────────────────────────────┘
                          ↓
             ┌────────────┴────────────┐
             ↓                         ↓
  ┌──────────────────────┐  ┌──────────────────────┐
  │ ACCEPTED             │  │ ESCALATED            │
  │ Status: ZAMKNIĘTE    │  │ Status: ESKALOWANE   │
  │ • Close ticket       │  │ • POST /api/tickets/ │
  │ • Ask for rating     │  │   {id}/escalate      │
  │ • Store satisfaction │  │ {                    │
  │                      │  │  "escalationReason": │
  │                      │  │  "Problem persists"  │
  │                      │  │ }                    │
  └──────────────────────┘  └──────┬───────────────┘
                                    ↓
                    ┌───────────────────────────────┐
                    │ ADMIN RECEIVES ESCALATION     │
                    │ • Reviews Worker's comment    │
                    │ • Sees Specialist's effort    │
                    │ • Decides on action:          │
                    │   - Assign to Senior Spec     │
                    │   - Increase priority         │
                    │   - Resolve himself           │
                    │   - Escalate to Management    │
                    └───────────────────────────────┘
```

---

## 🎯 Podsumowanie Kluczowych Zasad

| Zasada | Opis | Przykład |
|--------|------|---------|
| **Private Constructors** | Konstruktory domeny są prywatne | `private Ticket() { }` |
| **Factory Methods** | Create() - jedyna forma tworzenia | `Ticket.Create(...)` |
| **Fluent Validation** | Walidacja Request DTOs | `CreateTicketRequestValidator` |
| **Domain Validation** | Wewnętrzna walidacja w Create() | `if (string.IsNullOrWhiteSpace(title))` |
| **Services Orchestrate** | Services koordynują operacje | Fetch → Policy → Domain → Persist |
| **Policies Synchronous** | Policies nie mają dependencies na DB | `Result<bool> CanWorkerEscalate(...)` |
| **Mappers Transform** | Mappers konwertują Model → DTO | `TicketMapper.Map(ticket)` |
| **Repositories Abstract** | Repositories ukrywają persystencję | `IRepository<T, TId>` |
| **Worker Escalates** | Worker (nie Specialist) inicjuje eskalację | `EscalateTicketAsync(workerId, reason)` |
| **Specialist Marks Ready** | Specialist oznacza jako GOTOWE DO WERYFIKACJI | `MarkAsReadyForVerificationAsync()` |
| **Policy Validation** | Policies blokują złe rozwiązania | `WorkerEscalationPolicy`, `ResolutionPolicy` |

---

## 🚀 Dalsze Kroki

Kod jest gotowy do implementacji w **Cursor IDE** z użyciem:
- Domain-Driven Design
- FluentValidation
- File-based persistence
- Synchroniczne Policies
- Mappers dla DTOs
- Private constructors + Factory methods
- **Worker-Initiated Escalation Workflow** ✨
- **MarkAsReadyForVerification** status

**Gotów na kod? Które klasy generować najpierw?** 🎯

```
Enums: TicketStatus, Priority, ResolutionType
Domain Models: Ticket, User, Team, Escalation
Policies: ResolutionPolicy, WorkerEscalationPolicy, SpecialistResolutionPolicy
Services: TicketService
DTOs & Validators: All request/response types
Controllers: TicketsController
```
