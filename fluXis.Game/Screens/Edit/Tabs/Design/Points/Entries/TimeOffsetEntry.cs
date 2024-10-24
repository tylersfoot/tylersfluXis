﻿using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Map.Structures.Events;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using fluXis.Game.Utils;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Design.Points.Entries;

public partial class TimeOffsetEntry : PointListEntry
{
    protected override string Text => "Time Offset";
    protected override Colour4 Color => FluXisColors.TimeOffset;

    private TimeOffsetEvent offset => Object as TimeOffsetEvent;

    public TimeOffsetEntry(TimeOffsetEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone() => new TimeOffsetEvent
    {
        Time = Object.Time,
        Duration = offset.Duration,
        UseStartValue = offset.UseStartValue,
        StartOffset = offset.StartOffset,
        TargetOffset = offset.TargetOffset,
        Easing = offset.Easing
    };

    protected override Drawable[] CreateValueContent()
    {
        var text = "";

        if (offset.UseStartValue)
            text += $"{(int)offset.StartOffset}ms > ";

        text += $"{(int)offset.TargetOffset}ms";

        return new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = $"{text} {(int)offset.Duration}ms",
                Colour = Color
            }
        };
    }

    protected override IEnumerable<Drawable> CreateSettings()
    {
        var startToggle = new PointSettingsToggle
        {
            Text = "Use Start Value",
            TooltipText = "Enables whether start values should be used.",
            Bindable = new Bindable<bool>(offset.UseStartValue),
            OnStateChanged = enabled =>
            {
                offset.UseStartValue = enabled;
                Map.Update(offset);
            }
        };

        return base.CreateSettings().Concat(new Drawable[]
        {
            new PointSettingsLength<TimeOffsetEvent>(Map, offset, BeatLength),
            startToggle,
            new PointSettingsTextBox
            {
                Enabled = startToggle.Bindable,
                Text = "Start Offset",
                TooltipText = "The visual offset at the start of the event in milliseconds.",
                DefaultText = offset.StartOffset.ToStringInvariant(),
                OnTextChanged = box =>
                {
                    if (box.Text.TryParseIntInvariant(out var result))
                        offset.StartOffset = result;
                    else
                        box.NotifyError();

                    Map.Update(offset);
                }
            },
            new PointSettingsTextBox
            {
                Text = "Target Offset",
                TooltipText = "The visual offset in milliseconds.",
                DefaultText = offset.TargetOffset.ToStringInvariant(),
                OnTextChanged = box =>
                {
                    if (box.Text.TryParseIntInvariant(out var result))
                        offset.TargetOffset = result;
                    else
                        box.NotifyError();

                    Map.Update(offset);
                }
            },
            new PointSettingsEasing<TimeOffsetEvent>(Map, offset)
        });
    }
}