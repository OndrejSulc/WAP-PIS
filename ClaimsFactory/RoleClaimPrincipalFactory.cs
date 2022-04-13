using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using WAP_PIS.Database;

namespace WAP_PIS.ClaimsFactory;

public class RoleClaimPrincipalFactory : UserClaimsPrincipalFactory<Account>
{
    public RoleClaimPrincipalFactory(
        UserManager<Account> userManager,
        IOptions<IdentityOptions> options
    ) : base(userManager, options)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(Account user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        switch (user)
        {
            case Manager manager:
                {
                    identity.AddClaim(new Claim("Role",
                        manager.IsCEO ? "CEO" : "Manager"
                    ));
                    return identity;
                }
            case Secretary secretary:
                {
                    identity.AddClaim(new Claim("Role",
                        "Secretary"
                    ));

                    if (secretary.Manager != null)
                    {
                        identity.AddClaim(new Claim("SecretaryFor",
                            secretary.Manager.Id
                        ));
                    }
                    return identity;
                }
            default:
                return identity;
        }
    }
}
