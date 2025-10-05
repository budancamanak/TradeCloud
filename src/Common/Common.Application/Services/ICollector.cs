using Common.Core.Models;

namespace Common.Application.Services;

public interface ICollector
{
    void Collect(IntegrationEvent model);
}