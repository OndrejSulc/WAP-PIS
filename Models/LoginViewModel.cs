using System.Text.Json.Serialization;
using WAP_PIS.Authorization;

namespace WAP_PIS.Models;

public class LoginViewModel
{
    public string? Login { get; set; }
    public string? Password { get; set; }
    public bool? Successful_Authentication { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Role? Role { get; set; }
}
