using System.Security.Claims;
using Flyio.Demo.Module.SharedKernel.Infra;
using Microsoft.AspNetCore.Http;

namespace Flyio.Demo.Todos.Infra;

public class TenantGetter : ITenantGetter
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantGetter(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetCurrentTenant()
    {
        var user = _httpContextAccessor.HttpContext!.User;

        return user.FindFirstValue("tenant")!;
    }
}