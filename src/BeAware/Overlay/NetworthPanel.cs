namespace BeAware.Overlay;

using System.Collections.Generic;
using System.Linq;

using BeAware.Helpers;
using BeAware.MenuManager.Overlay;

using Divine.Entity;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.EventArgs;
using Divine.Extensions;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Renderer;
using Divine.Update;

internal class NetworthPanel
{
    private readonly NetworthPanelMenu NetworthPanelMenu;

    private readonly PanelMove PanelMove;

    private readonly Hero LocalHero = EntityManager.LocalHero;

    private readonly HashSet<Item> Items = new();

    private readonly Dictionary<Hero, int> Heroes = new();

    public NetworthPanel(Common common)
    {
        RendererManager.LoadImageFromAssembly("BeAware.Resources.Textures.item_panel.png");

        NetworthPanelMenu = common.MenuConfig.OverlayMenu.NetworthPanelMenu;

        PanelMove = new PanelMove(new Vector2(NetworthPanelMenu.PositionXItem, NetworthPanelMenu.PositionYItem));

        NetworthPanelMenu.EnableItem.ValueChanged += OnEnableValueChanged;
        NetworthPanelMenu.MoveItem.ValueChanged += OnMoveValueChanged;

        PanelMove.ValueChanged += PanelMoveChanged;
    }

    public void Dispose()
    {
        NetworthPanelMenu.EnableItem.ValueChanged -= OnEnableValueChanged;
        NetworthPanelMenu.MoveItem.ValueChanged -= OnMoveValueChanged;

        PanelMove.ValueChanged -= PanelMoveChanged;

        if (NetworthPanelMenu.EnableItem.Value)
        {
            EntityManager.EntityAdded -= OnEntityAdded;
            EntityManager.EntityRemoved -= OnEntityRemoved;
            RendererManager.Draw -= OnDraw;
        }
    }

    private void OnEnableValueChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
    {
        if (e.Value)
        {
            EntityManager.EntityAdded += OnEntityAdded;
            EntityManager.EntityRemoved += OnEntityRemoved;
            RendererManager.Draw += OnDraw;
        }
        else
        {
            EntityManager.EntityAdded -= OnEntityAdded;
            EntityManager.EntityRemoved -= OnEntityRemoved;
            RendererManager.Draw -= OnDraw;
        }
    }

    private void OnMoveValueChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
    {
        System.Console.WriteLine(e.Value);
        if (e.Value)
        {
            PanelMove.ActivateMove = true;
        }
        else
        {
            PanelMove.ActivateMove = false;
        }
    }

    private void PanelMoveChanged(bool isTimeout, Vector2 position)
    {
        if (isTimeout)
        {
            NetworthPanelMenu.MoveItem.Value = false;
            return;
        }

        NetworthPanelMenu.PositionXItem.Value = (int)position.X;
        NetworthPanelMenu.PositionYItem.Value = (int)position.Y;
    }

    private void OnEntityAdded(EntityAddedEventArgs e)
    {
        if (e.Entity is not Item item)
        {
            return;
        }

        if (e.IsCollection)
        {
            var owner = item.Owner;
            if (owner == null || owner is not Hero hero || hero.IsIllusion)
            {
                return;
            }

            if (!Heroes.ContainsKey(hero))
            {
                Heroes[hero] = (int)item.Cost;
            }
            else
            {
                Heroes[hero] += (int)item.Cost;
            }

            Items.Add(item);
        }
        else
        {
            UpdateManager.BeginInvoke(() =>
            {
                if (!item.IsValid)
                {
                    return;
                }

                var owner = item.Owner;
                if (owner == null || !owner.IsValid || owner is not Hero hero || hero.IsIllusion)
                {
                    return;
                }

                if (!Heroes.ContainsKey(hero))
                {
                    Heroes[hero] = (int)item.Cost;
                }
                else
                {
                    Heroes[hero] += (int)item.Cost;
                }

                Items.Add(item);
            });
        }
    }

    private void OnEntityRemoved(EntityRemovedEventArgs e)
    {
        if (e.Entity is not Item item)
        {
            return;
        }

        if (Items.Remove(item))
        {
            var owner = item.Owner;
            if (owner == null || owner is not Hero hero)
            {
                return;
            }

            if (!Heroes.ContainsKey(hero))
            {
                return;
            }

            Heroes[hero] -= (int)item.Cost;
        }
    }

    private void OnDraw()
    {
        var size = NetworthPanelMenu.SizeItem.Value;
        var sizeX = 20 + size * 0.4f;
        var sizeY = sizeX / 1.30f;

        var panelPosition = new Vector2(NetworthPanelMenu.PositionXItem, NetworthPanelMenu.PositionYItem);
        var panelSize = new Vector2(sizeX * 7.31f, 10 * (sizeY + 2) + 2);
        var heroSize = new Vector2(sizeX * 1.36f, sizeY);
        PanelMove.Size = new Vector2(panelSize.X + 9, panelSize.Y + 9);

        RendererManager.DrawImage("BeAware.Resources.Textures.item_panel.png", new Rect(panelPosition.X - 2, panelPosition.Y, panelSize.X, panelSize.Y));

        var heroes = EntityManager.GetEntities<Hero>().Where(x => !x.IsIllusion);

        var maxNetWorthItems = Heroes.Select(x => x.Value).DefaultIfEmpty().Max();

        var heroPosition = panelPosition;

        foreach (var pair in Heroes.OrderByDescending(x => x.Value))
        {
            var hero = pair.Key;
            var netWorth = pair.Value;

            heroPosition += new Vector2(0, 2);
            RendererManager.DrawImage(hero.Name, new Rect(heroPosition.X, heroPosition.Y, heroSize.X, heroSize.Y), ImageType.Unit, true);
            heroPosition += new Vector2(sizeX * 1.4f, 0);

            var lineMaxSize = panelSize.X - (heroSize.X + 7);
            var lineSize = new Vector2(netWorth * lineMaxSize / maxNetWorthItems, sizeY);

            var color = hero.IsAlly(LocalHero) ? new Color(0, 255, 0, 180) : new Color(255, 0, 0, 180);
            RendererManager.DrawFilledRectangle(new Rect(heroPosition.X, heroPosition.Y, lineSize.X, lineSize.Y), Color.Zero, color, 0);

            var netWorhText = netWorth.ToString();
            var measureText = RendererManager.MeasureText(netWorhText, sizeY);
            RendererManager.DrawText(netWorhText, heroPosition + new Vector2((lineMaxSize / 2) - measureText.X * 0.5f, (lineSize.Y - measureText.Y) / 2), Color.White, sizeY);

            heroPosition = new Vector2(panelPosition.X, sizeY + heroPosition.Y);
        }

        if (PanelMove.ActivateMove)
        {
            var text = PanelMove.Time.ToString();
            var sizeText = RendererManager.MeasureText(text, panelSize.X);

            RendererManager.DrawText(text, panelPosition - new Vector2((sizeText.X * 0.5f) - (panelSize.X / 2), 0), new Color(255, 255, 255, 50), panelSize.X / 2);
            RendererManager.DrawRectangle(new Rect(panelPosition.X - 5, panelPosition.Y - 5, PanelMove.Size.X, PanelMove.Size.Y), Color.WhiteSmoke, 1);
        }
    }
}