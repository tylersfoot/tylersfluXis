using System.IO;
using System.Linq;
using fluXis.Game.Plugins;
using osu.Framework.Platform;

namespace fluXis.Import.Meow;

public class MeowPluginConfig : PluginConfigManager<MeowPluginSetting>
{
    protected override string ID => "meow";

    public MeowPluginConfig(Storage storage)
        : base(storage)
    {
    }

    protected override void InitialiseDefaults()
    {
        SetDefault(MeowPluginSetting.GameLocation, getLocation());
    }

    private string getLocation()
    {
        const string c_path = @"C:\Program Files (x86)\Steam\steamapps\common\Meow\Meow.exe";
        var installPath = "";

        if (File.Exists(c_path)) installPath = c_path;
        else
        {
            string[] drives = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
                              .Select(c => $@"{c}:\")
                              .Where(Directory.Exists)
                              .ToArray();

            const string steam_lib_path = @"SteamLibrary\steamapps\common\Meow\Meow.exe";
            const string steam_path = @"Steam\steamapps\common\Meow\Meow.exe";

            foreach (var drive in drives)
            {
                if (File.Exists($"{drive}{steam_lib_path}"))
                {
                    installPath = $"{drive}{steam_lib_path}";
                    break;
                }

                if (File.Exists($"{drive}{steam_path}"))
                {
                    installPath = $"{drive}{steam_path}";
                    break;
                }
            }
        }

        if (string.IsNullOrEmpty(installPath))
            return "";

        return Path.GetDirectoryName(installPath);
    }
}

public enum MeowPluginSetting
{
    GameLocation
}
