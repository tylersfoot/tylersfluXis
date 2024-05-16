using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Online.API.Requests.Users;
using fluXis.Game.Online.Fluxel;
using fluXis.Shared.Components.Users;
using Humanizer;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Overlay.User.Sidebar;

public partial class ProfileFollowerList : FillFlowContainer
{
    [Resolved]
    private FluxelClient fluxel { get; set; }

    private long id { get; }

    private FluXisSpriteText countText;
    private FillFlowContainer noFollowers;
    private FillFlowContainer followers;

    public ProfileFollowerList(long id)
    {
        this.id = id;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(0, 10);

        InternalChildren = new Drawable[]
        {
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Horizontal,
                Padding = new MarginPadding { Horizontal = 10 },
                Children = new[]
                {
                    new FluXisSpriteText
                    {
                        Text = "Followers ", // yes, this space is intentional
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        WebFontSize = 24
                    },
                    countText = new FluXisSpriteText
                    {
                        Alpha = .8f,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        WebFontSize = 16
                    }
                }
            },
            noFollowers = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Alpha = 0,
                Children = new Drawable[]
                {
                    new SpriteIcon
                    {
                        Size = new Vector2(20),
                        Icon = FontAwesome6.Solid.UserGroup,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre
                    },
                    new FluXisSpriteText
                    {
                        Text = "No followers",
                        WebFontSize = 16,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre
                    }
                }
            },
            followers = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(0, 10)
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        if (id == -1)
            return;

        var req = new UserFollowersRequest(id);
        req.Success += res => SetData(res.Data);

        req.PerformAsync(fluxel);
    }

    public void SetData(List<APIUserShort> data)
    {
        if (data.Count == 0)
        {
            noFollowers.FadeIn(400);
            return;
        }

        countText.Text = data.Count.ToMetric();

        // show the first 3 as normal
        for (var i = 0; i < Math.Min(3, data.Count); i++)
            followers.Add(new FollowerEntry(data[i]));

        // if there are more than 3, show an overflow
        if (data.Count > 3)
            followers.Add(new FollowerOverflow(data.GetRange(3, data.Count - 3)));
    }

    private partial class FollowerEntry : FillFlowContainer
    {
        [CanBeNull]
        [Resolved(CanBeNull = true)]
        private FluXisGame game { get; set; }

        private APIUserShort user { get; }

        public FollowerEntry(APIUserShort user)
        {
            this.user = user;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Direction = FillDirection.Horizontal;
            Spacing = new Vector2(8);

            InternalChildren = new Drawable[]
            {
                new LoadWrapper<DrawableAvatar>
                {
                    Size = new Vector2(40),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    CornerRadius = 5,
                    Masking = true,
                    LoadContent = () => new DrawableAvatar(user)
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    }
                },
                new FluXisSpriteText
                {
                    Text = user.Username,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    WebFontSize = 16
                }
            };
        }

        protected override bool OnClick(ClickEvent e)
        {
            game?.PresentUser(user.ID);
            return game != null;
        }
    }

    private partial class FollowerOverflow : FillFlowContainer
    {
        private List<APIUserShort> users { get; }

        public FollowerOverflow(List<APIUserShort> users)
        {
            this.users = users;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Direction = FillDirection.Horizontal;
            Spacing = new Vector2(10);

            InternalChildren = new Drawable[]
            {
                new FluXisSpriteText
                {
                    Text = $"+{users.Count.ToMetric()} more",
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    WebFontSize = 14
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(-15),
                    ChildrenEnumerable = users.GetRange(0, Math.Min(5, users.Count)).Select(u => new OverflowEntry(u))
                }
            };
        }
    }

    private partial class OverflowEntry : CompositeDrawable
    {
        [CanBeNull]
        [Resolved(CanBeNull = true)]
        private FluXisGame game { get; set; }

        private APIUserShort user { get; }

        private const int outline_width = 2;

        public OverflowEntry(APIUserShort user)
        {
            this.user = user;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Size = new Vector2(30);

            InternalChildren = new Drawable[]
            {
                new Circle
                {
                    Size = new Vector2(30 + outline_width * 2),
                    Colour = FluXisColors.Background1,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                },
                new LoadWrapper<DrawableAvatar>
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    CornerRadius = 15,
                    Masking = true,
                    LoadContent = () => new DrawableAvatar(user)
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    }
                }
            };
        }

        protected override bool OnClick(ClickEvent e)
        {
            game?.PresentUser(user.ID);
            return game != null;
        }
    }
}
