using System.Globalization;
using fluXis.Map.Structures.Bases;
using fluXis.Utils;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Shared.Points.Settings.Preset;

public partial class PointSettingsTime : PointSettingsTextBox
{
    private EditorMap map { get; }
    private ITimedObject obj { get; }

    public PointSettingsTime(EditorMap map, ITimedObject obj)
    {
        this.map = map;
        this.obj = obj;

        Text = "Time";
        TooltipText = "The time in milliseconds when the event should trigger.";
        DefaultText = obj.Time.ToStringInvariant("0");
        OnTextChanged = box =>
        {
            if (float.TryParse(box.Text, CultureInfo.InvariantCulture, out var time))
            {
                obj.Time = time;
                map.Update(obj);
            }
            else TextBox.NotifyError();
        };
    }

    protected override Drawable CreateExtraButton() => new PointSettingsToCurrentButton
    {
        Action = t =>
        {
            TextBox.Text = t.ToStringInvariant("0");
            obj.Time = t;
            map.Update(obj);
        }
    };
}
