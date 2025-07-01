[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://docs.abblix.com/docs/technical-requirements)
[![language](https://img.shields.io/badge/language-C%23-239120)](https://learn.microsoft.com/ru-ru/dotnet/csharp/tour-of-csharp/overview)
[![OS](https://img.shields.io/badge/OS-linux%2C%20windows-0078D4)](https://docs.abblix.com/docs/technical-requirements)
[![CPU](https://img.shields.io/badge/CPU-x86%2C%20x64-FF8C00)](https://docs.abblix.com/docs/technical-requirements)

## 🚀 Features

**TradeCloud** is a project that will allow users to run analysis on stock or cryptocurrencies prices.
Features the project are as such:
- `User TrackList`: So that users will have preferred tickers/symbols to track.
- `Analysis Executions`: So that users can create analysis executions, run/cancel/stop them. 
Each plugin might have output signals that can be drawn on a chart with price information.
Have base classes to be used to develop own algorithms.
Based on supplied parameter range sets, project will run each possible analysis combination in sequence.
- `Market`: Project will fetch necessary prices before running the analysis.



| Name       | Reason                                                      |
|:-----------|:------------------------------------------------------------|
| `GRPC`     | Used for inter-service request-response style communication |
| `RabbitMQ` | Used for inter-service communication                        |
| `MediatR`  | Used to direct requests to internal handlers.               |
| `Redis`    | Used to get plugin & tickers.                               |
| `Hangfire` | Used to schedule analysis.                                  |


### Development Roadmap
- [ ] Backend
  - [x] v1: Manage user track list, analysis & plugin execution/cancellation
- [ ] Worker
  - [x] v1: Execute analysis, cached math
- [x] Security
  - [x] v1: Handle authentication/authorization and permission checks
- [ ] Market
  - [x] v1: Handle ticker ops, fetch price info.
- [ ] Notifications
  - [ ] v1: Web sockets will be used for notifications
- [x] Web UI - React web app
  - [x] Ability to create analysis with parameter sets
  - [x] Display analysis outputs on candlestick chart with labels.

### License
MIT License has been chosen for the project at the moment. Feel free to contribute. 🚀
