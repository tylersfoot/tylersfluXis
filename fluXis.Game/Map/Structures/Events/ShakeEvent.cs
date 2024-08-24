using fluXis.Game.Map.Structures.Bases;
using Newtonsoft.Json;

namespace fluXis.Game.Map.Structures.Events;

public class ShakeEvent : IMapEvent, IHasDuration
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("duration")]
    public double Duration { get; set; }

    [JsonProperty("magnitude")]
    public float Magnitude { get; set; } = 10;
}