# 🏋️‍♂️ MenoPro - Premium Gym Management System

**MenoPro** is a modern, full-stack gym management application built with **ASP.NET Core MVC**. It moves beyond standard CRUD operations to offer a premium user experience with **Glassmorphism UI**, real-time interactive charts, and role-based portals for Members and Trainer.

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

## 🔑 Configuration & Secrets

No real secrets are stored in the repository. Configuration is layered:

1. **`appsettings.json`** — non-secret structure only (hosts, ports, flags).
2. **Development** — use [user-secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets):

   ```bash
   cd GymPL
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.;Database=GymAppV2;Integrated Security=SSPI;TrustServerCertificate=True"
   dotnet user-secrets set "EmailSettings:SmtpUser" "you@gmail.com"
   dotnet user-secrets set "EmailSettings:SmtpPass" "your-app-password"
   dotnet user-secrets set "Stripe:SecretKey" "sk_test_..."
   dotnet user-secrets set "Stripe:PublishableKey" "pk_test_..."
   dotnet user-secrets set "Stripe:WebhookSecret" "whsec_..."
   dotnet user-secrets set "Authentication:Google:ClientId" "..."
   dotnet user-secrets set "Authentication:Google:ClientSecret" "..."
   dotnet user-secrets set "GeminiSettings:ApiKey" "..."
   ```

3. **Production** — environment variables (double underscore = section separator):

   ```
   ConnectionStrings__DefaultConnection
   EmailSettings__SmtpUser, EmailSettings__SmtpPass
   Stripe__SecretKey, Stripe__PublishableKey, Stripe__WebhookSecret
   Authentication__Google__ClientId, Authentication__Google__ClientSecret
   GeminiSettings__ApiKey
   ```

Features degrade gracefully when keys are missing: the AI chat and email sending report
"not configured" instead of crashing, and Google sign-in stays disabled.

### 💳 Stripe (Payments)

1.  Sign up or log in to the [Stripe Dashboard](https://dashboard.stripe.com/).
2.  Enable **Test Mode**.
3.  Go to **Developers > API keys** to get your `Publishable key` and `Secret key`.
4.  To get the `WebhookSecret`:
    - Install the [Stripe CLI](https://stripe.com/docs/stripe-cli).
    - Run `.\stripe login` and then `.\stripe listen --forward-to https://localhost:5000/api/StripeWebhook`.
    - The CLI will provide a webhook signing secret starting with `whsec_`.

### 📧 Email (SMTP)

The app uses Gmail for sending notifications.

1.  Go to your [Google Account Settings](https://myaccount.google.com/security).
2.  Enable **2-Step Verification**.
3.  Search for **App Passwords**.
4.  Create a new app password (e.g., named "GymMVC").
5.  Store it as the `EmailSettings:SmtpPass` user-secret (see Configuration & Secrets above).

### 🌐 Google Authentication

1.  Go to the [Google Cloud Console](https://console.cloud.google.com/).
2.  Create a new project.
3.  Navigate to **APIs & Services > Credentials**.
4.  Click **Create Credentials > OAuth client ID**.
5.  Configure the **OAuth consent screen** if prompted.
6.  Set the application type to **Web application**.
7.  Add `https://localhost:5000/signin-google` to **Authorized redirect URIs**.
8.  Copy the `Client ID` and `Client Secret`.

### 🤖 Gemini AI

1.  Go to [Google AI Studio](https://aistudio.google.com/).
2.  Click on **Get API key**.
3.  Create a new API key in a new or existing project.
4.  Copy the key to the `GeminiSettings:ApiKey` user-secret (see Configuration & Secrets above).

---

## 👨‍💻 Author

Built by **Abdallah Mohamed** as a showcase of modern Full Stack .NET development.
