using System.Collections.Generic;
using fluXis.Game.Map.Structures;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace fluXis.Game.Screens.Edit;

public class EditorClipboardContent
{
    public IList<HitObject> HitObjects { get; init; }

    public override string ToString() => JsonConvert.SerializeObject(this);

    [CanBeNull]
    public static EditorClipboardContent Deserialize(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<EditorClipboardContent>(json);
        }
        catch
        {
            return null;
        }
    }
}
