# TrailBlaze

A full stack UK trail discovery web application. Search hiking and walking trails across the UK, view them on an interactive map, get directions to the trailhead, follow the route with live GPS tracking, and leave reviews with photos.

Built with an ASP.NET Core Web API backend and a Blazor Web App frontend.

## Features

- Search trails by name, location, or difficulty
- Paginated trail listing (20 per page, Load More)
- Interactive trail maps with Mapbox, including route lines and start markers
- Turn by turn directions to the trailhead via Google Maps
- Live GPS tracking while following a trail
- Reviews and star ratings, with photo uploads via Cloudinary
- 694 UK trails in the database, 654 with real route geometry sourced from OpenStreetMap

### Planning and Design

- Project planned using FigJam for collaborative whiteboarding (user stories, RAT, data classes, tech stack)
- Development tracked using Trello kanban board with colour-coded cards by category

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core Web API (C#), .NET 8.0 |
| Frontend | Blazor Web App, .NET 8.0 (server + WebAssembly client) |
| Database | SQL Server, Entity Framework Core 8.0.8 |
| Testing | NUnit, Moq, 48 tests |
| Maps | Mapbox GL JS v2.15.0 |
| Image storage | Cloudinary |
| Trail data | OpenStreetMap (OSM API, Geofabrik) |
| Geocoding | Nominatim |

## Project Structure

```
TrailBlaze.API           ASP.NET Core Web API backend
TrailBlaze.UI             Blazor Web App frontend (server side)
TrailBlaze.UI.Client       Blazor WebAssembly client
TrailBlaze.Tests          NUnit test project
```

### API folder layout

```
Controllers                  TrailsController, ReviewsController
Data Transfer Objects        TrailDto, CreateTrailDto, ReviewDto, CreateReviewDto,
                              OverpassTrailResultDto, OverpassResponseDto,
                              OverpassElementDto, OverpassGeometryDto, WeatherDto,
                              UpdateRouteDataDto
Migrations                   EF Core migrations
Models                       Trail, Review, User
Repositories                 ITrailRepository, TrailRepository,
                              IReviewRepository, ReviewRepository
Services                     OverpassService, WeatherService,
                              CloudinaryService, GeocodingService
```

### UI folder layout

```
Components/Layout    MainLayout.razor
Components/Pages     Home.razor, TrailList.razor, TrailDetail.razor
Components/Shared    NavBar.razor, TrailCard.razor, ReviewForm.razor, ImageUpload.razor
Services             TrailService.cs, ReviewService.cs, CloudinaryService.cs
wwwroot              app.css, mapbox.js
```

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- SQL Server (a local instance works fine)
- A Mapbox access token
- A Cloudinary account with an unsigned upload preset

### Setup

1. Clone the repository:
   ```
   git clone https://github.com/FarouqAbdul93/TrailBlaze.git
   cd TrailBlaze
   ```

2. Add your connection string to `TrailBlaze.API/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=TrailBlazeDB;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

3. Add your Mapbox token and Cloudinary details to `TrailBlaze.UI/TrailBlaze.UI/appsettings.json`:
   ```json
   {
     "MapboxToken": "YOUR_MAPBOX_TOKEN",
     "Cloudinary": {
       "CloudName": "YOUR_CLOUD_NAME",
       "UploadPreset": "YOUR_UPLOAD_PRESET"
     }
   }
   ```

4. Apply the EF Core migrations:
   ```
   dotnet ef database update --project TrailBlaze.API
   ```

5. Run the API and the UI (in separate terminals):
   ```
   dotnet run --project TrailBlaze.API
   dotnet run --project TrailBlaze.UI
   ```

   - API: `https://localhost:7212` (Swagger at `/swagger`)
   - UI: `https://localhost:7239`

Neither `appsettings.json` file is committed to source control, so these steps are required on every fresh clone.

## API Endpoints

**Trails**

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/Trails?pageNumber=1&pageSize=20` | Paginated trail list |
| GET | `/api/Trails/{id}` | Single trail, including route data |
| POST | `/api/Trails` | Create a trail |
| PATCH | `/api/Trails/{id}/routedata` | Update a trail's route GeoJSON |
| GET | `/api/Trails/search?location=` | Search by name or location |
| GET | `/api/Trails/difficulty?difficulty=` | Filter by difficulty |
| GET | `/api/Trails/live-search?location=` | Live search via Overpass |
| GET | `/api/Trails/{id}/reviews` | Reviews for a trail |

**Reviews**

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/Reviews` | Create a review |
| GET | `/api/Reviews/{trailId}` | Get reviews for a trail |

## Testing

48 tests, all passing, covering repositories, services, and controller integration:

- `TrailRepositoryTests` (7)
- `ReviewRepositoryTests` (5)
- `TrailServiceTests` (5)
- `ReviewServiceTests` (6)
- `OverpassServiceTests` (5)
- `WeatherServiceTests` (6)
- `TrailsIntegrationTests` (7)
- `ReviewsIntegrationTests` (7)

Run the suite with:
```
dotnet test
```

## Trail Data

The database holds 694 trails: 10 hand seeded quality UK trails, plus 684 real UK hiking trails pulled from OpenStreetMap's Geofabrik extract. Route geometry for 654 of those trails was fetched from the official OpenStreetMap API and stored permanently in the database, so no live API calls are needed to display a route. The remaining 40 trails lack route data due to minor issues in the source OSM data.

## Notable Engineering Decisions

- **Mapbox in Blazor Server**: Blazor Server renders the page before the JavaScript can initialise the map, so the map div doesn't exist yet when the script runs. Worked around this using an HTML `onclick` attribute to trigger JavaScript directly instead of `@onclick`.
- **Image uploads**: Uploading through Blazor Server's SignalR connection caused timeouts, so uploads go straight from the browser to Cloudinary via `fetch`, bypassing Blazor entirely.
- **Trail route data**: Public Overpass API servers were unreliable (timeouts and 406 errors), so route geometry is fetched once per trail from the official OpenStreetMap API and cached in the database.
- **Pagination**: Loading all 694 trails at once was slow, so the trail list is paginated (20 per page) with a Load More button, and the list query excludes route data and reviews.

## Known Issues

- The trails list page can be slow to load due to the volume of data (694 trails). This is a known issue and optimisation is planned.
- 40 trails display a map marker but no route line due to missing geometry in the source OSM data.
- Most OSM-sourced trails show "United Kingdom" as the location rather than a specific region. Reverse geocoding to fix this is on the roadmap.

## Roadmap

- Optimise trail list load speed
- Backfill missing route data
- Add reverse geocoding for accurate trail locations

## Contributors

- [Farouq](https://github.com/FarouqAbdul93) — Frontend (Blazor)
- [Ebrahim](https://github.com/ibby1995) — Backend (ASP.NET Core Web API)

## Acknowledgements

- Trail data © OpenStreetMap contributors, licensed under ODbL
- Maps powered by Mapbox
- Images hosted on Cloudinary
- Route geometry sourced from the OpenStreetMap API
