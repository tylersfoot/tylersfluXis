using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using fluXis.Game.Database.Maps;
using fluXis.Game.Import;
using fluXis.Game.Overlay.Notifications;
using fluXis.Import.Meow.Map;
using JetBrains.Annotations;
using osu.Framework.Bindables;
using osu.Framework.Logging;

namespace fluXis.Import.Meow;

[UsedImplicitly]
public class MeowImport : MapImporter
{
    public override string[] FileExtensions => new[] { ".qp" };
    public override string GameName => "Meow";
    public override bool SupportsAutoImport => true;
    public override string Color => "#0cb2d8";

    private Bindable<string> MeowPath { get; }
    private string songsPath => string.IsNullOrEmpty(MeowPath.Value) ? "" : Path.Combine(MeowPath.Value, "Songs");

    public MeowImport(MeowPluginConfig config)
    {
        MeowPath = config.GetBindable<string>(MeowPluginSetting.GameLocation);
    }

    public override void Import(string path)
    {
        if (!File.Exists(path))
            return;

        var notification = CreateNotification();

        try
        {
            var fileName = Path.GetFileNameWithoutExtension(path);
            var folder = CreateTempFolder(fileName);

            var qp = ZipFile.OpenRead(path);

            qp.Dispose();

            var pack = CreatePackage(fileName, folder);
            FinalizeConversion(pack, notification);
            CleanUp(folder);
        }
        catch (Exception e)
        {
            notification.State = LoadingState.Failed;
            Logger.Error(e, "Error while importing Meow map");
        }
    }

    public override List<RealmMapSet> GetMaps()
    {
        if (string.IsNullOrEmpty(MeowPath.Value))
            return base.GetMaps();

        string dbPath = Path.Combine(MeowPath.Value, "Meow.db");

        if (!File.Exists(dbPath))
        {
            Logger.Log($"Could not find Meow database at {dbPath}");
            return base.GetMaps();
        }

        List<RealmMapSet> mapSets = new();

        try
        {
            Dictionary<string, List<RealmMap>> maps = new();

            var resources = GetResourceProvider(songsPath);

            foreach (var (directoryName, mapSetMaps) in maps)
            {
                var mapSetRealm = new MeowRealmMapSet(mapSetMaps)
                {
                    FolderPath = Path.Combine(songsPath, directoryName),
                    OnlineID = 0,
                    Cover = "",
                    Resources = resources
                };

                foreach (var map in mapSetMaps)
                    map.MapSet = mapSetRealm;

                mapSets.Add(mapSetRealm);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e, "Error while reading Meow database");
            return base.GetMaps();
        }

        return mapSets;
    }
}
