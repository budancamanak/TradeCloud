[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://docs.abblix.com/docs/technical-requirements)
[![language](https://img.shields.io/badge/language-C%23-239120)](https://learn.microsoft.com/ru-ru/dotnet/csharp/tour-of-csharp/overview)
[![OS](https://img.shields.io/badge/OS-linux%2C%20windows-0078D4)](https://docs.abblix.com/docs/technical-requirements)
[![CPU](https://img.shields.io/badge/CPU-x86%2C%20x64-FF8C00)](https://docs.abblix.com/docs/technical-requirements)

````shell
dotnet ef database update -p .\Backend.Infrastructure.csproj -s ..\Backend.API\Backend.API.csproj
dotnet ef migrations add {MigrationName} -p .\Backend.Infrastructure.csproj -s ..\Backend.API\Backend.API.csproj
````

## 🚀 Features

**Backend.API** is an API Server built to provide features for clients to access the system.
Features this API contains are as such:
- `List Available Plugins`: So that clients can choose one from available plugins to execute.
- `Manage Plugin Executions`: API will manage executions that were requested by user to run.
Will maintain an active plugin queue`(FIFO)` to be run.
Will display executions that are in waiting to be triggered in the queue.
Will allow users to modify a plugin parameters or cancel.
- `Manage User TrackList`: API will manage tickers that users marked as tracked. 
- `List Available Tickers`: So that clients can choose a ticker to track or run plugin on.
- `Manage System Settings`: API will maintain system settings to be used in whole application.
- `API Gateway`: API will behave as a gateway to distribute calls to appropriate services.
- `GraphQL`: API will maintain `GraphQL` functionalities for aggregator services.
- `LoadBalancer`: API will have a load balancer in front to distribute the workload evenly. 
- `Kubernetes`: API's orchestration will be handled by Kubernetes services.

| Tech            | Reason                                                                                                                                                                                   |
|:----------------|:-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `GRPC`          | Ask data from `AuthService`<br/> Ask data from worker service(Available plugins).<br/> Ask data from market service(Available tickers) <br/> Ask worker for remaining plugin run slot    |
| `RabbitMQ`      | Will trigger Worker service to run a plugin. <br/> Will listen for plugin status for status update/next plugin run                                                                       |
| `MediatR`       | Will be used to direct requests to internal handlers.                                                                                                                                    |
| `Redis`         | Will use redis to get plugin & tickers.                                                                                                                                                  |

<br>

#### **GRPC** *(Clients)*
- **List Available Tickers**: Will access to `Market Service` & retrieve available tickers
- **List Available Plugins**: Will access to `Worker Service` & retrieve available plugins
- **Available Plugin Slot**: Will access to `Worker Service` & retrieve plugin run slot
- **Login & Logout**: Will access to `Auth Service` to execute login, logout & token refresh
- **Authentication Check**: Will access to `Auth Service` to check authentications for requests.
- **Authorization Check**: Will access to `Auth Service` to check user permissions for requests.

#### **RabbitMQ** *(Producers)*
- **Next Plugin from Queue**: Will trigger event for next waiting plugin execution to `Worker Service`
- **Ticker Update**: Will trigger event to update ticker database to `Market Service`
- **Plugin Status Update**: Will trigger event to notify other services(`Notifier Service/`)

#### **RabbitMQ** *(Consumers)*
- **Plugin Status Update**: Will listen for these events `(from Worker Service)` to update plugin execution status.
These events will be consumed by `Notification Service` as well.
  - `PluginStartedEvent`
  - `PluginCancelledEvent`
  - `PluginFailedEvent`: 
  - `PluginOutputEvent`: Will listen for this event `(from Worker Service)`. It will contain plugin result. Will store plugin results to database.
  - `PluginFinishedEvent` : Will listen for this event `(from Worker Service)` to start next waiting plugin execution.
  - `PluginProgressEvent` : Will listen for this event `(from Worker Service)` to update plugin execution progress status. 
  - `NextPluginStartRequestedEvent`: Will listen for this handler to start next plugin execution. Will be called within
`Backend Service`. So it's not a `RabbitMQ Consumer`, but a `MediatR handler`.

## Internals

#### Manage User Track List
- **List users' track list**: Might need to access market service to retrieve ticker information if not found in cache.
So it's better to implement a track list service to handle the job. Track list service will be triggered by `MediatR` 
and will need `GRPC` client to access to `Market Service`. Track list will then populate the data with ticker names before returning to user.
- **Remove ticker from track list**: Since database is within this service, `MediatR` with repository pattern would be sufficient.
- **Add ticker to track list**: Since database is within this service, `MediatR` with repository pattern would be sufficient.

#### Manage Plugin Executions
Almost all items below need `GRPC` client to do authentication & authorization checks.
- **List active plugin queue**: Will access to database to retrieve waiting list. Might need to use cache with no eviction policy as well.
- **List available plugins**: Will fetch available plugins to run. Workflow could be same as `List users' track list`.
- **Create plugin execution**: Will save the plugin & fire `PluginStartEvent` to trigger the engine.
- **Cancel plugin execution**: Will update db. Will remove plugin from queue if not started.
Otherwise will fire `PluginCancelRequestedEvent`. `Worker Service` will catch this event and stop the plugin.
    - Will update the state as `CancelRequested.` if it's on the queue, will then update the state as `Cancelled`
    - if plugin is already in `Working` state, `Worker Service` will fire `PluginCancelledEvent`
- **Next plugin execution**: Whenever a plugin finishes, `Backend Service` will use `MediatR` to start next plugin.
`PluginExecutionService` with `MediatR`, `Repository` and `RedisCache` might be necessary.
- **Update plugin execution state**: Will be handled in consumers. Will need repository to access database. 
Events are described above.
- **List plugin execution history**: Will be REST API. Will use `MediatR` & `Repository` to access database.
Plugin run details won't be displayed. Plugin info, start&end dates, result: success: with output count, error: with error message.
- **Display plugin execution details**: Will be REST API. Will use `MediatR` & `Repository` to access database.
Plugin info and details will be displayed. `PluginOutputs` are also included in details.
- **Generate plugin execution param set**: Will generate param set array for a plugin to run. 
When a range for a parameter supplied, service will generate an array of parameter to run the plugin on each of them.
- ***Execution Engine***: Will need an engine waiting for plugin executions to be saved and consume the queue. 
Engine could listen for `RabbitMQ` to start the next plugin.

### Development Roadmap
#### Backend.Domain
- Models
  - [x] UserTrackList
  - [x] PluginExecution
  - [x] PluginOutput
- DTOs
  - [x] UserTrackListDTO
  - [x] PluginExecutionDTO
  - [x] PluginOutputDTO

### **Backend.Application**
###### Abstraction
- Repositories
  - [x] Design PluginExecutionRepository
  - [x] Design TrackListRepository
  - [x] Design PluginOutputRepository
- Services
  - [ ] Design PluginExecutionEngine: *Maintains active plugin queue. Starts next plugin. Uses Repository, `RabbitMQ`*
  - [ ] Design PluginExecutionService: *Generates param set array for plugin executions*
  - [ ] Design PluginService : *Retrieves available plugins. Uses `RedisCache` & `GRPC Client`*
  - [ ] Design TickerService:  *Retrieves available tickers. Uses `RedisCache` & `GRPC Client`*
###### Behaviours
  - [x] Design ValidationBehaviour: *Could be same of the one in `MarketService.Behaviours.ValidationBehaviour`
  - [x] Design LoggingBehaviour: *Could be same of the one in `MarketService.Behaviours.LoggingBehaviour`
###### Exceptions
  - [ ] Design Exceptions
###### Features
  - [x] List Available Plugins
  - [x] List Available Tickers
  - User Track List Operations
    - [x] Add Ticker to UserTrackList
    - [x] Remove Ticker from UserTrackList
    - [x] List UserTrackList
  - Plugin Execution Operations
    - [x] Create Plugin Execution
    - [x] Cancel Plugin Execution
    - [x] Next Plugin Execution
    - [x] Update Plugin State
  - Plugin Execution Details
    - [x] List Plugin Execution History
    - [x] Get Plugin Execution Details
###### Mappers
  - [x] Design Model2DTO Mappers
  - [x] Design Request Mappers: *API Requests, GRPC Requests, RabbitMQ Events*
###### Services
  - [x] Implement Cache Builders
###### Validators
  - [x] Implement Model Validators
  - [x] Implement Request Validators
###### Dependency Injection
  - [x] Register Services

### **Backend.Infrastructure**
###### Data
- [x] Design Data Model Configurations
- [x] Implement Database Context
###### Migrations
- [x] Database Migrations
###### Messaging
- [x] Implement Consumers
###### Repositories
- [x] Implement Repositories
###### Services
- [x] Implement Services
###### Dependency Injection
- [x] Register Services
 
