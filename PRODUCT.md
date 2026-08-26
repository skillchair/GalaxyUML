# Product

<!-- impeccable:product-schema 1 -->

## Platform

web

## Stack
Vite + React + TypeScript + Tailwind CSS + Radix UI Primitives + @microsoft/signalr + Custom Zero-Bloat Interactive SVG/HTML Diagramming Engine

## Users
Software engineering teams, university students (AIPS), software architects, and technical team leads collaborating on object-oriented system design and UML class modeling.

## Product Purpose
GalaxyUML is a collaborative software design workstation for creating teams, scheduling meetings, communicating via real-time chat, and collaboratively architecting UML diagrams on an interactive shared canvas.

## Positioning
An uncompromising, zero-bloat precision engineering tool for collaborative software modeling—fast, light-mode native ("light mode maxx"), high-density, tactile, and directly synchronized with an ASP.NET Core SignalR backend.

## Operating Context
Engineering design sessions, system architecture reviews, computer science coursework, and team planning meetings where diagram clarity, low latency, and intuitive controls matter.

## Capabilities and Constraints
- Full authentication (JWT, register/login, session persistence).
- Team management (create team, join via 6-character code, member list, role administration: Owner/Organizer/Member, ban members, leave/delete team).
- Meeting lifecycle (start meeting, join/leave, end meeting, live participant roster, toggle draw permissions per participant).
- Real-time UML Canvas (Custom SVG/HTML Engine):
  - UML Class Boxes (Title, visibility modifiers, attributes, methods, live interactive drag, resize, editing).
  - Association / Connection Lines (Connected box endpoints, middle text label, automatic angle and connection calculation).
  - Diagram manipulation (Add class box, add line, move elements, delete element, clear board, undo/refresh).
  - Real-time multi-user synchronization via SignalR (`DiagramHub` on `/diagramHub`).
  - Active participant indicator & draw permission badge.
- Real-time in-meeting chat with instant message delivery and timestamping.
- Aesthetic constraint: Pure Light Mode Maxx—crisp technical workstation, sharp borders, high density, monospaced code tokens, no AI SaaS gradient slop.

## Brand Commitments
- Name: GalaxyUML
- Visual Voice: Precision Technical / IDE-Grade Workstation. Light-mode native, high density, sharp 1px borders, crisp monochrome ink with purposeful technical accents (cobalt / graphite / emerald indicator), monospace typography for signatures & code.

## Evidence on Hand
- Existing ASP.NET Core API at `/api/auth`, `/api/teams`, `/api/meetings`, `/api/diagram`.
- SignalR Hub at `/diagramHub`.
- Desktop clients (WPF & Avalonia) in `client/` as reference for domain logic and data structures.

## Product Principles
1. **Precision & Density:** Every pixel serves a purpose. Dense toolbars, clean typography, unambiguous states, and clear affordances without decorative fluff.
2. **Synchronous Fluidity:** Real-time updates via SignalR feel immediate and rock solid; canvas mutations update smoothly with optimistic feedback and server reconciliation.
3. **No Slop, True Craft:** Zero AI buzzword wrappers or generic gradient templates. Clean, sharp, responsive, light-mode engineering aesthetic.

## Accessibility & Inclusion
- High contrast light mode (WCAG AAA compliant text contrast).
- Keyboard navigation and shortcut keys for canvas tools (e.g. V: select/pan, C: class box, L: line, Del: delete, Esc: deselect).
- Clear screen-reader accessible form labels and ARIA attributes for modals, dropdowns, and canvas elements.
