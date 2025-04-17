using Common.Core.DTOs.Backend;
using Common.Core.Enums;
using MediatR;

namespace Backend.Application.Features.Execution.UserAnalysisExecutionList;

public class UserAnalysisExecutionListRequest : IRequest<List<UserAnalysisExecutionDto>>
{
    public int UserId { get; set; }
    public PluginStatus? Status { get; set; }

    public UserAnalysisExecutionListRequest(int userId, PluginStatus? status)
    {
        this.UserId = userId;
        this.Status = status;
    }
}