﻿namespace fluXis.Game.Screens.Edit;

public class EditorSnapProvider
{
    private EditorMap map { get; }
    private EditorSettings settings { get; }
    private EditorClock clock { get; }

    public EditorSnapProvider(EditorMap map, EditorSettings settings, EditorClock clock)
    {
        this.map = map;
        this.settings = settings;
        this.clock = clock;
    }

    public double SnapTime(double time)
    {
        var tp = map.MapInfo.GetTimingPoint(time);
        double t = tp.Time;
        double increase = tp.Signature * tp.MsPerBeat / (4 * settings.SnapDivisor);
        if (increase == 0) return time; // no snapping, the game will just freeze because it loops infinitely

        if (time < t)
        {
            while (true)
            {
                var next = t - increase;

                if (next < time)
                {
                    t = next;
                    break;
                }

                t = next;
            }
        }
        else
        {
            while (true)
            {
                var next = t + increase;
                if (next > time) break;

                t = next;
            }
        }

        if (t < 0) t = 0;
        if (t > clock.TrackLength) t = clock.TrackLength;

        return t;
    }
}
