# 🏋️‍♂️ MenoPro - Premium Gym Management System

**MenoPro** is a modern, full-stack gym management application built with **ASP.NET Core MVC**. It moves beyond standard CRUD operations to offer a premium user experience with **Glassmorphism UI**, real-time interactive charts, and role-based portals for Members and Trainers.

---

## 🔥 Key Features

### 👤 Member Portal

- **Interactive Dashboard**: Real-time **Chart.js** analytics for Weight History and Workout Consistency.
- **Smart Workout Plans**: View assigned daily routines with integrated **Video Demonstrations** (modal popups).
- **Diet & Nutrition**: Clean, card-based interface for meal plans and calorie tracking.
- **Progress Tracking**: Log weight and workout sessions with a few clicks.

### 🎓 Trainer Pro Portal

- **Member Management**: Easy onboarding and plan assignment for clients.
- **Plan Builder**:Create custom Workout and Diet strategies.
- **Insights**: Monitor client progress and adherence to plans.

### 🎨 UI & UX Excellence

- **Glassmorphism Design**: A sleek, translucent aesthetic using modern CSS backdrop-filters.
- **Dark/Light Mode**: Fully theme-aware components with a global one-click toggle.
- **Responsive Layout**: Optimized for mobile, tablet, and desktop experiences.
- **Toast Notifications**: Replaced standard alerts with **SweetAlert2** for non-intrusive feedback.

---

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core MVC (.NET 8/9)
- **Database**: Microsoft SQL Server / Entity Framework Core
- **Frontend**: Bootstrap 5, Vanilla CSS3 (Custom Variables), JavaScript
- **Libraries**:
  - **Chart.js**: For dynamic data visualization.
  - **SweetAlert2**: For beautiful alerts and toasts.
  - **FontAwesome**: For iconography.
- **Identity**: ASP.NET Core Identity for secure Authentication & Authorization.

---

## 🚀 Getting Started

1.  **Clone the repository**

    ```bash
    git clone https://github.com/abdallah670/GymMVC.git
    ```

2.  **Configure Database**
    Update the `ConnectionStrings` in `appsettings.json` to point to your local SQL Server instance.

3.  **Run Migrations**

    ```bash
    Update-Database
    ```

4.  **Launch the App**
    Run the application via Visual Studio or `dotnet run`.

---

## 📸 Screenshots

_(Add your screenshots here)_

| Member Dashboard                            | Dark Mode                                   |
| ------------------------------------------- | ------------------------------------------- |
| ![Dashboard Placehoder](docs/dashboard.png) | ![Dark Mode Placeholder](docs/darkmode.png) |

---

## 👨‍💻 Author

Built by **[Your Name]** as a showcase of modern Full Stack .NET development.
