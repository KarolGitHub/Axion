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

### 📊 Current Status

| Phase       | Status           | Completion |
| ----------- | ---------------- | ---------- |
| **MVP**     | ✅ **COMPLETED** | 100%       |
| **Phase 2** | ✅ **COMPLETED** | 100%       |
| **Phase 3** | ✅ **COMPLETED** | 100%       |
| **Phase 4** | 🔄 **PLANNED**   | 0%         |
| **Phase 5** | 🔄 **PLANNED**   | 0%         |
| **Phase 6** | 🔄 **PLANNED**   | 0%         |
| **Phase 7** | 🔄 **PLANNED**   | 0%         |

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

### 🚀 Phase 3 (Portfolio / Optional) ✅ COMPLETED

Phase 3 focuses on advanced, optional, or portfolio-worthy features to extend functionality.

- **AI-powered task prioritization** ✅
  - Suggest task priorities based on deadlines and workload
  - Interactive AI insights component with workload analysis
  - Smart task suggestions and deadline analysis
- **Multi-tenant SaaS support** ✅
  - Support multiple organizations with isolated data
  - Organization model with plan management
  - Resource limits and usage tracking
- **Integration with external calendars (Google, Outlook)** ✅
  - Sync tasks and events with popular calendar services
  - Calendar conflict detection and resolution
  - Export functionality (ICS, CSV, JSON)
- **Mobile app (React Native / Expo)** ✅
  - Native mobile experience for iOS and Android
  - Cross-platform navigation and UI components
  - Offline support and push notifications

---

### 🔥 Phase 4 (Enterprise Features)

Phase 4 introduces enterprise-grade features for professional teams and organizations.

- **Real-time collaboration & communication**
  - Live chat system with @mentions and notifications
  - Video conferencing integration (Zoom/Teams)
  - Activity feed with real-time updates
  - Comments and threaded discussions
- **Advanced security & compliance**
  - SSO integration (SAML, OAuth, LDAP)
  - Two-factor authentication (SMS, email, authenticator)
  - Audit logging and activity tracking
  - GDPR compliance and data privacy
  - Role-based permissions and access control
- **Third-party integrations**
  - GitHub/GitLab code repository integration
  - Slack/Discord team communication
  - Google Workspace and Microsoft 365
  - CRM systems (Salesforce, HubSpot)
  - Import from Trello, Jira, Asana
- **Advanced analytics & reporting**
  - Custom dashboard builder
  - Predictive analytics and ML insights
  - Burndown charts and agile tracking
  - Resource utilization reports
  - ROI tracking and cost analysis

---

### ⚡ Phase 5 (Performance & Scalability)

Phase 5 focuses on technical excellence and platform scalability.

- **Microservices architecture**
  - Break down monolithic backend into services
  - API gateway and service discovery
  - Event-driven architecture
  - Distributed tracing and monitoring
- **Performance optimization**
  - Redis caching layer
  - CDN integration for global delivery
  - Database optimization and read replicas
  - API rate limiting and protection
- **DevOps & deployment**
  - Docker containerization
  - Kubernetes orchestration
  - CI/CD pipeline automation
  - Blue-green deployment strategy
- **Advanced mobile features**
  - Offline mode with full functionality
  - Rich push notifications with actions
  - Biometric authentication
  - Camera integration and QR codes
  - Location services and GPS tracking

---

### 🎯 Phase 6 (Business Intelligence)

Phase 6 introduces advanced business features and intelligence.

- **Financial management**
  - Budget tracking and cost monitoring
  - Invoice generation and automated billing
  - Expense management with receipt scanning
  - Multi-currency support
  - Tax compliance and reporting
- **Business intelligence**
  - Data warehouse and ETL processes
  - Custom SQL report builder
  - Interactive data visualization
  - KPI dashboards and metrics
  - Trend analysis and forecasting
- **Workflow automation**
  - Visual workflow designer
  - Conditional logic and automation rules
  - Webhook support for external systems
  - Scheduled tasks and approvals
  - Escalation rules and notifications
- **Customer management**
  - Client portal with external access
  - Project templates and best practices
  - Contract management and digital signatures
  - White-label solutions and branding
  - API marketplace for integrations

---

### 🚀 Phase 7 (Innovation & Future)

Phase 7 explores cutting-edge technologies and innovation.

- **AI/ML capabilities**
  - Natural language processing for task creation
  - Smart scheduling and optimization
  - Document analysis and insights
  - Sentiment analysis and team mood tracking
  - Predictive maintenance and resource planning
- **Emerging technologies**
  - Blockchain integration for audit trails
  - IoT device integration and smart office
  - AR/VR support for project visualization
  - Voice assistants (Alexa, Google Assistant)
  - Edge computing and local processing
- **Advanced collaboration**
  - Virtual whiteboards and real-time collaboration
  - Mind mapping and visual planning
  - Team retrospectives and agile ceremonies
  - Knowledge base and documentation
  - Learning paths and training modules
- **Personalization & accessibility**
  - Custom themes and user preferences
  - Personalized dashboards and layouts
  - Multi-language support and internationalization
  - WCAG compliance and accessibility features
  - Smart recommendations and AI insights

---

### 🎯 Roadmap Priorities & Timeline

#### **Immediate Priority (Next 3-6 months)**

- **Phase 4: Enterprise Features** - Focus on real-time collaboration, security, and integrations
- **High Impact Features**: Live chat, SSO, third-party integrations, advanced analytics

#### **Medium Term (6-12 months)**

- **Phase 5: Performance & Scalability** - Technical foundation for growth
- **Key Focus**: Microservices, caching, DevOps automation, mobile offline mode

#### **Long Term (12+ months)**

- **Phase 6: Business Intelligence** - Advanced business features
- **Phase 7: Innovation & Future** - Cutting-edge technologies

#### **Success Metrics**

- **User Adoption**: Monthly active users, feature usage
- **Performance**: Page load times, API response times
- **Business**: Revenue growth, customer satisfaction
- **Technical**: System uptime, error rates, scalability

#### **Risk Mitigation**

- **Technical Debt**: Regular refactoring and code reviews
- **Security**: Regular security audits and penetration testing
- **Scalability**: Performance monitoring and capacity planning
- **User Experience**: Continuous feedback collection and A/B testing

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
