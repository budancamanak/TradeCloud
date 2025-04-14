using AutoMapper;
using Backend.Application.Abstraction.Repositories;
using Common.Application.Repositories;
using Common.Application.Services;
using Common.Core.DTOs.Backend;
using Common.Plugin.Abstraction;
using FluentValidation;
using MediatR;

namespace Backend.Application.Features.Execution.ListActivePlugins;

/// <summary>
/// active plugin means, it's either running or queued. ones that are not finished.
/// 
/// </summary>
/// <param name="cache"></param>
/// <param name="mapper"></param>
/// <param name="repository"></param>
public class ListActivePluginsRequestHandler(
    IValidator<ListActivePluginsRequest> validator,
    ICacheService cache,
    IMapper mapper,
    IPluginExecutionRepository repository)
    : IRequestHandler<ListActivePluginsRequest, List<PluginExecutionsDto>>
{
    public async Task<List<PluginExecutionsDto>> Handle(ListActivePluginsRequest request,
        CancellationToken cancellationToken)
    {
        /*
         * todo will load active plugins -> plugins that are in running or queued state
         * todo will try to load it from cache or database
         * todo consider cache eviction policies -> as plugins might be removed from cache sometime
         */
        // var items = await cache.GetAsync<List<PluginExecutionsDto>>(CacheKeyGenerator.ActivePluginCountKey());
        // throw new NotImplementedException();
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var items = await repository.GetActivePluginExecutions(request.AnalysisExecutionId);
        var dtos = mapper.Map<List<PluginExecutionsDto>>(items);
        return dtos;
    }
}