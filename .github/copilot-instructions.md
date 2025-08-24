## Copilot Instructions for Medical First Aid Event & Shift Manager (Blazor)

### Project Overview
This Blazor application manages events and shifts for a medical first aid team. Users can create, view, and manage events, assign staff to shifts, and track attendance. The app should be visually attractive, easy to use, and fully responsive for desktop and mobile devices.

---

### Design & UI Guidelines
- Use **Bootstrap** for all UI components to ensure a modern, responsive design.
- Prioritize **mobile usability**: test layouts on small screens, use offcanvas navigation, and touch-friendly controls.
- Apply a **clean, professional color scheme** (e.g., blue, white, and accent colors for alerts/status).
- Use **Bootstrap Cards** for event and shift listings.
- Include a **dashboard** with summary stats (upcoming events, active shifts, staff on duty).
- Use **modals** for creating/editing events and shifts.
- Navigation should be clear and simple (sidebar or top navbar).

---

### Features
- **Event Management**: Create, edit, delete, and view events (name, date, location, description).
- **Shift Scheduling**: Assign staff to shifts, view shift rosters, mark attendance.
- **Staff Directory**: List staff members, contact info, roles.
- **Responsive Calendar View**: Show events and shifts in a calendar (Bootstrap or third-party calendar component).
- **Notifications**: Alert staff of upcoming shifts/events (use Bootstrap alerts/toasts).

---

### Coding Standards
- Follow **SOLID principles** and clean architecture.
- Use **Blazor components** for reusable UI parts.
- Organize code into clear folders: `Components`, `Pages`, `Services`, `Models`.
- Use **dependency injection** for services.
- Write **async code** for data access and UI updates.
- Add **XML comments** for public methods/classes.
- Use **meaningful variable and method names**.
- Avoid code duplication; refactor common logic into helpers/services.
- Add **unit tests** for business logic.

---

### Bootstrap Integration
- Reference Bootstrap via CDN or include in `wwwroot/lib`.
- Use Bootstrap grid system for layouts.
- Prefer Bootstrap form controls, buttons, navbars, cards, modals, and alerts.
- Customize Bootstrap with a site-specific theme in `wwwroot/app.css`.

---

### Accessibility & Usability
- Ensure all interactive elements are keyboard accessible.
- Use ARIA attributes for modals, alerts, and navigation.
- Provide clear error messages and validation feedback.
- Test with screen readers and mobile devices.

---

### Example UI Components
- **Event Card**: Shows event details, edit/delete buttons.
- **Shift Modal**: Assign staff, select date/time, save/cancel.
- **Staff List**: Table or cards with contact info and role badges.
- **Dashboard**: Cards for stats, upcoming events, active shifts.

---

### Final Notes
- Keep UI simple, clean, and attractive.
- Use Bootstrap icons for visual cues.
- Document all major components and services.
- Ensure the app is easy to extend for future features (e.g., shift swap, event reports).

