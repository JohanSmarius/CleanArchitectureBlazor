# Architecture Diagrams

This document contains Mermaid diagrams that describe the structure, data model, and key flows of the Medical First Aid Event & Shift Manager solution.

---

## 1. Solution Architecture

The solution follows Clean Architecture with four layers. Dependencies always point inward: outer layers depend on inner layers, never the other way around.

```mermaid
graph TB
    subgraph Presentation["🖥️ Presentation Layer"]
        BlazorServer["CleanArchitectureBlazor\n(Blazor Server / SSR)"]
        BlazorClient["CleanArchitectureBlazor.Client\n(Blazor WebAssembly)"]
    end

    subgraph AppLayer["📋 Application Layer"]
        UseCases["Use Cases\nCreateEventUseCase\nUpdateEventUseCase"]
        DomainSvc["EventDomainService\n(pure business rules)"]
        AppInterfaces["Interfaces\nIEventRepository\nIShiftRepository\nIStaffRepository\nIStaffAssignmentRepository\nIEmailService\nIEventService\nICreateEventUseCase\nIUpdateEventUseCase"]
        DTOs["Data Adapters\nEventDTO · ShiftDTO\nEventMapper · ShiftMapper"]
    end

    subgraph DomainLayer["🏛️ Domain Layer (Entities)"]
        Entities["Aggregate Entities\nEvent · Shift · Staff · StaffAssignment"]
        Enums["Enumerations\nEventStatus · ShiftStatus\nStaffRole · AssignmentStatus"]
    end

    subgraph InfraLayer["🔧 Infrastructure Layer"]
        Repos["Repositories\nEventRepository\nShiftRepository\nStaffRepository\nStaffAssignmentRepository"]
        EmailImpl["EmailService\n(SMTP via MailKit)"]
        DbCtx["ApplicationDbContext\n(EF Core · SQL Server)"]
    end

    BlazorServer -->|"injects (DI)"| AppInterfaces
    BlazorServer -->|"registers impls"| Repos
    BlazorServer -->|"registers impls"| EmailImpl
    AppLayer -->|"uses"| DomainLayer
    InfraLayer -->|"implements"| AppInterfaces
    InfraLayer -->|"persists"| DomainLayer
    Repos --> DbCtx
    EmailImpl -. "SMTP" .-> ExternalMail[("📧 Mail Server")]
    DbCtx -. "SQL" .-> DB[("🗄️ SQL Server")]
```

---

## 2. Entity Relationship Diagram

The core domain model that drives the application.

```mermaid
erDiagram
    EVENT {
        int Id PK
        string Name
        datetime StartDate
        datetime EndDate
        string Location
        string Description
        EventStatus Status
        string ContactPerson
        string ContactPhone
        string ContactEmail
        bool NotificationSent
        datetime CreatedAt
        datetime UpdatedAt
    }

    SHIFT {
        int Id PK
        int EventId FK
        string Name
        datetime StartTime
        datetime EndTime
        int RequiredStaff
        string Description
        ShiftStatus Status
        datetime CreatedAt
        datetime UpdatedAt
    }

    STAFF {
        int Id PK
        string FirstName
        string LastName
        string Email
        string Phone
        StaffRole Role
        string CertificationLevel
        datetime CertificationExpiry
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }

    STAFF_ASSIGNMENT {
        int Id PK
        int ShiftId FK
        int StaffId FK
        AssignmentStatus Status
        datetime CheckInTime
        datetime CheckOutTime
        string Notes
        datetime AssignedAt
        datetime UpdatedAt
    }

    EVENT ||--o{ SHIFT : "has (cascade delete)"
    SHIFT ||--o{ STAFF_ASSIGNMENT : "has (cascade delete)"
    STAFF ||--o{ STAFF_ASSIGNMENT : "is assigned via"
```

---

## 3. Application Layer Class Diagram

Key classes, interfaces, and their relationships in the Application layer.

> **Note on design inconsistency:** `CreateEventUseCase` correctly uses `EventDTO` as its input/output contract, which keeps domain entities out of the presentation layer. `UpdateEventUseCase` and `EventService` currently use the `Event` domain entity directly — this is a known deviation from the Clean Architecture ideal that should be addressed in a future refactor by introducing an `UpdateEventDTO`.

```mermaid
classDiagram
    class ICreateEventUseCase {
        <<interface>>
        +Execute(EventDTO) Task~EventDTO~
    }
    class IUpdateEventUseCase {
        <<interface>>
        +Execute(Event) Task~Event~
        %% TODO: should accept/return UpdateEventDTO
    }
    class IEventService {
        <<interface>>
        +UpdateEventAsync(Event) Task~Event~
        %% TODO: should accept/return UpdateEventDTO
    }
    class IEventRepository {
        <<interface>>
        +GetAllEventsAsync() Task~List~Event~~
        +GetEventByIdAsync(int) Task~Event~
        +CreateEventAsync(Event) Task~Event~
        +UpdateEventAsync(Event) Task~Event~
        +DeleteEventAsync(int) Task
        +GetUpcomingEventsAsync() Task~List~Event~~
        +GetEventsByDateRangeAsync(DateTime, DateTime) Task~List~Event~~
    }
    class IShiftRepository {
        <<interface>>
        +GetAllShiftsAsync() Task~List~Shift~~
        +GetShiftByIdAsync(int) Task~Shift~
        +GetShiftsByEventIdAsync(int) Task~List~Shift~~
        +CreateShiftAsync(Shift) Task~Shift~
        +UpdateShiftAsync(Shift) Task~Shift~
        +DeleteShiftAsync(int) Task
        +GetUpcomingShiftsAsync() Task~List~Shift~~
        +GetShiftsByDateAsync(DateTime) Task~List~Shift~~
    }
    class IStaffRepository {
        <<interface>>
        +GetAllStaffAsync() Task~List~Staff~~
        +GetStaffByIdAsync(int) Task~Staff~
        +CreateStaffAsync(Staff) Task~Staff~
        +UpdateStaffAsync(Staff) Task~Staff~
        +DeleteStaffAsync(int) Task
        +GetActiveStaffAsync() Task~List~Staff~~
        +GetStaffByRoleAsync(StaffRole) Task~List~Staff~~
        +IsEmailUniqueAsync(string, int?) Task~bool~
    }
    class IStaffAssignmentRepository {
        <<interface>>
        +GetAllAssignmentsAsync() Task~List~StaffAssignment~~
        +GetAssignmentsByShiftIdAsync(int) Task~List~StaffAssignment~~
        +GetAssignmentsByStaffIdAsync(int) Task~List~StaffAssignment~~
        +CreateAssignmentAsync(StaffAssignment) Task~StaffAssignment~
        +UpdateAssignmentAsync(StaffAssignment) Task~StaffAssignment~
        +DeleteAssignmentAsync(int) Task
        +CheckInStaffAsync(int) Task~StaffAssignment~
        +CheckOutStaffAsync(int) Task~StaffAssignment~
        +IsStaffAvailableAsync(int, DateTime, DateTime, int?) Task~bool~
    }
    class IEmailService {
        <<interface>>
        +SendStaffAssignmentNotificationAsync(Staff, Shift, Event) Task
        +SendEventPlannedNotificationAsync(Event) Task
        +SendEventConfirmationNotificationAsync(Event) Task
        +SendEventInvoiceNotificationAsync(Event) Task
        +SendFinancialDepartmentInvoiceNotificationAsync(Event) Task
        +SendEmailAsync(string, string, string) Task
    }
    class CreateEventUseCase {
        -IEventRepository repository
        -ILogger logger
        +Execute(EventDTO) Task~EventDTO~
    }
    class UpdateEventUseCase {
        -IEventRepository _repository
        -IEmailService _emailService
        -ILogger _logger
        -EventDomainService _domainService
        +Execute(Event) Task~Event~
    }
    class EventService {
        -IEventRepository _repository
        -IEmailService _emailService
        -ILogger _logger
        -EventDomainService _domainService
        +UpdateEventAsync(Event) Task~Event~
    }
    class EventDomainService {
        +ApplyChanges(Event, Event) EventStatusChangeDecision
    }
    class EventStatusChangeDecision {
        <<record struct>>
        +bool ShouldSendPlannedNotification
        +bool PromoteToConfirmedAfterPlanned
        +bool ShouldSendInvoiceNotification
    }

    CreateEventUseCase ..|> ICreateEventUseCase
    UpdateEventUseCase ..|> IUpdateEventUseCase
    EventService ..|> IEventService
    CreateEventUseCase --> IEventRepository
    UpdateEventUseCase --> IEventRepository
    UpdateEventUseCase --> IEmailService
    UpdateEventUseCase --> EventDomainService
    EventService --> IEventRepository
    EventService --> IEmailService
    EventService --> EventDomainService
    EventDomainService --> EventStatusChangeDecision
```

---

## 4. EventStatus State Machine

The lifecycle of an `Event` from creation to completion.

```mermaid
stateDiagram-v2
    [*] --> Requested : Event created

    Requested --> Planned : Staff planned\n(triggers planned email\nif ContactEmail set)
    Requested --> Cancelled : Event cancelled

    Planned --> Confirmed : Auto-promoted after\nplanned email sent\nsuccessfully
    Planned --> Cancelled : Event cancelled

    Confirmed --> Active : Event starts
    Confirmed --> Cancelled : Event cancelled

    Active --> Completed : Event ends
    Active --> Cancelled : Event cancelled

    Completed --> SendInvoice : Invoice requested\n(triggers invoice email)

    SendInvoice --> [*] : Done

    Cancelled --> [*]
```

---

## 5. ShiftStatus State Machine

The lifecycle of a `Shift` within an event.

```mermaid
stateDiagram-v2
    [*] --> Open : Shift created

    Open --> Full : Required staff\ncount reached
    Open --> Cancelled : Shift cancelled

    Full --> Open : Staff assignment\nremoved
    Full --> InProgress : Shift start time reached
    Full --> Cancelled : Shift cancelled

    InProgress --> Completed : Shift end time reached

    Completed --> [*]
    Cancelled --> [*]
```

---

## 6. Create Event — Sequence Diagram

```mermaid
sequenceDiagram
    actor User
    participant Page as EventEdit.razor
    participant UC as CreateEventUseCase
    participant Repo as EventRepository
    participant DB as SQL Server

    User->>Page: Fill in event form & submit
    Page->>UC: Execute(EventDTO)
    UC->>UC: Validate dates\n(EndDate > StartDate,\nStartDate in future)
    alt Validation fails
        UC-->>Page: throw ApplicationException
        Page-->>User: Show validation error
    else Validation passes
        UC->>UC: Map DTO → Entity\nSet Status = Requested\nAdd default Shift
        UC->>Repo: CreateEventAsync(entity)
        Repo->>DB: INSERT Events + Shifts
        DB-->>Repo: Saved entity with Id
        Repo-->>UC: Event
        UC->>UC: Map Entity → EventDTO
        UC-->>Page: EventDTO (created)
        Page-->>User: Redirect to event detail
    end
```

---

## 7. Update Event — Sequence Diagram (with notification side-effects)

> **Note:** The current implementation passes the `Event` domain entity from the page directly to `UpdateEventUseCase`. Ideally, an `UpdateEventDTO` should be used here (consistent with `CreateEventUseCase`) to preserve layer separation. This is a known technical debt item.

```mermaid
sequenceDiagram
    actor User
    participant Page as EventEdit.razor
    participant UC as UpdateEventUseCase
    participant DS as EventDomainService
    participant Repo as EventRepository
    participant Email as EmailService
    participant DB as SQL Server
    participant SMTP as Mail Server

    User->>Page: Edit event & submit
    Page->>UC: Execute(updatedEvent)
    UC->>UC: Validate dates
    UC->>Repo: GetEventByIdAsync(id)
    Repo->>DB: SELECT Events WHERE Id=…
    DB-->>Repo: existing Event
    Repo-->>UC: existing Event

    UC->>DS: ApplyChanges(existing, updated)
    DS->>DS: Copy mutable fields\nEvaluate notification rules
    DS-->>UC: EventStatusChangeDecision

    alt ShouldSendPlannedNotification
        UC->>Email: SendEventPlannedNotificationAsync(event)
        Email->>SMTP: Send planned email to ContactEmail
        SMTP-->>Email: OK
        UC->>UC: Promote Status → Confirmed\nSet NotificationSent = true
    end

    alt ShouldSendInvoiceNotification
        UC->>Email: SendEventInvoiceNotificationAsync(event)
        Email->>SMTP: Send invoice email to ContactEmail
        SMTP-->>Email: OK
        UC->>Email: SendFinancialDepartmentInvoiceNotificationAsync(event)
        Email->>SMTP: Send invoice email to finance dept
        SMTP-->>Email: OK
        UC->>UC: Set NotificationSent = true
    end

    UC->>Repo: UpdateEventAsync(existing)
    Repo->>DB: UPDATE Events SET …
    DB-->>Repo: OK
    Repo-->>UC: updated Event
    UC-->>Page: updated Event
    Page-->>User: Show success message
```

---

## 8. Staff Assignment — Sequence Diagram

```mermaid
sequenceDiagram
    actor User
    participant Page as Assignments.razor
    participant AssignRepo as StaffAssignmentRepository
    participant ShiftRepo as ShiftRepository
    participant Email as EmailService
    participant DB as SQL Server

    User->>Page: Select shift & staff member, confirm
    Page->>AssignRepo: IsStaffAvailableAsync(staffId, start, end)
    AssignRepo->>DB: SELECT assignments WHERE overlap
    DB-->>AssignRepo: result
    AssignRepo-->>Page: bool (available)

    alt Staff not available
        Page-->>User: Show conflict warning
    else Staff available
        Page->>AssignRepo: CreateAssignmentAsync(assignment)
        AssignRepo->>DB: INSERT StaffAssignments
        DB-->>AssignRepo: saved assignment

        AssignRepo->>Email: SendStaffAssignmentNotificationAsync(staff, shift, event)
        Email-->>AssignRepo: OK

        AssignRepo-->>Page: StaffAssignment
        Page-->>User: Assignment confirmed

        Note over Page,ShiftRepo: Update shift status if fully staffed
        Page->>ShiftRepo: GetShiftByIdAsync(shiftId)
        ShiftRepo->>DB: SELECT Shifts + StaffAssignments
        DB-->>ShiftRepo: Shift
        alt Assignments >= RequiredStaff
            Page->>ShiftRepo: UpdateShiftAsync(shift with Status=Full)
            ShiftRepo->>DB: UPDATE Shifts
        end
    end
```
