# Social Film Platform

<div align="center">

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?style=for-the-badge&logo=dotnet)
![MySQL](https://img.shields.io/badge/MySQL-8.0-4479A1?style=for-the-badge&logo=mysql&logoColor=white)
![Google Gemini](https://img.shields.io/badge/Google%20Gemini-AI-4285F4?style=for-the-badge&logo=google&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white)

*A modern film archiving and social platform with AI-powered features*

[Features](#features) • [Tech Stack](#tech-stack) • [Installation](#installation) • [Usage](#usage) • [AI Features](#ai-features)

</div>

---

## About The Project

**Social Film Platform** is a web application built with ASP.NET Core MVC for film enthusiasts to catalog, review, and share movies. Features vintage cinema aesthetic with modern functionality and AI-powered tag suggestions.

### Key Highlights

- **Movie Archive** - Database with genres, actors, trailers, and images
- **Personal Diaries** - Curated film collections with tags and categories
- **Review System** - Write, edit, and vote on reviews
- **AI Integration** - Automatic suggestions using Google Gemini
- **Multi-Role System** - Admin, Editor, and User roles
- **Vintage UI/UX** - Classic Hollywood-inspired design

---

## Features

### Core Functionality

**Movies**
- CRUD operations
- Search by title, director, genre
- Sort by popularity or date
- Upload posters, embed trailers
- Associate with actors

**Actors**
- Profile pages with biography
- Filmography listings
- Photo uploads

**Diaries (Movie Lists)**
- Public/private collections
- Tag and category system
- Vote on diaries
- AI-powered suggestions

**Reviews**
- Rich-text reviews (up to 2000 chars)
- Like/Dislike voting
- Edit/delete capabilities
- Moderation tools

**Organization**
- Genres: Action, Sci-Fi, Drama, Comedy, Thriller, Romance, Animation, Horror
- User-defined tags and categories

### User Roles

| Role | Permissions |
|------|------------|
| **Admin** | Full access: manage all content, users, and settings |
| **Editor** | Create/edit/delete movies, actors, genres; moderate reviews |
| **User** | Create diaries, write reviews, vote on content |

---

## AI Features

The platform integrates **Google Gemini Flash AI** for intelligent content suggestions.

### How It Works

1. **Diary Analysis**: When editing a diary, click "Suggest with AI"
2. **Content Processing**: The AI analyzes:
   - Movie titles in the diary
   - Diary name and description
   - Common themes and genres
3. **Smart Suggestions**: Receives 3 relevant tags + 3 categories
4. **One-Click Apply**: Add suggestions directly to your diary

### Example Output

For a diary containing *Fight Club*, *Pulp Fiction*, and *Mulholland Drive*:

```json
{
  "tags": ["Psychological Thriller", "Cult Classics", "Mind-Bending"],
  "categories": ["Thriller", "Drama", "Neo-Noir"]
}
```

### AI Architecture

```
User Request → DiariesController.SuggestTags()
                     ↓
        Extract movie titles from diary
                     ↓
        Build context-aware prompt
                     ↓
        Google Gemini API call
                     ↓
        Parse JSON response
                     ↓
        Return tags + categories
```

**API Configuration**: Set `Gemini:ApiKey` in `appsettings.json` or User Secrets

---

## Tech Stack

### Backend
- **Framework**: ASP.NET Core 9.0 MVC
- **Language**: C# 13.0
- **ORM**: Entity Framework Core 9.0
- **Database**: MySQL 8.0 (via Pomelo.EntityFrameworkCore.MySql)
- **Authentication**: ASP.NET Core Identity with role-based authorization

### Frontend
- **UI Framework**: Bootstrap 5
- **Rich Text Editor**: Quill.js
- **Icons**: Bootstrap Icons
- **Fonts**: Google Fonts (Cinzel, Playfair Display, Lato)
- **JavaScript**: Vanilla JS with Fetch API

### AI & Security
- **AI Service**: Google Gemini Flash API
- **XSS Protection**: HtmlSanitizer (Ganss.Xss)
- **Image Uploads**: Custom file handling with GUID naming

### Architecture
- **Pattern**: MVC (Model-View-Controller)
- **Dependency Injection**: Built-in ASP.NET Core DI
- **Razor Pages**: For Identity UI
- **View Components**: Partial views for reusable UI

---

## Installation

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [MySQL 8.0+](https://dev.mysql.com/downloads/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Google Gemini API Key](https://makersuite.google.com/app/apikey) (free tier available)

### Step 1: Clone the Repository

```bash
git clone https://github.com/dariabulacu/SocialFilmPlatform.git
cd SocialFilmPlatform
```

### Step 2: Configure Database

1. Create a MySQL database:
```sql
CREATE DATABASE socialfilmplatform CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

2. Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=socialfilmplatform;User=root;Password=YOUR_PASSWORD;"
  }
}
```

### Step 3: Configure AI (Optional)

Add your Gemini API key:

```json
{
  "Gemini": {
    "ApiKey": "YOUR_GEMINI_API_KEY"
  }
}
```

**Get API Key**: [Google AI Studio](https://makersuite.google.com/app/apikey)

### Step 4: Run Migrations

```bash
cd SocialFilmPlatform
dotnet ef database update
```

This will:
- Create all database tables
- Seed initial data (users, movies, actors, genres)

### Step 5: Run the Application

```bash
dotnet run
```

Navigate to: `https://localhost:5001`

---

## Default User Accounts

The database is seeded with test accounts:

| Role | Email | Password |
|------|-------|----------|
| **Admin** | admin@test.com | `Admin1!` |
| **Editor** | editor@test.com | `Editor1!` |
| **User** | user@test.com | `User1!` |

**Warning:** Change these passwords in production!

---

## Usage Guide

### Creating a Movie (Editor/Admin)

1. Navigate to Movies → Add Movie
2. Fill required fields (title, director, description, date, score, genre)
3. Optional: Upload poster, add trailer URL
4. Click Save Movie

### Creating a Diary (All Users)

1. Go to Diaries → Create New
2. Enter name, description, public/private toggle
3. Add movies from movie detail pages
4. Use AI Suggest for auto-generated tags

### Writing a Review

1. Go to movie detail page
2. Use rich text editor in Critiques section
3. Click Publish
4. Vote on other reviews

---

## Project Structure

```
SocialFilmPlatform/
├── Controllers/           # MVC Controllers
│   ├── MoviesController.cs
│   ├── DiariesController.cs
│   ├── ActorsController.cs
│   ├── ReviewsController.cs
│   └── ...
├── Models/               # Data Models
│   ├── Movie.cs
│   ├── Diary.cs
│   ├── Review.cs
│   ├── ApplicationUser.cs
│   └── SeedData.cs
├── Views/                # Razor Views
│   ├── Movies/
│   ├── Diaries/
│   ├── Actors/
│   └── Shared/
├── Services/             # Business Logic
│   ├── IAiService.cs
│   └── GeminiAiService.cs
├── Data/                 # EF Core Context
│   ├── ApplicationDbContext.cs
│   └── Migrations/
├── wwwroot/              # Static Files
│   ├── css/
│   ├── js/
│   └── images/
└── Program.cs            # App Entry Point
```

---

## Database Schema


### Key Relationships

- **Many-to-Many**: Movies ↔ Actors (via `ActorMovie`)
- **Many-to-Many**: Diaries ↔ Movies (via `MovieDiary`)
- **Many-to-Many**: Diaries ↔ Tags/Categories
- **One-to-Many**: Movie → Reviews, User → Diaries

---

## Security Features

### XSS Protection
```csharp
var sanitizer = new HtmlSanitizer();
movie.Description = sanitizer.Sanitize(movie.Description);
```

### Role-Based Authorization
```csharp
[Authorize(Roles = "Editor,Admin")]
public IActionResult New() { ... }
```

### CSRF Protection
- Built-in anti-forgery tokens on all forms
- `[ValidateAntiForgeryToken]` on POST actions

### SQL Injection Prevention
- Entity Framework parameterized queries
- No raw SQL execution

---

## API Endpoints

### Movie API (Internal)
- `GET /Movies/Index` - List all movies (paginated)
- `GET /Movies/Show/{id}` - Movie details
- `POST /Movies/New` - Create movie
- `POST /Movies/Edit/{id}` - Update movie
- `POST /Movies/Delete/{id}` - Delete movie

### Diary API (Internal)
- `GET /Diaries/Index` - Public diaries
- `GET /Diaries/MyDiaries` - User's diaries
- `POST /Diaries/SuggestTags` - AI suggestions (JSON response)

### Review API (Internal)
- `POST /Reviews/New` - Submit review
- `POST /Reviews/Vote` - Like/dislike review (JSON response)

---

## Testing

### Database Seeding
The application automatically seeds test data:
- 3 user roles
- 6 test users
- 8 genres
- 6 actors
- 7 movies
- 5 sample diaries
- 3 sample reviews

---


