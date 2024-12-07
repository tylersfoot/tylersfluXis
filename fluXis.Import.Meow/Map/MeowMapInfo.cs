using fluXis.Game.Map;
using Newtonsoft.Json;

namespace fluXis.Import.Meow.Map;

public class MeowMapInfo : MapInfo
{
    [JsonIgnore]
    public new MeowMap Map { get; init; }

    public MeowMapInfo(MapMetadata metadata)
        : base(metadata)
    {
    }

    public override T GetMapEvents<T>() => Map.GetEffects() as T;
}
