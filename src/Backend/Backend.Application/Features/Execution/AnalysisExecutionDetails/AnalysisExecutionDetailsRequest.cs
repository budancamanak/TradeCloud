using Backend.Domain.Entities;
using Common.Core.DTOs.Backend;
using MediatR;

namespace Backend.Application.Features.Execution.AnalysisExecutionDetails;

public class AnalysisExecutionDetailsRequest : IRequest<AnalysisExecutionDto>
{
    public int AnalysisExecutionId { get; set; }
}