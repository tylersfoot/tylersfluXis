using System.IO;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;

namespace fluXis.Import.Meow.Map;

public class MeowRealmMap : RealmMap
{
    public override MapInfo GetMapInfo()
    {
        var path = MapSet.GetPathForFile(FileName);

        if (!File.Exists(path))
            return null;

        string yaml = File.ReadAllText(path);
        return MeowImport.ParseFromYaml(yaml).ToMapInfo();
    }
}
