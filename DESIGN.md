# Design System

<!-- impeccable:design-schema 1 -->

## Direction

- Key: `253c91cd`
- Mode: `Operate` (Engineering & Technical Modeling Workstation)
- Visual Identity: Precision Technical Drafting Workstation (Pure Light Mode Native / "Light Mode Maxx").
- Thesis: Zero-bloat interactive SVG UML modeling canvas with dense IDE-grade chrome, direct SignalR live synchronization, and instant multi-user drawing.

## Palette & Materials

- **Canvas Ground:** Pure Paper White (`#FFFFFF`) with 20px geometric dot matrix (`#CBD5E1`) reserved strictly for the drawing canvas surface.
- **Chrome & Workspace Ground:** Crisp Slate Tones (`#F8FAFC`, `#F1F5F9`, `#E2E8F0`).
- **Graphite Ink & Typography:** Deep Technical Slate (`#0F172A`), Secondary Text (`#334155`, `#475569`), Monospace Muted (`#64748B`, `#94A3B8`).
- **Technical Cobalt Accent (Selection & Active Tools):** Technical Blue (`#2563EB`, `#3B82F6`, `#1D4ED8`, Background Wash `#EFF6FF`, `#DBEAFE`).
- **Signal Emerald (Live Status & Permissions):** Connected / Live Beacon (`#059669`, `#10B981`, Background Wash `#ECFDF5`).
- **Amber Warning (Owner / Organizer / View Only):** Amber (`#D97706`, `#B45309`, Background Wash `#FFFBEB`).
- **Rose Danger (Destructive Actions / Errors):** Crimson (`#E11D48`, `#DC2626`, `#BE123C`, Background Wash `#FFF1F2`).

## Typography & Typesetting

- **UI Chrome & Navigation:** `Inter`, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif (weights: 400 regular, 500 medium, 600 semibold, 700 bold).
- **Class Signatures, Code Tokens & Join Codes:** `JetBrains Mono`, ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace (tabular numerals, code syntax highlighting).
- **Scale:**
  - Micro / Badges / Coordinate readouts: `10px - 11px`
  - Body / Form Inputs / Tool labels: `12px - 13px`
  - Headers / Modal Titles: `14px - 16px`

## Component Grammar & Architecture

- **UML Class Box (3 Compartments):**
  1. Top Header: Centered bold class title + stereotype.
  2. Attributes Section: Monospace attribute definitions with visibility symbols (`+`, `-`, `#`, `~`).
  3. Methods Section: Monospace method signatures.
  - Interactive perimeter anchor points (N, E, S, W) for relationship line snapping.
  - 1.5px technical cobalt selection ring + corner resize handle.
  - Quick action toolbar on hover/selection (Edit modal, Connect line, Delete).
- **Relationship Lines:**
  - Dynamic box perimeter intersection tracking.
  - Arrowhead markers (`#uml-arrow`).
  - Middle text cardinality / association labels with crisp white background pills.
- **Top Navigation Bar:**
  - Workspace logo, Team switcher dropdown with join codes, Live meeting beacon, Canvas tool modes (`V`, `C`, `L`), Zoom controls (`-`, `+`, `100%`, `0`), Magnetic Grid Snap toggle (`20px`), Draw Permission badge, SignalR status indicator, API Endpoint settings, User profile dropdown.
- **Left Collapsible Sidebar (Navigator):**
  - Tab 1: Teams Hub (Team list, Member count, Join by code modal, Create team modal, Copy code).
  - Tab 2: Team Members & Roles (Owner / Organizer / Member badges, Owner administration context menu to promote/demote or ban).
  - Tab 3: Meetings (Active meeting alert, 1-click Join, Start meeting button, Leave / End meeting).
- **Right Collapsible Dock (Communication & Inspector):**
  - Tab 1: In-Meeting Real-time Chat (Instant chat stream, usernames, timestamps, auto-scroll).
  - Tab 2: Meeting Room Roster (Active participants, live draw badges, 1-click organizer draw permission toggles).
  - Tab 3: Element Inspector & Code Generator (Live C#, TypeScript, and Python code generation with 1-click copy).

## Accessibility & Performance

- Strict contrast ratios exceeding WCAG AAA standard on all text.
- Full keyboard shortcuts (`V`, `C`, `L`, `0`, `Space + Drag`, `Escape`).
- Zero unnecessary re-renders; SVG canvas uses hardware-accelerated SVG transform matrix.
