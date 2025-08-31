# Axion

**Axion** is a modern, full-stack workspace and project management platform designed to help teams collaborate efficiently. Built with **React v18+** on the frontend and **.NET 7+ / C#** on the backend, Axion emphasizes **performance, responsive design (RWD), real-time updates, and maintainable architecture**—perfect for professional portfolios.

---

## 🌟 Features

### Core Functionality

- **User Authentication & Roles**
  - Employee, Manager, and Admin roles
  - Secure login, JWT-based authentication
- **Project & Task Management**
  - Create and manage projects
  - Assign tasks, deadlines, and priorities
  - Kanban-style task board with drag-and-drop support
- **Workspace Booking**
  - Reserve meeting rooms, desks, and equipment
  - Calendar view with availability
  - Conflict detection and prevention
- **Real-Time Collaboration**
  - Task-level comments
  - In-app notifications via SignalR
- **Analytics & Reports**
  - Productivity dashboards
  - Resource utilization reports
  - Export reports (PDF/Excel)
- **Responsive Design**
  - Mobile-first UI, works on all devices
  - Accessible design with Tailwind CSS

### Optional/Advanced

- Integration with Google/Outlook Calendar
- Email notifications
- Export/import tasks and projects
- Potential AI task prioritization (future enhancement)

---

## 🛠️ Technology Stack

| Layer            | Technology                                                                      |
| ---------------- | ------------------------------------------------------------------------------- |
| Frontend         | React v18+, TypeScript, Tailwind CSS                                            |
| Routing          | React Router v6+                                                                |
| State Management | TanStack Query (server state), Zustand (local/global state)                     |
| Styling          | Tailwind CSS, optional Shadcn UI components                                     |
| Performance      | React Suspense, Concurrent Rendering, Lazy Loading, Memoization, Virtualization |
| Testing          | Jest + React Testing Library, Vitest, Cypress/Playwright, Storybook             |
| Backend          | .NET 7+, C#, Entity Framework Core, async/await                                 |
| API              | RESTful API, Swagger/OpenAPI documentation                                      |
| Database         | SQL Server LocalDB / PostgreSQL                                                 |
| CI/CD            | GitHub Actions, Docker, Azure Deployment                                        |
| Monitoring       | Sentry, Plausible Analytics                                                     |

---

## 🏗️ Project Structure

```bash
axion/
│
├── frontend/ # React SPA
│ ├── public/
│ ├── src/
│ │ ├── components/
│ │ ├── pages/
│ │ ├── hooks/
│ │ ├── state/ # Zustand / Query clients
│ │ ├── utils/
│ │ └── App.tsx
│ ├── package.json
│ └── tailwind.config.js
│
├── backend/ # .NET API
│ ├── Controllers/
│ ├── Models/
│ ├── Data/ # EF Core context
│ ├── Services/
│ ├── Program.cs
│ └── Startup.cs
│
├── docs/
├── tests/ # Frontend and backend tests
└── README.md
```

---

## 🚀 Getting Started

### Prerequisites

- Node.js v18+
- .NET 7 SDK
- SQL Server LocalDB (included with Visual Studio) or PostgreSQL
- Git

### Quick Start

1. **Clone the repository**

   ```bash
   git clone <repository-url>
   cd axion
   ```

2. **Setup Backend**

   ```bash
   cd backend/Axion.API
   dotnet restore
   dotnet run
   ```

   The API will be available at `https://localhost:7001` or `http://localhost:5000`

3. **Setup Frontend**
   ```bash
   cd frontend
   npm install
   npm start
   ```
   The React app will be available at `http://localhost:3000`

### Detailed Setup

#### Backend Setup

1. **Navigate to backend directory**

   ```bash
   cd backend/Axion.API
   ```

2. **Install dependencies**

   ```bash
   dotnet restore
   ```

3. **Configure database**

   - The default connection uses SQL Server LocalDB
   - For PostgreSQL, update the connection string in `appsettings.json`
   - The database will be created automatically on first run

4. **Run the application**

   ```bash
   dotnet run
   ```

5. **Access Swagger documentation**
   - Open `https://localhost:7001/swagger` in your browser

#### Frontend Setup

1. **Navigate to frontend directory**

   ```bash
   cd frontend
   ```

2. **Install dependencies**

   ```bash
   npm install
   ```

3. **Start development server**

   ```bash
   npm start
   ```

4. **Access the application**
   - Open `http://localhost:3000` in your browser

### Environment Configuration

#### Backend Configuration

Update `backend/Axion.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AxionDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "Jwt": {
    "Key": "your-super-secret-key-with-at-least-32-characters",
    "Issuer": "https://axion-api.com",
    "Audience": "https://axion-client.com"
  }
}
```

#### Frontend Configuration

Create `frontend/.env`:

```env
REACT_APP_API_URL=http://localhost:5000/api
```

---

## 🏛️ High-Level Architecture

The system consists of a modern frontend, a robust backend, and real-time communication, with support for notifications and analytics.

- **React SPA**: Single Page Application with Tailwind CSS and TypeScript for responsive UI.
- **.NET 7 REST API**: Handles CRUD operations, authentication, and business logic.
- **EF Core & SQL Server**: Database layer for tasks, projects, users, and resources.
- **SignalR**: Enables real-time updates for tasks and Kanban boards.
- **Notifications & Analytics**: Centralized module for sending emails, push notifications, and generating reports.

**How It Works**

1. Users log in via JWT-authenticated endpoints.
2. React SPA fetches data via REST API (TanStack Query handles caching & background updates).
3. Task/project changes and comments propagate in real-time via SignalR.
4. Resource availability and analytics are visualized in dashboards.
5. Admins can generate/export reports or manage resources.

---

## 🛣️ Roadmap

### 🏗 MVP (Minimum Viable Product)

The MVP focuses on core functionality to allow users to manage tasks and projects efficiently.

- **User authentication with roles**
  - Register, login, and role-based access control (e.g., Admin, User)
- **CRUD tasks and projects**
  - Create, read, update, delete tasks and projects
- **Kanban board**
  - Drag-and-drop interface for task management
- **Resource booking**
  - Reserve and manage shared resources
- **Real-time task updates with SignalR**
  - Instant updates across clients for task changes
- **Responsive design**
  - Optimized for desktop, tablet, and mobile screens

---

### 📊 Phase 2

Phase 2 introduces enhanced features for better analytics, communication, and usability.

- **Analytics dashboard & PDF/Excel exports**
  - Visual insights into project progress
  - Export reports in PDF/Excel formats
- **Email notifications**
  - Task reminders and project updates
- **Advanced filtering & search**
  - Easily find tasks or projects based on multiple criteria
- **Dark mode support**
  - Toggle between light and dark UI themes

---

### 🚀 Phase 3 (Portfolio / Optional)

Phase 3 focuses on advanced, optional, or portfolio-worthy features to extend functionality.

- **AI-powered task prioritization**
  - Suggest task priorities based on deadlines and workload
- **Multi-tenant SaaS support**
  - Support multiple organizations with isolated data
- **Integration with external calendars (Google, Outlook)**
  - Sync tasks and events with popular calendar services
- **Mobile app (React Native / Expo)**
  - Native mobile experience for iOS and Android

---

## 🧪 Testing

### Backend Testing

```bash
cd backend/Axion.API
dotnet test
```

### Frontend Testing

```bash
cd frontend
npm test
```

### E2E Testing (Future)

```bash
npm run test:e2e
```

---

## 📚 API Documentation

Once the backend is running, you can access the Swagger documentation at:

- `https://localhost:7001/swagger`
- `http://localhost:5000/swagger`

### Key Endpoints

- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `GET /api/projects` - Get all projects
- `POST /api/projects` - Create new project
- `GET /api/tasks` - Get all tasks
- `POST /api/tasks` - Create new task
- `GET /api/resources` - Get all resources
- `POST /api/bookings` - Create new booking

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

---

## 🆘 Support

If you encounter any issues or have questions:

1. Check the [Issues](../../issues) page
2. Create a new issue with detailed information
3. Contact the development team

---

## 🙏 Acknowledgments

- React team for the amazing frontend framework
- Microsoft for .NET and Entity Framework
- Tailwind CSS for the utility-first CSS framework
- All contributors and supporters of this project
