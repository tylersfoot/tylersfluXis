using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Shaders;
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

    // private float maxStrength
    // {
    //     get
    //     {
    //         if (shader.Type == ShaderType.Chromatic)
    //             return 20f;

    //         return 1f;
    //     }
    // }

    // private float maxBlockSize = 1f;
    // private float maxColorRate = 1f;

    // private float step
    // {
    //     get
    //     {
    //         if (shader.Type == ShaderType.Chromatic)
    //             return 1f;

    //         return .01f;
    //     }
    // }

    public ShaderEntry(ShaderEvent obj) : base(obj)
    {
    }

    public override ITimedObject CreateClone()
    {
        var clone = new ShaderEvent
        {
            Time = Object.Time,
            Duration = shader.Duration,
            ShaderName = shader.ShaderName,
            UseStartParams = shader.UseStartParams
        };

        // clone Start and End parameters
        foreach (var param in shader.StartParameters)
        {
            clone.StartParameters[param.Key] = param.Value.Clone();
        }

        foreach (var param in shader.EndParameters)
        {
            clone.EndParameters[param.Key] = param.Value.Clone();
        }

        return clone;
    }

    protected override Drawable[] CreateValueContent()
    {
        var text = $"{shader.ShaderName} {(int)shader.Duration}ms (";

        // not sure how to do this esp. with mult. parameters
        // if (shader.UseStartParams)
        //     text += $"{shader.StartParameters.Strength} > ";

        // text += $"{shader.EndParameters.Strength})";

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
            new PointSettingsDropdown<string>
            {
                Text = "Shader",
                TooltipText = "The shader to apply to the playfield.",
                CurrentValue = shader.ShaderName,
                Items = ShaderSettings.Shaders.Keys.ToList(),
                OnValueChanged = value =>
                {
                    shader.ShaderName = value;
                    Map.Update(shader);
                }
            },
            startValToggle
        };

        // dynamically generate UI elements based on the shader's parameters
        foreach (var param in shader.StartParameters)
        {
            var startParameter = param.Value;

            switch (startParameter.Type)
            {
                case ShaderParameterType.Slider:
                    var startSlider = startParameter as SliderParameter;
                    var endSlider = shader.EndParameters[param.Key] as SliderParameter;

                    settings.Add(new PointSettingsSlider<float>
                    {
                        Enabled = startValToggle.Bindable,
                        Text = $"Start {startSlider.Name}",
                        TooltipText = startSlider.Tooltip,
                        CurrentValue = startSlider.Value,
                        Min = startSlider.SliderMin,
                        Max = startSlider.SliderMax,
                        Step = startSlider.SliderStep,
                        OnValueChanged = value =>
                        {
                            startSlider.Value = value;
                            Map.Update(shader);
                        }
                    });

                    settings.Add(new PointSettingsSlider<float>
                    {
                        Text = $"End {endSlider.Name}",
                        TooltipText = endSlider.Tooltip,
                        CurrentValue = endSlider.Value,
                        Min = endSlider.SliderMin,
                        Max = endSlider.SliderMax,
                        Step = endSlider.SliderStep,
                        OnValueChanged = value =>
                        {
                            endSlider.Value = value;
                            Map.Update(shader);
                        }
                    });
                    break;

                case ShaderParameterType.Checkbox:
                    var checkbox = startParameter as CheckboxParameter;

                    settings.Add(new PointSettingsToggle
                    {
                        Text = $"{checkbox.Name}",
                        TooltipText = checkbox.Tooltip,
                        Bindable = new Bindable<bool>(checkbox.Value),
                        OnStateChanged = value =>
                        {
                            checkbox.Value = value;
                            Map.Update(shader);
                        }
                    });
                    break;
            }
        }

        settings.Add(new PointSettingsEasing<ShaderEvent>(Map, shader));

        return base.CreateSettings().Concat(settings);
    }
}
