using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Multi;

[JsonObject(MemberSerialization.OptIn)]
public interface IMultiplayerRoomSettings
{
    [JsonProperty("name")]
    string Name { get; set; }

    [JsonProperty("password")]
    bool HasPassword { get; }
}