using JetBrains.Annotations;

namespace fluXis.Import.Meow.Map.Structs;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class MeowTimingPointInfo
{
    public float StartTime { get; set; }
    public float Bpm { get; set; }
    public int TimeSignature { get; set; }
    public bool Hidden { get; set; }
}
