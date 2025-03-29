using System.Text.Json.Serialization;
using Agency.Service.Entities;

namespace Agency.Service.Holidays;

public class HolidayResponse
{
    [JsonPropertyName("holidays")]
    public List<Holiday> Holidays { get; set; } = new();
}