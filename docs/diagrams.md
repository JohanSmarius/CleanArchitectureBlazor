# Solution Architecture Diagrams

This document contains Mermaid diagrams describing the structure and behaviour of the Medical First Aid Event & Shift Manager.

---

## 1. Project Dependency Graph

Shows how the .NET projects in the solution reference each other.

```mermaid
graph TD
    Domain["Domain\n(.NET class library)\nEntities & Enums"]
    DomainService["DomainService\n(.NET class library)\nInterfaces, Services &\nDomain Rules"]
    Infrastructure["Infrastructure\n(.NET class library)\nEF Core Repositories\n& Email Service"]
    BlazorServer["CleanArchitectureBlazor\n(Blazor Web App – Server)\nPages, Components\n& DI Root"]
    BlazorClient["CleanArchitectureBlazor.Client\n(Blazor WASM Client)"]
    DomainServiceTests["DomainService.Tests\n(xUnit test project)"]

    DomainService --> Domain
    Infrastructure --> Domain
    Infrastructure --> DomainService
    BlazorServer --> Domain
    BlazorServer --> DomainService
    BlazorServer --> Infrastructure
    BlazorServer --> BlazorClient
    DomainServiceTests --> DomainService
    DomainServiceTests --> Domain
```

---

## 2. Clean Architecture Layer Diagram

Illustrates the layered architecture and the direction of allowed dependencies (inner layers have no knowledge of outer layers).

```mermaid
graph TB
    subgraph Presentation["Presentation Layer"]
        Pages["Blazor Pages\nEvents · Shifts · Staff\nDashboard · Assignments"]
        Components["Blazor Components\nLayout · Account · Shared"]
    end

    subgraph Application["Application / Domain Service Layer"]
        IRepos["Repository Interfaces\nIEventRepository\nIShiftRepository\nIStaffRepository\nIStaffAssignmentRepository"]
        IServices["Service Interfaces\nIEventService\nIEmailService"]
        EventService["EventService\n(orchestration)"]
        EventDomainSvc["EventDomainService\n(pure business rules)"]
        DomainException["DomainException"]
    end

    subgraph DomainLayer["Domain Layer"]
        Entities["Entities\nEvent · Shift\nStaff · StaffAssignment"]
        Enums["Enums\nEventStatus · ShiftStatus\nStaffRole · AssignmentStatus"]
    end

    subgraph InfrastructureLayer["Infrastructure Layer"]
        Repos["EF Core Repositories\nEventRepository\nShiftRepository\nStaffRepository\nStaffAssignmentRepository"]
        EmailSvc["EmailService\n(SMTP)"]
        DbCtx["ApplicationDbContext\n(SQL Server)"]
    end

    Pages --> IRepos
    Pages --> IServices
    EventService --> IRepos
    EventService --> IServices
    EventService --> EventDomainSvc
    EventDomainSvc --> Entities
    Repos --> DbCtx
    Repos -.->|implements| IRepos
    EmailSvc -.->|implements| IServices
```

---

## 3. Domain Class Diagram

Entity relationships and their properties.

```mermaid
classDiagram
    class Event {
        +int Id
        +string Name
        +DateTime StartDate
        +DateTime EndDate
        +string Location
        +string? Description
        +EventStatus Status
        +string? ContactPerson
        +string? ContactPhone
        +string? ContactEmail
        +bool NotificationSent
        +DateTime CreatedAt
        +DateTime? UpdatedAt
        +List~Shift~ Shifts
    }

    class Shift {
        +int Id
        +int EventId
        +string Name
        +DateTime StartTime
        +DateTime EndTime
        +int RequiredStaff
        +string? Description
        +ShiftStatus Status
        +DateTime CreatedAt
        +DateTime? UpdatedAt
        +Event Event
        +List~StaffAssignment~ StaffAssignments
    }

    class Staff {
        +int Id
        +string FirstName
        +string LastName
        +string Email
        +string? Phone
        +StaffRole Role
        +string? CertificationLevel
        +DateTime? CertificationExpiry
        +bool IsActive
        +DateTime CreatedAt
        +DateTime? UpdatedAt
        +string FullName
        +List~StaffAssignment~ StaffAssignments
    }

    class StaffAssignment {
        +int Id
        +int ShiftId
        +int StaffId
        +AssignmentStatus Status
        +DateTime? CheckInTime
        +DateTime? CheckOutTime
        +string? Notes
        +DateTime AssignedAt
        +DateTime? UpdatedAt
        +Shift Shift
        +Staff Staff
    }

    class EventStatus {
        <<enumeration>>
        Requested
        Planned
        Confirmed
        Active
        Completed
        SendInvoice
        Cancelled
    }

    class ShiftStatus {
        <<enumeration>>
        Open
        Full
        InProgress
        Completed
        Cancelled
    }

    class StaffRole {
        <<enumeration>>
        FirstAider
        TeamLeader
        Paramedic
        Doctor
        Volunteer
    }

    class AssignmentStatus {
        <<enumeration>>
        Assigned
        Confirmed
        CheckedIn
        CheckedOut
        NoShow
        Cancelled
    }

    Event "1" --> "0..*" Shift : contains
    Shift "1" --> "0..*" StaffAssignment : has
    Staff "1" --> "0..*" StaffAssignment : assigned via
    Event --> EventStatus
    Shift --> ShiftStatus
    Staff --> StaffRole
    StaffAssignment --> AssignmentStatus
```

---

## 4. Event Status State Machine

The lifecycle of an `Event` from creation through completion and invoicing.

```mermaid
stateDiagram-v2
    [*] --> Requested : Event created

    Requested --> Planned : Mark as planned\n(triggers contact email\nif ContactEmail present)
    Planned --> Confirmed : Auto-promoted after\nplanned notification sent
    Confirmed --> Active : Event starts
    Active --> Completed : Event ends
    Completed --> SendInvoice : Issue invoice\n(triggers invoice email)
    SendInvoice --> [*] : Invoicing complete

    Requested --> Cancelled : Cancel event
    Planned --> Cancelled : Cancel event
    Confirmed --> Cancelled : Cancel event
    Active --> Cancelled : Cancel event

    note right of Planned
        Notification sent to ContactEmail.
        Status auto-advances to Confirmed
        once email is dispatched.
    end note

    note right of SendInvoice
        Invoice email sent to ContactEmail.
    end note
```

---

## 5. Shift Status State Machine

The lifecycle of a `Shift` within an event.

```mermaid
stateDiagram-v2
    [*] --> Open : Shift created

    Open --> Full : RequiredStaff\nassignments reached
    Full --> Open : Assignment removed
    Open --> InProgress : Shift start time reached
    Full --> InProgress : Shift start time reached
    InProgress --> Completed : Shift end time reached

    Open --> Cancelled : Shift cancelled
    Full --> Cancelled : Shift cancelled
    InProgress --> Cancelled : Shift cancelled
```

---

## 6. Staff Assignment Status State Machine

Tracks attendance and duty status for a single staff member assigned to a shift.

```mermaid
stateDiagram-v2
    [*] --> Assigned : Staff member assigned

    Assigned --> Confirmed : Staff confirms attendance
    Confirmed --> CheckedIn : Staff checks in at event
    CheckedIn --> CheckedOut : Staff checks out

    Assigned --> NoShow : Staff does not appear
    Confirmed --> NoShow : Staff does not appear

    Assigned --> Cancelled : Assignment cancelled
    Confirmed --> Cancelled : Assignment cancelled
```

---

## 7. Update Event Sequence Diagram

Detailed flow through `EventService.UpdateEventAsync`, including domain rules and email side-effects.

```mermaid
sequenceDiagram
    actor User
    participant Page as Blazor Page
    participant ES as EventService
    participant EDS as EventDomainService
    participant Repo as IEventRepository
    participant Email as IEmailService

    User->>Page: Submit event update form
    Page->>ES: UpdateEventAsync(updated)

    ES->>ES: Validate dates\n(EndDate > StartDate)

    ES->>Repo: GetEventByIdAsync(id)
    Repo-->>ES: existing (tracked entity)

    ES->>EDS: ApplyChanges(existing, updated)
    Note over EDS: Copies mutable fields onto<br/>existing entity.<br/>Evaluates notification rules.
    EDS-->>ES: EventStatusChangeDecision

    alt ShouldSendPlannedNotification == true
        ES->>Email: SendEventPlannedNotificationAsync(existing)
        Email-->>ES: OK
        ES->>ES: existing.Status = Confirmed\nexisting.NotificationSent = true
    end

    alt ShouldSendInvoiceNotification == true
        ES->>Email: SendEventInvoiceNotificationAsync(existing)
        Email-->>ES: OK
        ES->>ES: existing.NotificationSent = true
    end

    ES->>Repo: UpdateEventAsync(existing)
    Repo-->>ES: saved entity

    ES-->>Page: updated Event
    Page-->>User: Show success message
```

---

## 8. Database Entity-Relationship Diagram

```mermaid
erDiagram
    Events {
        int Id PK
        nvarchar Name
        datetime2 StartDate
        datetime2 EndDate
        nvarchar Location
        nvarchar Description
        int Status
        nvarchar ContactPerson
        nvarchar ContactPhone
        nvarchar ContactEmail
        bit NotificationSent
        datetime2 CreatedAt
        datetime2 UpdatedAt
    }

    Shifts {
        int Id PK
        int EventId FK
        nvarchar Name
        datetime2 StartTime
        datetime2 EndTime
        int RequiredStaff
        nvarchar Description
        int Status
        datetime2 CreatedAt
        datetime2 UpdatedAt
    }

    Staff {
        int Id PK
        nvarchar FirstName
        nvarchar LastName
        nvarchar Email
        nvarchar Phone
        int Role
        nvarchar CertificationLevel
        datetime2 CertificationExpiry
        bit IsActive
        datetime2 CreatedAt
        datetime2 UpdatedAt
    }

    StaffAssignments {
        int Id PK
        int ShiftId FK
        int StaffId FK
        int Status
        datetime2 CheckInTime
        datetime2 CheckOutTime
        nvarchar Notes
        datetime2 AssignedAt
        datetime2 UpdatedAt
    }

    AspNetUsers {
        nvarchar Id PK
        nvarchar UserName
        nvarchar Email
        nvarchar PasswordHash
    }

    Events ||--o{ Shifts : "contains (cascade delete)"
    Shifts ||--o{ StaffAssignments : "has (cascade delete)"
    Staff ||--o{ StaffAssignments : "assigned via (cascade delete)"
```
