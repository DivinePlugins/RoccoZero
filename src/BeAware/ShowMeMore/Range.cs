namespace BeAware.ShowMeMore;

using BeAware.MenuManager.ShowMeMore.Range;

using Divine.Entity;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Numerics;
using Divine.Particle;
using Divine.Update;

internal sealed class Range
{
    private readonly Hero LocalHero = EntityManager.LocalHero;

    private readonly string CustomRangeName = $"CustomRange_{EntityManager.LocalHero.Handle}";

    private readonly CustomRangeMenu CustomRangeMenu;

    private readonly CustomRange2Menu CustomRange2Menu;

    private readonly CustomRange3Menu CustomRange3Menu;

    public Range(Common common)
    {
        var rangeMenu = common.MenuConfig.ShowMeMoreMenu.RangeMenu;
        CustomRangeMenu = rangeMenu.CustomRangeMenu;
        CustomRange2Menu = rangeMenu.CustomRange2Menu;
        CustomRange3Menu = rangeMenu.CustomRange3Menu;

        CustomRangeMenu.EnableItem.ValueChanged += OnValueChanged;
        CustomRangeMenu.RangeItem.ValueChanged += OnValueChanged;
        CustomRangeMenu.RedItem.ValueChanged += OnValueChanged;
        CustomRangeMenu.GreenItem.ValueChanged += OnValueChanged;
        CustomRangeMenu.BlueItem.ValueChanged += OnValueChanged;

        CustomRange2Menu.EnableItem.ValueChanged += OnValueChanged;
        CustomRange2Menu.RangeItem.ValueChanged += OnValueChanged;
        CustomRange2Menu.RedItem.ValueChanged += OnValueChanged;
        CustomRange2Menu.GreenItem.ValueChanged += OnValueChanged;
        CustomRange2Menu.BlueItem.ValueChanged += OnValueChanged;

        CustomRange3Menu.EnableItem.ValueChanged += OnValueChanged;
        CustomRange3Menu.RangeItem.ValueChanged += OnValueChanged;
        CustomRange3Menu.RedItem.ValueChanged += OnValueChanged;
        CustomRange3Menu.GreenItem.ValueChanged += OnValueChanged;
        CustomRange3Menu.BlueItem.ValueChanged += OnValueChanged;
    }

    public void Dispose()
    {
        CustomRangeMenu.EnableItem.ValueChanged -= OnValueChanged;
        CustomRangeMenu.RangeItem.ValueChanged -= OnValueChanged;
        CustomRangeMenu.RedItem.ValueChanged -= OnValueChanged;
        CustomRangeMenu.GreenItem.ValueChanged -= OnValueChanged;
        CustomRangeMenu.BlueItem.ValueChanged -= OnValueChanged;

        CustomRange2Menu.EnableItem.ValueChanged -= OnValueChanged;
        CustomRange2Menu.RangeItem.ValueChanged -= OnValueChanged;
        CustomRange2Menu.RedItem.ValueChanged -= OnValueChanged;
        CustomRange2Menu.GreenItem.ValueChanged -= OnValueChanged;
        CustomRange2Menu.BlueItem.ValueChanged -= OnValueChanged;

        CustomRange3Menu.EnableItem.ValueChanged -= OnValueChanged;
        CustomRange3Menu.RangeItem.ValueChanged -= OnValueChanged;
        CustomRange3Menu.RedItem.ValueChanged -= OnValueChanged;
        CustomRange3Menu.GreenItem.ValueChanged -= OnValueChanged;
        CustomRange3Menu.BlueItem.ValueChanged -= OnValueChanged;
    }

    private void OnValueChanged(object sender, object e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (CustomRangeMenu.EnableItem)
            {
                AddOrUpdate(1, CustomRangeMenu.RangeItem.Value, new Color((int)CustomRangeMenu.RedItem.Value, (int)CustomRangeMenu.GreenItem.Value, (int)CustomRangeMenu.BlueItem.Value));
            }
            else
            {
                Remove(1);
            }

            if (CustomRange2Menu.EnableItem)
            {
                AddOrUpdate(2, CustomRange2Menu.RangeItem.Value, new Color((int)CustomRange2Menu.RedItem.Value, (int)CustomRange2Menu.GreenItem.Value, (int)CustomRange2Menu.BlueItem.Value));
            }
            else
            {
                Remove(2);
            }

            if (CustomRange3Menu.EnableItem)
            {
                AddOrUpdate(3, CustomRange3Menu.RangeItem.Value, new Color((int)CustomRange3Menu.RedItem.Value, (int)CustomRange3Menu.GreenItem.Value, (int)CustomRange3Menu.BlueItem.Value));
            }
            else
            {
                Remove(3);
            }
        });
    }

    private void AddOrUpdate(int index, float range, Color color)
    {
        ParticleManager.CreateRangeParticle($"{CustomRangeName}_{index}", LocalHero, range, color);
    }

    private void Remove(int index)
    {
        ParticleManager.DestroyParticle($"{CustomRangeName}_{index}");
    }
}