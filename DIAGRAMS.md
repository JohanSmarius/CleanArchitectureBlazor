# Solution Diagrams

Mermaid diagrams describing the architecture, data model, service interactions, and workflows of the **Medical First Aid Event & Shift Manager** Blazor application.

---

## 1. Solution Architecture

High-level view of the two projects and their layers.

```mermaid
graph TB
    subgraph Browser["Browser / Client"]
        WASM["CleanArchitectureBlazor.Client\n(Blazor WebAssembly)"]
    end

    subgraph Server["ASP.NET Core Server"]
        subgraph Presentation["Presentation Layer"]
            Pages["Razor Components / Pages\n(Dashboard, Events, Shifts,\nStaff, Assignments)"]
            Layout["Layout\n(MainLayout, NavMenu)"]
            Account["Identity UI\n(Login, Register)"]
        end

        subgraph AppServices["Application / Service Layer"]
            IEventSvc["IEventService"]
            IShiftSvc["IShiftService"]
            IStaffSvc["IStaffService"]
            IAssignSvc["IStaffAssignmentService"]
            IEmailSvc["IEmailService"]
        end

        subgraph Infrastructure["Infrastructure Layer"]
            DbCtx["ApplicationDbContext\n(EF Core)"]
            EmailImpl["EmailService\n(SMTP)"]
        end

        subgraph Data["Data Layer"]
            SQL[("SQL Server")]
        end
    end

    Browser <-->|"SignalR / HTTP"| Server
    Pages --> IEventSvc
    Pages --> IShiftSvc
    Pages --> IStaffSvc
    Pages --> IAssignSvc
    IEventSvc --> DbCtx
    IEventSvc --> IEmailSvc
    IShiftSvc --> DbCtx
    IStaffSvc --> DbCtx
    IAssignSvc --> DbCtx
    IAssignSvc --> IEmailSvc
    IEmailSvc --> EmailImpl
    DbCtx --> SQL
```

---

## 2. Entity Relationship Diagram

```mermaid
erDiagram
    Event {
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

    Shift {
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

    Staff {
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

    StaffAssignment {
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

    ApplicationUser {
        string Id PK
        string UserName
        string Email
        string PasswordHash
    }

    Event ||--o{ Shift : "has"
    Shift ||--o{ StaffAssignment : "has"
    Staff ||--o{ StaffAssignment : "assigned to"
```

---

## 3. Class Diagram – Models and Services

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

    class IEventService {
        <<interface>>
        +GetAllEventsAsync() Task~List~Event~~
        +GetEventByIdAsync(int) Task~Event?~
        +CreateEventAsync(Event) Task~Event~
        +UpdateEventAsync(Event) Task~Event~
        +DeleteEventAsync(int) Task
        +GetUpcomingEventsAsync() Task~List~Event~~
        +GetEventsByDateRangeAsync(DateTime,DateTime) Task~List~Event~~
    }

    class IShiftService {
        <<interface>>
        +GetAllShiftsAsync() Task~List~Shift~~
        +GetShiftByIdAsync(int) Task~Shift?~
        +GetShiftsByEventIdAsync(int) Task~List~Shift~~
        +CreateShiftAsync(Shift) Task~Shift~
        +UpdateShiftAsync(Shift) Task~Shift~
        +DeleteShiftAsync(int) Task
        +GetUpcomingShiftsAsync() Task~List~Shift~~
        +GetShiftsByDateAsync(DateTime) Task~List~Shift~~
    }

    class IStaffService {
        <<interface>>
        +GetAllStaffAsync() Task~List~Staff~~
        +GetStaffByIdAsync(int) Task~Staff?~
        +CreateStaffAsync(Staff) Task~Staff~
        +UpdateStaffAsync(Staff) Task~Staff~
        +DeleteStaffAsync(int) Task
        +GetActiveStaffAsync() Task~List~Staff~~
        +GetStaffByRoleAsync(StaffRole) Task~List~Staff~~
        +IsEmailUniqueAsync(string,int?) Task~bool~
    }

    class IStaffAssignmentService {
        <<interface>>
        +GetAllAssignmentsAsync() Task~List~StaffAssignment~~
        +GetAssignmentByIdAsync(int) Task~StaffAssignment?~
        +GetAssignmentsByShiftIdAsync(int) Task~List~StaffAssignment~~
        +GetAssignmentsByStaffIdAsync(int) Task~List~StaffAssignment~~
        +CreateAssignmentAsync(StaffAssignment) Task~StaffAssignment~
        +UpdateAssignmentAsync(StaffAssignment) Task~StaffAssignment~
        +DeleteAssignmentAsync(int) Task
        +CheckInStaffAsync(int) Task~StaffAssignment?~
        +CheckOutStaffAsync(int) Task~StaffAssignment?~
        +IsStaffAvailableAsync(int,DateTime,DateTime,int?) Task~bool~
    }

    class IEmailService {
        <<interface>>
        +SendStaffAssignmentNotificationAsync(Staff,Shift,Event) Task
        +SendEventPlannedNotificationAsync(Event) Task
        +SendEventConfirmationNotificationAsync(Event) Task
        +SendEventInvoiceNotificationAsync(Event) Task
        +SendFinancialDepartmentInvoiceNotificationAsync(Event) Task
        +SendEmailAsync(string,string,string) Task
    }

    Event "1" --> "0..*" Shift : has
    Shift "1" --> "0..*" StaffAssignment : has
    Staff "1" --> "0..*" StaffAssignment : assigned to
    IEventService ..> Event : uses
    IShiftService ..> Shift : uses
    IStaffService ..> Staff : uses
    IStaffAssignmentService ..> StaffAssignment : uses
    IStaffAssignmentService ..> IEmailService : uses
    IEventService ..> IEmailService : uses
```

---

## 4. EventStatus State Machine

```mermaid
stateDiagram-v2
    [*] --> Requested : Event created

    Requested --> Planned : Planner confirms planning\n(triggers confirmation email\nto contact → auto-advances to Confirmed)
    Planned --> Confirmed : Email sent successfully
    Confirmed --> Active : Event day starts
    Active --> Completed : Event finishes
    Completed --> SendInvoice : Invoice is ready\n(triggers invoice email\nto contact + finance dept)
    SendInvoice --> [*]

    Requested --> Cancelled
    Planned --> Cancelled
    Confirmed --> Cancelled
    Active --> Cancelled
```

---

## 5. ShiftStatus State Machine

```mermaid
stateDiagram-v2
    [*] --> Open : Shift created

    Open --> Full : All required staff\nhave been assigned
    Full --> Open : Assignment removed /\nstaff count drops below required
    Full --> InProgress : Shift start time reached
    Open --> InProgress : Shift start time reached
    InProgress --> Completed : Shift end time reached

    Open --> Cancelled
    Full --> Cancelled
    InProgress --> Cancelled
```

---

## 6. AssignmentStatus State Machine

```mermaid
stateDiagram-v2
    [*] --> Assigned : Staff assigned to shift\n(triggers email notification)

    Assigned --> Confirmed : Staff confirms availability
    Confirmed --> CheckedIn : Staff checks in at event
    CheckedIn --> CheckedOut : Staff checks out

    Assigned --> NoShow : Staff did not appear
    Confirmed --> NoShow : Staff did not appear

    Assigned --> Cancelled
    Confirmed --> Cancelled
    CheckedIn --> Cancelled
```

---

## 7. Sequence Diagram – Assigning Staff to a Shift

```mermaid
sequenceDiagram
    actor User
    participant Page as Assignments.razor
    participant AssignSvc as StaffAssignmentService
    participant ShiftSvc as ShiftService
    participant DB as ApplicationDbContext
    participant EmailSvc as EmailService
    participant SMTP as SMTP Server

    User->>Page: Select shift and staff, click Assign
    Page->>AssignSvc: IsStaffAvailableAsync(staffId, startTime, endTime)
    AssignSvc->>DB: Query overlapping StaffAssignments
    DB-->>AssignSvc: Result (available / conflict)
    AssignSvc-->>Page: true / false

    alt Staff is available
        Page->>AssignSvc: CreateAssignmentAsync(assignment)
        AssignSvc->>DB: Add StaffAssignment
        DB-->>AssignSvc: Saved (assignment.Id set)
        AssignSvc->>DB: GetAssignmentByIdAsync (with Staff + Shift + Event)
        DB-->>AssignSvc: Full assignment object
        AssignSvc->>EmailSvc: SendStaffAssignmentNotificationAsync(staff, shift, event)
        EmailSvc->>SMTP: Send email to staff member
        SMTP-->>EmailSvc: OK (or error – swallowed)
        AssignSvc-->>Page: Created assignment
        Page-->>User: Show success toast / refresh list
    else Staff has conflicting shift
        Page-->>User: Show conflict warning
    end
```

---

## 8. Sequence Diagram – Updating Event Status with Email Notifications

```mermaid
sequenceDiagram
    actor Planner
    participant Page as EventEdit.razor
    participant EventSvc as EventService
    participant DB as ApplicationDbContext
    participant EmailSvc as EmailService
    participant SMTP as SMTP Server

    Planner->>Page: Edit event, change status, click Save

    Page->>EventSvc: UpdateEventAsync(event)
    EventSvc->>DB: FindAsync(event.Id)
    DB-->>EventSvc: existingEvent

    alt Status changed to Planned (and no notification sent yet)
        EventSvc->>EmailSvc: SendEventPlannedNotificationAsync(event)
        EmailSvc->>SMTP: Send planning confirmation to ContactEmail
        SMTP-->>EmailSvc: OK
        Note over EventSvc: Auto-advance status to Confirmed\nand set NotificationSent = true
    else Status changed to SendInvoice
        EventSvc->>EmailSvc: SendEventInvoiceNotificationAsync(event)
        EmailSvc->>SMTP: Send invoice to ContactEmail
        SMTP-->>EmailSvc: OK
        EventSvc->>EmailSvc: SendFinancialDepartmentInvoiceNotificationAsync(event)
        EmailSvc->>SMTP: Send invoice copy to finance department
        SMTP-->>EmailSvc: OK
        Note over EventSvc: Set NotificationSent = true
    end

    EventSvc->>DB: SaveChangesAsync()
    DB-->>EventSvc: OK
    EventSvc-->>Page: Updated event
    Page-->>Planner: Show success message / navigate back
```

---

## 9. Component Hierarchy – Blazor Pages

```mermaid
graph TD
    App["App.razor"]
    Routes["Routes.razor"]
    MainLayout["MainLayout.razor"]
    NavMenu["NavMenu.razor"]

    Dashboard["Dashboard.razor\n/dashboard"]
    Events["Events.razor\n/events"]
    EventDetails["EventDetails.razor\n/events/{id}"]
    EventEdit["EventEdit.razor\n/events/edit/{id?}"]
    Shifts["Shifts.razor\n/shifts"]
    ShiftDetails["ShiftDetails.razor\n/shifts/{id}"]
    ShiftEdit["ShiftEdit.razor\n/shifts/edit/{id?}"]
    Staff["Staff.razor\n/staff"]
    StaffDetails["StaffDetails.razor\n/staff/{id}"]
    Assignments["Assignments.razor\n/assignments"]

    App --> Routes
    Routes --> MainLayout
    MainLayout --> NavMenu
    MainLayout --> Dashboard
    MainLayout --> Events
    MainLayout --> EventDetails
    MainLayout --> EventEdit
    MainLayout --> Shifts
    MainLayout --> ShiftDetails
    MainLayout --> ShiftEdit
    MainLayout --> Staff
    MainLayout --> StaffDetails
    MainLayout --> Assignments
```

---

## 10. Deployment / Infrastructure Overview

```mermaid
graph LR
    subgraph Client["End User Device"]
        Browser["Web Browser\n(Blazor WASM)"]
    end

    subgraph CloudHosting["Cloud / On-Premises Hosting"]
        WebServer["ASP.NET Core Web Server\n(Kestrel / IIS)"]
        subgraph BackEnd["Application"]
            BlazorServer["Blazor Server Components\n(Interactive Server Render Mode)"]
            IdentityServer["ASP.NET Core Identity"]
            Services["Application Services\n(EventService, ShiftService,\nStaffService, StaffAssignmentService)"]
        end
        DB[("SQL Server Database")]
        SMTP["SMTP Mail Server"]
    end

    Browser <-->|"HTTPS + SignalR"| WebServer
    WebServer --> BlazorServer
    WebServer --> IdentityServer
    BlazorServer --> Services
    Services --> DB
    Services --> SMTP
```
