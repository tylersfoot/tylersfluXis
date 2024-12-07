using System.Collections.Generic;
using System.IO;
using fluXis.Game.Database.Maps;

namespace fluXis.Import.Meow.Map;

public class MeowRealmMapSet : RealmMapSet
{
    public string FolderPath { get; init; } = string.Empty;

    public MeowRealmMapSet(List<RealmMap> maps)
        : base(maps)
    {
    }

    public override string GetPathForFile(string filename)
        => string.IsNullOrEmpty(filename) ? FolderPath : Path.Combine(FolderPath, filename);
}
