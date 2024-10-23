using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Shaders;
using fluXis.Game.Graphics.Shaders.Glitch;
using fluXis.Game.Map.Structures.Events;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Gameplay;

public partial class ShaderEventHandler : EventHandler<ShaderEvent>
{
    private ShaderStackContainer stack { get; }
    private ShaderType[] types { get; }

    public ShaderEventHandler(List<ShaderEvent> events, ShaderStackContainer stack)
        : base(events)
    {
        types = events.Select(x => x.Type).Distinct().ToArray();
        this.stack = stack;
        Trigger = trigger;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        foreach (var shaderType in types)
        {
            var shader = stack.GetShader(shaderType) ?? throw new Exception($"Shader with type {shaderType} is not in stack!");
            AddInternal(new TransformHandler(shader));
        }
    }

    private void trigger(ShaderEvent ev)
    {
        var handler = InternalChildren.OfType<TransformHandler>().FirstOrDefault(x => x.Type == ev.Type) ?? throw new Exception($"Handler with type {ev.ShaderName} is not in scene tree!");

        // handle Start Parameters if they exist and UseStartParams is true
        if (ev.UseStartParams)
        {
            foreach (var param in ev.StartParameters)
            {
                switch (param.Value)
                {
                    case SliderParameter slider:
                        handler.SetParameterTo(param.Key, slider.Value);
                        break;
                    case CheckboxParameter checkbox:
                        handler.SetParameterTo(param.Key, checkbox.Value ? 1f : 0f); // Boolean represented as 1 or 0 for simplicity
                        break;
                }
            }
        }

        // handle End Parameters
        foreach (var param in ev.EndParameters)
        {
            if (param.Value is SliderParameter slider)
            {
                handler.SetParameterTo(param.Key, slider.Value, ev.Duration, ev.Easing);
            }
        }
    }

    // The shader stack is outside the gameplay clock.
    // We use this to keep up with rate changes and seeking
    private partial class TransformHandler : Drawable
    {
        private ShaderContainer container { get; }
        public ShaderType Type { get; }

        public TransformHandler(ShaderContainer container)
        {
            this.container = container;
            Type = container.Type;
        }

        // set parameter values dynamically using reflection
        public void SetParameterTo(string paramName, float value, double dur = 0, Easing ease = Easing.None)
        {
            var property = container.GetType().GetProperty(paramName);

            if (property != null && property.PropertyType == typeof(float))
            {
                //  set the parameter directly if no duration, otherwise, animate the transition
                if (dur == 0)
                {
                    property.SetValue(container, value);
                }
                else
                {
                    this.TransformTo(paramName, value, dur, ease);
                }
            }
        }
    }
}
