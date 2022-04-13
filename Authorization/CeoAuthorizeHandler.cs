using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WAP_PIS.Database;

namespace WAP_PIS.Authorization;

public class CeoAuthorizeHandler : AuthorizationHandler<CeoAuthorizeHandler.CeoRequirement>
{
    private readonly ApplicationDbContext dbContext;
    private readonly UserManager<Account> userManager;
    public CeoAuthorizeHandler(ApplicationDbContext dbContext, UserManager<Account> userManager)
    {
        this.dbContext = dbContext;
        this.userManager = userManager;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CeoRequirement requirement)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = userManager.Users.SingleOrDefault(a => a.Id == userId);
        if(user == null) return Task.CompletedTask;

        switch (user)
        {
            case Secretary s:
                if (s.Manager.IsCEO)
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
                else
                {
                    return Task.CompletedTask;
                }
                break;
            case Manager m:
                if (m.IsCEO)
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
                else
                {
                    return Task.CompletedTask;
                }
                break;
            default:
                return Task.CompletedTask;
        }
    }
    public class CeoRequirement : IAuthorizationRequirement
    {
    }
}
