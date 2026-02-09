# GitHub Copilot Instructions

## Repository Overview

**TrendingGiphyBot** is a Discord bot worker service that posts trending and random GIFs from the Giphy and Klipy APIs to Discord channels on configurable schedules. The bot manages channel settings, caches GIF data, and coordinates posting behavior through multiple hosted worker services.

### Focus

These instructions apply **only to the `TrendingGiphyBotWorkerService` folder**. All other folders (`TrendingGiphyBot`, `TrendingGiphyBotCore`, `TrendingGiphyBotFunctions`, `TrendingGiphyBotModel`, `TrendingGiphyBotTests`) are legacy and should be completely ignored and excluded from development.

### Tech Stack

- **Framework**: .NET 10.0 Worker Service
- **Database**: SQLite (Entity Framework Core 10.0.2)
- **Discord Integration**: Discord.Net 3.18.0
- **HTTP Client**: IHttpClientFactory with resilience handlers
- **Dependency Injection**: Injectio 5.1.0 (attribute-based registration)
- **Logging**: Serilog with console and file sinks
- **Task Scheduling**: Cronos 0.11.1
- **Testing**: NUnit 4.3.2

### Architecture Overview

The `TrendingGiphyBotWorkerService` is organized by feature (folder-by-feature structure):

- **Program.cs**: Application entry point - configures services, database, Discord client, logging, and runs all hosted workers
- **AppConfig.cs**: Root configuration model (binds to `Tgb` section in appsettings)
- **ChannelSettings/**: Channel-level settings and Discord post tracking (GiphyPost, KlipyPost models)
- **Configuration/**: Config extension methods and exceptions
- **Database/**: Entity Framework DbContext (`TrendingGiphyBotDbContext`, migrations)
- **Discord/**: Discord socket client handlers, interactions, and command registration
- **GifPostingBehavior/**: Seeding and managing GIF posting behaviors (which GIFs to post)
- **Giphy/**: Giphy API integration, staging, caching (search, trending, random)
- **Intervals/**: Time interval configuration and seeding
- **Klipy/**: Klipy API integration, staging, caching (search, trending, random)
- **Logging/**: Serilog configuration and extensions
- **Paging/**: Pagination helpers for Discord message components
- **Results/**: Result/Maybe types for error handling
- **Utc/**: UTC offset utilities

### Key Hosted Workers (registered in Program.cs)

These run as long-lived background services:

1. `DiscordPostingWorker` - Handles scheduled GIF posting to Discord channels
2. `GiphyDataStagingWorker` - Stages Giphy search/trending data for processing
3. `GiphyCacheWorker` - Manages Giphy search and trending cache refreshes
4. `GiphyRandomCacheWorker` - Manages Giphy random GIF cache
5. `KlipyDataStagingWorker` - Stages Klipy search/trending data for processing
6. `KlipyCacheWorker` - Manages Klipy search and trending cache refreshes
7. `KlipyRandomCacheWorker` - Manages Klipy random GIF cache

### Key Configuration (appsettings.json structure)

```
Tgb:
  Giphy:
    ApiKey, BaseAddress
    Staging:
      SearchCaching: {CacheCapacity, TimeSpanBetweenRefreshes}
      TrendingCaching: {CacheCapacity, TimeSpanBetweenRefreshes}
      RandomCaching: {CacheCapacity, TimeSpanBetweenRefreshes}
      EnableRandomGifs, EnableSearchGifs, EnableTrendingGifs
      MaxRandomGifAttempts
  Klipy:
    ApiKey, BaseAddress, CustomerId
    Staging: (same structure as Giphy)
  Discord:
    Token, LogSeverity
    SocketClientHandler: {PlayingGame, GuildToRegisterCommands}
  Delayer:
    CronExpressionString
  Intervals:
    MinutesJsonArrayString, HoursJsonArrayString
  Pager:
    MaxCacheLoops
```

### Database

- **Location**: `app.db` (SQLite, created in application working directory)
- **Connection**: `Data Source={currentDirectory}/app.db`
- **Migrations**: Located in `Migrations/` folder, applied automatically on startup
- **Initialization**: `TrendingGiphyBotDbContext` seeded with `GifPostingBehavior` and `Interval` data on startup

### Startup Sequence (see Program.cs)

1. Load configuration from `appsettings.json` and optional `appsettings.Development.json`
2. Configure Discord socket client with required gateway intents
3. Setup Serilog logging to console and `logs/log.log` (daily rolling)
4. Register all services using Injectio attributes (`[RegisterSingleton]`, `[RegisterTransient]`, `[RegisterScoped]`)
5. Create HTTP clients for Giphy and Klipy APIs with resilience handlers
6. Attach Discord event handlers (interactions, button clicks, guild join/leave, ready event, etc.)
7. **Initialization phase** (before running workers):
   - Seed gifPosting behaviors and intervals
   - Refresh all trending and random caches
   - Refresh Giphy and Klipy data staging
   - Login to Discord and start socket client
8. Run all hosted workers concurrently using Host.RunAsync()

## C# Files

### Variable Declaration and Naming
- Always use `var` for variable declarations
- Typically name variables after their type: `var person = new Person();`, `var thingDoer = new ThingDoer();`
- Primary constructor parameters should be prepended with underscores as if they were fields: `IService _service`

### Code Organization
- Alphabetize groups of members (fields, methods, properties, etc.)
- Keep method overload parameter orders consistent across overloads
- Group instance, async, and static calls separately
- Group calls that return variables and side effects separately
- Keep naming of all tokens consistent between types with consistent structures

### Syntax and Style
- Always prefer the latest language syntax like primary constructors and pattern matching
- Parameter lists containing 3 or more items should be wrapped with parenthesis following Allman style (as if they were curly braces); otherwise, keep on single line
- When bodies are single line, prefer expression bodies, where the arrow `=>` dangles off of the first line and the expression follows on the next line
- Control statements (if/else/for/foreach/do/while/return/throw) should always be preceded by a newline unless they are the first line of the body

### Dependency Injection and Design Patterns
- Do not use private or static methods because they break dependency injection and mocking in unit tests
- All typical interface:implementation dependencies should use Injectio's attributes (like `[RegisterSingleton]`, `[RegisterTransient]`, `[RegisterScoped]`)
- Always use SLAP (Single Layer of Abstraction Principle)
- Always follow Single Responsibility Principle, even if you end up with many types containing only 1 method each
- Typically name classes as "ThingDoer" where the method name is "DoThing" (substitute reasonable nouns and actions)

### Project Structure
- Always use folder by feature organization
- Never use periods in file names (except for the extension)

### Quality Assurance
- Always build the code, run all unit tests, and fix any errors when iterating in agent mode

## JSON Files

### Configuration Organization
- Alphabetize all config options and all their ancestors, too
- Maintain consistent nesting and naming conventions across configuration files

## Build and Test Instructions

### Build
```powershell
cd TrendingGiphyBotWorkerService
dotnet build
```
- Expected: Builds successfully with no errors
- The build generates dependencies from NuGet via Injectio source generation

### Test
```powershell
cd TrendingGiphyBotWorkerServiceTests
dotnet test
```
- Uses NUnit test framework
- All tests should pass before committing changes

### Run
```powershell
cd TrendingGiphyBotWorkerService
dotnet run
```
- Creates `app.db` (SQLite database) in the working directory
- Logs output to console and `logs/log.log`
- Service runs indefinitely until stopped (Ctrl+C)

### Entity Framework Migrations
```powershell
cd TrendingGiphyBotWorkerService
dotnet ef migrations add MigrationName
dotnet ef database update
```
- Migrations are auto-applied on application startup
- Use Design-Time DbContext Factory if issues occur

## Important Notes

- **Trust these instructions**: Use grep, find, and semantic search tools only if information appears incomplete or incorrect
- **Injectio integration**: All service registration uses `[RegisterSingleton]`, `[RegisterTransient]`, or `[RegisterScoped]` attributes - never manually add to DI container
- **No private/static methods**: Enforced for testability and dependency injection compliance
- **Configuration is data-driven**: Posting behavior, intervals, and cache settings are fully configurable via `appsettings.json`
- **Async-first**: Most operations are async (cache refreshes, Discord interactions, database queries)
- **Error handling**: Use `Maybe<T>` and `Result<T>` types for null-safe and error-safe operations
