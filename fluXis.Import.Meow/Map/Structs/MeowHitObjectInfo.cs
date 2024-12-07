using JetBrains.Annotations;

namespace fluXis.Import.Meow.Map.Structs;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class MeowHitObjectInfo
{
    public float StartTime { get; set; }
    public int Lane { get; set; }
    public float EndTime { get; set; }

    public bool IsLongNote => EndTime > 0;
}
