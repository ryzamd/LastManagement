using LastManagement.Application.Features.Authentication.DTOs;
using LastManagement.Domain.Accounts;
using Mapster;

namespace LastManagement.Api.Features.Authentication;

public static class AuthenticationMapping
{
    public static void ConfigureMappings()
    {
        // Account -> UserDto
        TypeAdapterConfig<Account, UserDto>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Username, src => src.Username)
            .Map(dest => dest.FullName, src => src.FullName)
            .Map(dest => dest.Role, src => src.Role.ToString())
            .Map(dest => dest.LastLoginAt, src => src.LastLoginAt)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt);
    }
}