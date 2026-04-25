# DinoSweeper (MineSweeper Assessment)

DinoSweeper is a WPF-based Minesweeper clone built as part of a technical assessment.  
It demonstrates clean architecture, separation of logic and UI, and centralized resource management.

---

## 🚀 Quick Start

1. Clone the repository:
   ```bash
   git clone https://github.com/PhucNM-dev/DinoSweeper.git
   
2. Open the solution in Visual Studio 2022 (Community edition or higher).

3. Build the solution (Ctrl+Shift+B).

4. Run the project (F5).

---

## 📂 Project Structure

- **MineSweeper.sln** – Visual Studio solution file.  
- **MineSweeper/** – Source code folder:  
  - **Logic/** – Core game logic (`Board.cs`, `Cell.cs`, `MainLogic.cs`).  
  - **Views/** – UI windows (`MainWindow.xaml`, `SettingsWindow.xaml`).  
  - **Utils/** – Helpers (`Assets.cs`).  
  - **Assets/** – Images and icons used in the game.  
- **docs/** – Documentation and AI assistance logs.  

---

## 🎮 Features

- Classic Minesweeper mechanics: reveal cells, flag mines.  
- Difficulty levels:  
  - Beginner (9x9, 10 mines)  
  - Intermediate (16x16, 40 mines)  
  - Expert (30x16, 99 mines)  
- Timer modes:  
  - Normal mode (count up)  
  - Countdown mode (predefined time limits: 300s, 180s, 60s)  
- Hints:  
  - Beginner: unlimited  
  - Intermediate: 3  
  - Expert: none  
- UI:  
  - Dynamic board rendering  
  - Mines counter and timer display  
  - Hint counter with interactive icon  
  - Result dialog with victory/lose images  
- Settings:  
  - Difficulty and mode selection  
  - Countdown level options  

---

## 📖 Documentation

Additional documentation is available in the `/docs` folder:  
- Installation guide  
- Architecture overview  
- Features list  
- Assessment compliance report  
- AI assistance logs]

---

## 🤖 AI Assistance Transparency

AI tools were used to assist with code refactoring, documentation, and error resolution.  
All conversations will be exported and stored in `/docs/ai-assistance/`.  
