[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://docs.abblix.com/docs/technical-requirements)
[![language](https://img.shields.io/badge/language-C%23-239120)](https://learn.microsoft.com/ru-ru/dotnet/csharp/tour-of-csharp/overview)
[![OS](https://img.shields.io/badge/OS-linux%2C%20windows-0078D4)](https://docs.abblix.com/docs/technical-requirements)
[![CPU](https://img.shields.io/badge/CPU-x86%2C%20x64-FF8C00)](https://docs.abblix.com/docs/technical-requirements)

## 🚀 Features

**TradeCloud** is a project that will allow users to run analysis on stock or cryptocurrencies prices.
Features the project are as such:
- `User TrackList`: So that users will have preferred tickers/symbols to track.
- `Plugin Executions`: So that users can create plugin executions, run/cancel/stop them. 
Each plugin might have output signals that can be drawn on a chart with price information. 
- `Notifications`: So that we can inform users about events such as plugin finish, signal generations etc.
Currently, websocket signals & email notifications are considered.
- `Trading`: So that users might bind their exchange APIs to automate position openings and closing based on generated signals.
- `Market`: Project will fetch necessary prices before running the analysis.

| Tech            | Reason                                                              |
|:----------------|:--------------------------------------------------------------------|
| `GRPC`          | Will be used for inter-service request-response style communication |
| `RabbitMQ`      | Will be used for inter-service communication                        |
| `MediatR`       | Will be used to direct requests to internal handlers.               |
| `Redis`         | Will use redis to get plugin & tickers.                             |
| `API Gateway`   | `Ocelot` will be used.                                              |
| `GraphQL`       | Not decided yet.                                                    |
| `Load Balancer` | Nginx will be used till Kubernetes is done.                         |
| `Kubernetes`    | Not decided yet.                                                    |


### Development Roadmap
- [ ] Backend
- [ ] Worker
- [ ] Security
- [ ] Market
- [ ] Notifications
- [ ] Web UI

### License
MIT License has been chosen for the project at the moment. Feel free to contribute. 🚀