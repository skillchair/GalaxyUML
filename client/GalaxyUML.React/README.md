# GalaxyUML — React Client

Clean, zero-bloat, precision technical UML collaborative modeling workstation built with React, TypeScript, Tailwind CSS v4, Radix UI Primitives, and `@microsoft/signalr`.

## 🚀 Getting Started

### Prerequisites
- Node.js (v18+) or Bun (v1.0+)
- Running GalaxyUML ASP.NET Core backend (`http://localhost:5248`)

### Development
```bash
# Install dependencies
bun install
# or
npm install

# Start Vite development server (with proxy to backend on localhost:5248)
bun run dev
# or
npm run dev
```

The application will be available at `http://localhost:5173`.

### Production Build
```bash
bun run build
# or
npm run build
```

## 🛠️ Architecture & Features

- **Precision Technical UI ("Light Mode Maxx"):** High-contrast, dense, legible drafting workspace with sharp 1px borders and zero generic SaaS slop.
- **Custom Zero-Bloat Interactive SVG Engine:**
  - 3-compartment UML Class Boxes (Stereotypes, Class name, Attributes with visibility `+ - # ~`, Methods with signatures).
  - Dynamic Relationship Lines with smart perimeter anchor snapping, cardinality badges, and arrowheads.
  - Interactive multi-user drag & drop with optimistic rendering and SignalR delta broadcasting.
  - Pan & Zoom engine (Mouse wheel anchored zoom, middle-click / spacebar pan, minimap radar overview).
  - Export diagram as vector SVG or high-resolution PNG.
- **Real-Time SignalR Collaboration (`/diagramHub`):**
  - Instant multi-user drawing synchronization (`ClassBoxAdded`, `ElementMoved`, `LineAdded`, `ElementDeleted`, `BoardCleared`).
  - Active participant roster with live drawing permission badges.
  - In-meeting real-time chat stream with timestamps and user avatars.
- **Team & Workspace Management:**
  - Create teams, join via 6-character codes, manage member roles (`Owner`, `Organizer`, `Member`), and ban members.
  - Start, join, and end meetings.
  - Live C#, TypeScript, and Python code generator from selected UML class diagrams.
