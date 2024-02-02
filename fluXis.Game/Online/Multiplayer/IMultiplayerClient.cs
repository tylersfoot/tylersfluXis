using System.Threading.Tasks;
using fluXis.Game.Online.API.Models.Multi;
using fluXis.Game.Online.API.Models.Users;

namespace fluXis.Game.Online.Multiplayer;

public interface IMultiplayerClient
{
    Task UserJoined(APIUserShort user);

    Task UserLeft(long id);

    Task SettingsChanged(MultiplayerRoom room);

    Task ReadyStateChanged(long userId, bool isReady);

    Task Starting();
}
