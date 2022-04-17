using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using WAP_PIS.Authorization;

namespace WAP_PIS.Models;

public class AccountViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Role Role { get; set; }
}
