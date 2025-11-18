using LastManagement.Application.Features.LastNames.DTOs;
using LastManagement.Domain.LastNames.Entities;
using Mapster;

namespace LastManagement.Api.Features.LastNames;

public static class LastNamesMapping
{
    public static void ConfigureMappings()
    {
        // Entity -> DTO
        TypeAdapterConfig<LastName, LastNameDto>
            .NewConfig()
            .Map(dest => dest.Id, src => src.LastId)
            .Map(dest => dest.Status, src => src.Status.ToString());
    }
}