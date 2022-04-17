using System.Diagnostics;
using WAP_PIS.Database;

namespace WAP_PIS.Authorization;

public enum Role
{
    Manager, Ceo, Secretary, CeoSecretary
}
public static class RoleExtension
{
    public static Role GetRole(this Account account)
    {
        return account switch
        {
            Manager m => m.IsCEO ? Role.Ceo : Role.Manager,
            Secretary s => s.Manager.IsCEO ? Role.CeoSecretary : Role.Secretary,
            _ => throw new ArgumentException("Role for this account type is not implemented")
        };
    }
}
