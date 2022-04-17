using WAP_PIS.Database;

namespace WAP_PIS.Models;

public class CheckLoginResultViewModel
{
    public bool LoggedIn { get; set; }
    public AccountViewModel? User { get; set; }
}
