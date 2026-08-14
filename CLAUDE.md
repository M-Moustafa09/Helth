# Employee Health Certificate Management System
## Project Specification Document (for Claude Code)

---

## 1. Project Overview

A simple **ASP.NET Core MVC (.NET 6)** web application that allows a single **Admin** to manage employee health certificate records ("شهادة صحية"). Each employee record can be viewed as a printable/shareable PDF (styled like an official health certificate) with an embedded QR code that links to a **public, read-only** view of that employee's record — accessible without login.

The project is intentionally simple: **no complex business logic, no employee-side accounts, single Admin user.**

---

## 2. Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC — .NET 6 |
| ORM | Entity Framework Core — **Code First** |
| Database | SQL Server |
| Auth | ASP.NET Core Identity (or Cookie Authentication) — **Admin only**, single fixed admin account (no multi-tenant, no roles hierarchy) |
| PDF Generation | Claude Code to choose the most suitable .NET 6-compatible library (e.g. QuestPDF, iText7, or similar) |
| QR Code Generation | Any standard .NET QR code library (e.g. `QRCoder`) |
| Font | Noto Sans Arabic (used across the certificate view and generated PDF) |
| Language/Direction | Arabic (RTL) UI |

---

## 3. User Roles

- **Admin (single, fixed account)** — no multiple admins, no role hierarchy. Full CRUD access to employee records.
- **Public/Anonymous visitor** — accesses a single employee's record **only** via the QR code link, in **strict read-only mode**. No login, no navigation to other records, no edit/delete/share actions available.

---

## 4. Database Design (Code First — EF Core)

### 4.1 `Admin` Table
| Field | Type | Notes |
|---|---|---|
| Id | int (PK) | |
| Username | string | unique |
| PasswordHash | string | hashed via Identity/standard hashing |

> Single seeded admin record — no admin registration/self-signup UI needed.

### 4.2 `Employee` Table
| Field (EN) | Field Label (AR — shown in UI) | Type | Required |
|---|---|---|---|
| Id | — | int (PK, auto-increment, **sequential — used directly in the QR/public URL**) | — |
| PhotoPath | صورة الموظف | string (nullable — file path/URL) | **Optional** |
| Municipality | الأمانة | string | Required |
| SubMunicipality | البلدية | string | Required |
| FullName | الاسم | string | Required |
| NationalId | رقم الهوية | string | Required |
| Gender | الجنس | string (enum: ذكر / أنثى) | Required |
| Nationality | الجنسية | string | Required |
| HealthCertificateNumber | رقم الشهادة الصحية | string | Required |
| Profession | المهنة | string | Required |
| IssueDateHijri | تاريخ إصدار الشهادة الصحية (هجري) | string or date | Required |
| IssueDateGregorian | تاريخ إصدار الشهادة الصحية (ميلادي) | DateTime | Required |
| TrainingProgramType | نوع البرنامج التثقيفي | string | Required |
| TrainingProgramExpiryDate | تاريخ انتهاء البرنامج التثقيفي | DateTime | Required |
| LicenseNumber | رقم الرخصة | string | Required |
| FacilityName | اسم المنشأة | string | Required |
| FacilityNumber | رقم المنشأة | string | Required |
| CreatedAt | — | DateTime | auto |
| UpdatedAt | — | DateTime? | auto |

> **Delete behavior:** Hard Delete — record is permanently removed from the database (with its photo file, if applicable).

---

## 5. Pages / Routes

### 5.1 `/Account/Login` — Admin Login Page
- Simple login form: Username + Password.
- On success → redirect to Dashboard (`/Employees`).
- Protected via `[Authorize]` on all admin-only controllers/actions.

### 5.2 `/Employees` — Dashboard (Home Page, Admin only, `[Authorize]`)
- Grid/list of all employees: **photo + name** per card/row.
- Each item has a **"عرض" (View)** button → navigates to Employee Details page.
- **"إضافة موظف" (Add Employee)** button → opens the Add Employee form (same layout/design as the details form, but empty and editable).
- **Search bar** — searches by **Name AND National ID**.
- **Logout** button.

### 5.3 `/Employees/Details/{id}` — Employee Details Page (Admin only, `[Authorize]`)

**Layout (top to bottom):**

1. **Header container** — single container, two elements on the same level (not stacked):
   - Hamburger menu icon → **right side** (RTL context)
   - Logo → **left side**
2. **Page Title**: "شهادة صحية للأنشطة التجارية" — centered, roughly mid-page prominence as a section header.
3. **Employee Photo** — centered, directly below the title.
4. **Read-only Form** — directly below the photo (minimal spacing, no large gap). Non-editable by default. Contains all 15 fields listed in section 4.2, **in this exact order**:
   1. الأمانة
   2. البلدية
   3. الاسم
   4. رقم الهوية
   5. الجنس
   6. الجنسية
   7. رقم الشهادة الصحية
   8. المهنة
   9. تاريخ إصدار الشهادة الصحية (هجري)
   10. تاريخ إصدار الشهادة الصحية (ميلادي)
   11. نوع البرنامج التثقيفي
   12. تاريخ انتهاء البرنامج التثقيفي
   13. رقم الرخصة
   14. اسم المنشأة
   15. رقم المنشأة

   Each field = **label** (bold) directly above **value** (regular weight, not bold).

**Styling for this page:**
- Background: white
- Text color: black
- Font: **Noto Sans Arabic** throughout
- Field labels: **bold**
- Field values: regular weight

**Action Buttons (Admin only, visible on this page):**
- **تعديل (Edit)** — toggles the form into an editable state; while in edit mode, a **حذف (Delete)** action also becomes available/visible.
- **حذف (Delete)** — hard-deletes the employee record (with confirmation prompt).
- **مشاركة (Share)** — generates and downloads/opens a **PDF** version of this record (see Section 6).

### 5.4 `/Employees/Create` — Add Employee Page (Admin only, `[Authorize]`)
- **Exact same visual layout, field order, colors, and fonts** as the Details page form, but:
  - Empty fields (create mode)
  - All fields editable/fillable
  - Photo upload is **optional**
  - Save button creates the record and redirects to its Details page.

### 5.5 `/Public/Employee/{id}` — Public Read-Only View (**No Authentication Required**)
- Accessed exclusively via the QR code embedded in the shared PDF.
- URL is based on the employee's **sequential integer Id** (e.g. `/Public/Employee/12`).
- Displays the **exact same form layout** as the Admin Details page (same fields, same order, same styling: white background, black text, Noto Sans Arabic, bold labels/regular values).
- **Strictly read-only**: no Edit, Delete, Share, Login, or navigation to other employee records. No admin UI elements (no hamburger-triggered admin menu items, no dashboard link).

---

## Reference Design Image
See: `./reference-design/health-certificate-sample.png` — this is the official health certificate sample referenced throughout this document, especially in Section 6 (PDF Generation).

## 6. PDF Generation ("مشاركة" / Share feature)

- Triggered from the Employee Details page ("مشاركة" button).
- Generates a PDF styled after the official health certificate reference provided (green header band, same field order/labels, same layout logic: photo + identity fields + certificate fields).
- Must include:
  - Employee photo
  - All 15 data fields, clearly labeled
  - A **QR code** encoding the URL to the public read-only page: `/Public/Employee/{id}`
- Font: Noto Sans Arabic (or closest available match in the chosen PDF library)
- Claude Code should choose the best-fitting, actively maintained .NET 6-compatible PDF library (e.g. QuestPDF, iText7) to implement this.

---

## 7. Search Functionality

- Located on the Dashboard (`/Employees`).
- Simple text input; searches match against:
  - `FullName` (الاسم)
  - `NationalId` (رقم الهوية)
- Case-insensitive, partial match (e.g. `Contains`).

---

## 8. Non-Functional / Implementation Notes

- **Single Admin account** — seed one Admin record via EF Core migration/seed data; no admin registration UI needed.
- **Hard Delete** for employee removal — also delete the associated photo file from storage if one exists.
- **QR/Public URL** uses the employee's plain sequential database Id (no GUID needed — simplicity was explicitly prioritized over obscurity here).
- **Employee photo** is optional at creation; if absent, show a placeholder avatar wherever the photo would appear (dashboard card, details page, PDF).
- Keep the project structure simple and idiomatic MVC:
  - `Controllers/`: `AccountController`, `EmployeesController`, `PublicController`
  - `Models/`: `Admin`, `Employee`, and appropriate ViewModels (e.g. `EmployeeFormViewModel`, `EmployeeSearchViewModel`)
  - `Views/`: `Account/Login`, `Employees/Index`, `Employees/Details`, `Employees/Create`, `Employees/Edit` (or a shared partial view for the form, reused across Details/Create/Edit/Public), `Public/Employee`
  - `Data/`: `ApplicationDbContext` (EF Core Code First), Migrations
  - `Services/`: PDF generation service, QR code generation service
- Use a **shared partial view / component** for the employee form (used identically across Details/View, Create, Edit, and the Public page) to guarantee the layout, order, and styling stay perfectly consistent across all four contexts, as required.
- Reference design source: the uploaded health certificate sample (green header "شهادة صحية", logos top-right, photo + QR top-left, field pairs of label/value, footer contact bar) should guide the **PDF export** styling specifically. The in-app Details/Create/Public pages use the simpler white-background/black-text style described in Section 5.3.

---

## 9. Out of Scope (explicitly, per client confirmation)

- No employee self-login/employee accounts
- No multiple admins or role hierarchy
- No soft-delete / recovery of deleted records
- No attendance, payroll, leave management, or other HR modules
- No GUID-based obscure URLs — plain sequential IDs are acceptable
