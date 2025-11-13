namespace LastManagement.Application.Features.Authentication.DTOs
{
    public sealed record CreateAdminRequest(
        string Username,
        string Password,
        string FullName
    );
}
