﻿using System.Threading.Tasks;

namespace fluXis.Game.Online.Spectator;

public interface ISpectatorClient
{
    Task StartedPlaying(long id, SpectatorState state);
    Task StoppedPlaying(long id, SpectatorState state);
    Task RecieveFrameBundle(long id, SpectatorState state);
    Task ScoreProcessed(long id, SpectatorState state);
}
