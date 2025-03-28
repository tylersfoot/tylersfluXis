using System;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Screens.Gameplay.Audio;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Gameplay.HUD.Components;

public partial class Progressbar : GameplayHUDComponent
{
    [Resolved]
    private GameplayClock clock { get; set; }

    private Bar bar;
    private FluXisSpriteText currentTimeText;
    private FluXisSpriteText percentText;
    private FluXisSpriteText timeLeftText;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Padding = new MarginPadding(20);

        InternalChildren = new Drawable[]
        {
            bar = new Bar { Progressbar = this },
            currentTimeText = new FluXisSpriteText
            {
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                Y = 18,
                X = 10,
                FontSize = 32
            },
            percentText = new FluXisSpriteText
            {
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Y = 18,
                FontSize = 32
            },
            timeLeftText = new FluXisSpriteText
            {
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Y = 18,
                X = -10,
                FontSize = 32
            }
        };
    }

    protected override void Update()
    {
        base.Update();

        int timeLeft = (int)((Screen.Map.EndTime - clock.CurrentTime) / Screen.Rate);
        int totalTime = (int)((Screen.Map.EndTime - Screen.Map.StartTime) / Screen.Rate);

        int currentTime = (int)((clock.CurrentTime - Screen.Map.StartTime) / Screen.Rate);
        int catchupTime = (int)((Screen.RulesetContainer.Time.Current - Screen.Map.StartTime) / Screen.Rate);

        float percent = (float)currentTime / totalTime;
        float catchupPercent = (float)catchupTime / totalTime;

        bar.Progress = Math.Clamp(percent, 0, 1);
        bar.CatchUpProgress = Math.Clamp(catchupPercent, 0, 1);

        percentText.Text = $"{(int)Math.Clamp(percent * 100, 0, 100)}%";
        currentTimeText.Text = TimeUtils.Format(currentTime, false);
        timeLeftText.Text = TimeUtils.Format(timeLeft, false);
    }

    private partial class Bar : CircularContainer
    {
        public float Progress
        {
            set
            {
                if (value < 0 || !float.IsFinite(value)) value = 0;
                if (value > 1) value = 1;

                bar.ResizeWidthTo(value, 200);
            }
        }

        public float CatchUpProgress
        {
            set
            {
                if (value < 0 || !float.IsFinite(value)) value = 0;
                if (value > 1) value = 1;

                catchup.ResizeWidthTo(value, 200);

                var delta = Math.Abs(bar.Width - value);
                catchup.FadeTo(delta > 0.004f ? .4f : 0, 200);
            }
        }

        [Resolved]
        private GameplayClock clock { get; set; }

        public Progressbar Progressbar { get; init; }

        private Circle bar;
        private Circle catchup;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            Height = 10;
            Masking = true;
            Colour = FluXisColors.Text;

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = .2f,
                    Blending = BlendingParameters.Additive
                },
                bar = new Circle
                {
                    RelativeSizeAxes = Axes.Both
                },
                catchup = new Circle
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Highlight
                }
            };
        }

        protected override bool OnClick(ClickEvent e)
        {
            var progress = e.MousePosition.X / DrawWidth;
            if (progress < 0) progress = 0;
            if (progress > 1) progress = 1;

            var startTime = Progressbar.Screen.Map.StartTime;
            var endTime = Progressbar.Screen.Map.EndTime;
            double previousTime = clock.CurrentTime;
            double newTime = startTime + (endTime - startTime) * progress;

            Progressbar.Screen.OnSeek?.Invoke(previousTime, newTime);

            return true;
        }
    }
}
