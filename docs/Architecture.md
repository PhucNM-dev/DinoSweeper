# Architecture Overview

This document describes the architecture and design principles of the DinoSweeper project.

---

## 🏗️ High-Level Design

- **Separation of Concerns**  
  Game logic is isolated from UI to ensure maintainability and testability.

- **WPF MVVM-like Structure**  
  Although not a full MVVM implementation, the project separates logic classes from XAML views.

- **Centralized Resource Management**  
  Assets and helper classes are managed in a single place (`Utils/Assets.cs`).

---

## 📂 Project Components

- **Logic/**
  - `Board.cs` – Represents the game board and mine placement.
  - `Cell.cs` – Defines the state of each cell (revealed, flagged, mined).
  - `MainLogic.cs` – Handles game rules, timer, hints, and win/lose detection.

- **Views/**
  - `MainWindow.xaml` – Main game interface.
  - `SettingsWindow.xaml` – Difficulty and mode configuration.

- **Utils/**
  - `Assets.cs` – Provides centralized access to image/icon resources.

- **Assets/**
  - Contains images and icons used in the UI.

- **docs/**
  - Documentation files and AI assistance logs.

---

## 🔄 Data Flow

1. **User Interaction**  
   - Player clicks on a cell in the UI.  
2. **Logic Processing**  
   - `MainLogic` updates the board state and checks rules.  
3. **UI Update**  
   - Views reflect the updated state (revealed cells, flagged mines, timer).  
4. **Result Handling**  
   - Victory or defeat dialog is shown with corresponding assets.

---

## 🎯 Design Principles

- **Clean Architecture** – Clear separation between logic and presentation.  
- **Extensibility** – Easy to add new difficulty levels or features.  
- **Maintainability** – Centralized assets and modular code structure.  
- **Transparency** – AI assistance documented in `/docs/ai-assistance/`.  

---

## ✅ Summary

The DinoSweeper project follows a simple but effective architecture:  
- Logic classes handle the game rules.  
- Views provide the interface.  
- Utilities and assets keep resources organized.  

This ensures the project is easy to understand, extend, and maintain.
