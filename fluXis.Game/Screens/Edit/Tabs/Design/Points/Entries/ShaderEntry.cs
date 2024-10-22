using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Map.Structures.Events;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.List;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.Settings.Preset;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Design.Points.Entries;

public partial class ShaderEntry : PointListEntry
{
    protected override string Text => "Shader";
    protected override Colour4 Color => FluXisColors.Shader;

    private ShaderEvent shader => Object as ShaderEvent;

    private float maxStrength
    {
        get
        {
            if (shader.Type == ShaderType.Chromatic)
                return 20f;

            return 1f;
        }
    }

    private float maxBlockSize = 1f;
    private float maxColorRate = 1f;

    private float step
    {
        get
        {
            if (shader.Type == ShaderType.Chromatic)
                return 1f;

            return .01f;
        }
    }

    public ShaderEntry(ShaderEvent obj)
        : base(obj)
    {
    }

    public override ITimedObject CreateClone()
    {
        return new ShaderEvent
        {
            Time = Object.Time,
            Duration = shader.Duration,
            Type = shader.Type,
            UseStartParams = shader.UseStartParams,
            StartParameters = new ShaderEvent.ShaderParameters
            {
                Strength = shader.StartParameters.Strength,
                BlockSize = shader.StartParameters.BlockSize,
                ColorRate = shader.StartParameters.ColorRate
            },
            EndParameters = new ShaderEvent.ShaderParameters
            {
                Strength = shader.EndParameters.Strength,
                BlockSize = shader.EndParameters.BlockSize,
                ColorRate = shader.EndParameters.ColorRate
            }
        };
    }

    protected override Drawable[] CreateValueContent()
    {
        var text = $"{shader.ShaderName} {(int)shader.Duration}ms (";

        if (shader.UseStartParams)
            text += $"{shader.StartParameters.Strength} > ";

        text += $"{shader.EndParameters.Strength})";

        return new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = text,
                Colour = Color
            }
        };
    }

    protected override IEnumerable<Drawable> CreateSettings()
    {
        var startValToggle = new PointSettingsToggle
        {
            Text = "Use Start Value",
            TooltipText = "Enables whether start values should be used.",
            Bindable = new Bindable<bool>(shader.UseStartParams),
            OnStateChanged = enabled =>
            {
                shader.UseStartParams = enabled;
                Map.Update(shader);
            }
        };

        var settings = new List<Drawable>
        {
            new PointSettingsLength<ShaderEvent>(Map, shader, BeatLength),
            new PointSettingsDropdown<ShaderType>
            {
                Text = "Shader",
                TooltipText = "The shader to apply to the playfield.",
                CurrentValue = shader.Type,
                Items = Enum.GetValues<ShaderType>().ToList(),
                OnValueChanged = value =>
                {
                    shader.Type = value;
                    Map.Update(shader);
                }
            },

            startValToggle,

            new PointSettingsSlider<float>
            {
                Enabled = startValToggle.Bindable,
                Text = "Start Strength",
                TooltipText = "The strength of the shader effect.",
                CurrentValue = shader.StartParameters.Strength,
                Min = 0,
                Max = maxStrength,
                Step = step,
                OnValueChanged = value =>
                {
                    shader.StartParameters.Strength = value;
                    Map.Update(shader);
                }
            },

            new PointSettingsSlider<float>
            {
                Text = "End Strength",
                TooltipText = "The strength of the shader effect.",
                CurrentValue = shader.EndParameters.Strength,
                Min = 0,
                Max = maxStrength,
                Step = step,
                OnValueChanged = value =>
                {
                    shader.EndParameters.Strength = value;
                    Map.Update(shader);
                }
            }
        };

        // Check if the shader is of type Glitch before adding the block size and color rate sliders
        if (shader.Type == ShaderType.Glitch)
        {
            settings.AddRange(new Drawable[]
            {
                new PointSettingsSlider<float>
                {
                    Enabled = startValToggle.Bindable,
                    Text = "Start Block Size",
                    TooltipText = "The block size of the shader effect.",
                    CurrentValue = shader.StartParameters.BlockSize,
                    Min = 0,
                    Max = maxBlockSize,
                    Step = step,
                    OnValueChanged = value =>
                    {
                        shader.StartParameters.BlockSize = value;
                        Map.Update(shader);
                    }
                },

                new PointSettingsSlider<float>
                {
                    Text = "End Block Size",
                    TooltipText = "The block size of the shader effect.",
                    CurrentValue = shader.EndParameters.BlockSize,
                    Min = 0,
                    Max = maxBlockSize,
                    Step = step,
                    OnValueChanged = value =>
                    {
                        shader.EndParameters.BlockSize = value;
                        Map.Update(shader);
                    }
                },

                new PointSettingsSlider<float>
                {
                    Enabled = startValToggle.Bindable,
                    Text = "Start Color Rate",
                    TooltipText = "The color rate of the shader effect.",
                    CurrentValue = shader.StartParameters.ColorRate,
                    Min = 0,
                    Max = maxColorRate,
                    Step = step,
                    OnValueChanged = value =>
                    {
                        shader.StartParameters.ColorRate = value;
                        Map.Update(shader);
                    }
                },

                new PointSettingsSlider<float>
                {
                    Text = "End Color Rate",
                    TooltipText = "The color rate of the shader effect.",
                    CurrentValue = shader.EndParameters.ColorRate,
                    Min = 0,
                    Max = maxColorRate,
                    Step = step,
                    OnValueChanged = value =>
                    {
                        shader.EndParameters.ColorRate = value;
                        Map.Update(shader);
                    }
                }
            });
        }

        settings.Add(new PointSettingsEasing<ShaderEvent>(Map, shader));

        return base.CreateSettings().Concat(settings);
    }

}
