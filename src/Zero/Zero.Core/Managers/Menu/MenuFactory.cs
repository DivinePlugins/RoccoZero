namespace Divine.Core.Managers.Menu;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items.Components;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Input;
using Divine.Menu;
using Divine.Menu.Components;
using Divine.Menu.Items;

using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Attributes;

internal static class MenuFactory
{
    public static void RegisterMenu(object obj)
    {
        var type = obj.GetType();

        var rootMenuAttribute = type.GetCustomAttribute<MenuAttribute>();
        if (rootMenuAttribute == null)
        {
            return;
        }

        var rootMenu = MenuManager.HeroesMenu.AddMenu((rootMenuAttribute.Name, rootMenuAttribute.DisplayName));

        var imageAttribute = type.GetCustomAttributes<ImageAttribute>().LastOrDefault();
        if (imageAttribute != null)
        {
            if (imageAttribute is HeroImageAttribute heroImageAttribute)
            {
                rootMenu.SetImage(heroImageAttribute.HeroId);
            }
            else if (imageAttribute is AbilityImageAttribute abilityImageAttribute)
            {
                rootMenu.SetImage(abilityImageAttribute.AbilityId);
            }
        }

        var tooltipAttribute = type.GetCustomAttributes<TooltipAttribute>().LastOrDefault();
        if (tooltipAttribute != null)
        {
            rootMenu.SetTooltip(tooltipAttribute.Text);
        }

        var properties = obj.GetType().GetProperties();

        foreach (var property in properties.OrderBy(x => Priority(properties, x)))
        {
            var menuAttribute = property.GetCustomAttribute<MenuAttribute>();
            if (menuAttribute != null)
            {
                CreateMenu(menuAttribute.Name, menuAttribute.DisplayName, rootMenu, property.GetValue(obj), property);
            }
            else
            {
                var itemAttribute = property.GetCustomAttribute<ItemAttribute>();
                if (itemAttribute == null)
                {
                    continue;
                }

                CreateMenuItem(itemAttribute.Name, itemAttribute.DisplayName, rootMenu, obj, property);
            }
        }
    }

    private static void CreateMenu(string name, string displayName, Menu parentMenu, object obj, PropertyInfo parentProperty)
    {
        var menu = parentMenu.AddMenu((name, displayName));

        var imageAttribute = parentProperty.GetCustomAttributes<ImageAttribute>(false).LastOrDefault()
                ?? parentProperty.GetCustomAttributes<ImageAttribute>().LastOrDefault();

        if (imageAttribute != null)
        {
            if (imageAttribute is HeroImageAttribute heroImageAttribute)
            {
                menu.SetImage(heroImageAttribute.HeroId);
            }
            else if (imageAttribute is AbilityImageAttribute abilityImageAttribute)
            {
                menu.SetImage(abilityImageAttribute.AbilityId);
            }
        }

        var tooltipAttribute = parentProperty.GetCustomAttributes<TooltipAttribute>(false).LastOrDefault()
                ?? parentProperty.GetCustomAttributes<TooltipAttribute>().LastOrDefault();

        if (tooltipAttribute != null)
        {
            menu.SetTooltip(tooltipAttribute.Text);
        }

        var properties = obj.GetType().GetProperties();

        foreach (var property in properties.OrderBy(x => Priority(properties, x)))
        {
            var menuAttribute = property.GetCustomAttribute<MenuAttribute>();
            if (menuAttribute != null)
            {
                CreateMenu(menuAttribute.Name, menuAttribute.DisplayName, menu, property.GetValue(obj), property);
            }
            else
            {
                var itemAttribute = property.GetCustomAttribute<ItemAttribute>();
                if (itemAttribute == null)
                {
                    continue;
                }

                CreateMenuItem(itemAttribute.Name, itemAttribute.DisplayName, menu, obj, property);
            }
        }
    }

    private static void CreateMenuItem(string name, string displayName, Menu parentMenu, object obj, PropertyInfo property)
    {
        MenuItem menuItem = null;

        var propertyType = property.PropertyType;
        if (propertyType == typeof(MenuAbilityToggler))
        {
            var parameterAttribute = property.GetCustomAttributes<ParameterAttribute>(false).LastOrDefault(x => x.Name.Equals("priority", StringComparison.InvariantCultureIgnoreCase))
                 ?? property.GetCustomAttributes<ParameterAttribute>().LastOrDefault(x => x.Name.Equals("priority", StringComparison.InvariantCultureIgnoreCase));

            var values = new Dictionary<AbilityId, bool>();

            var valueAttributes = GetValueAttributes(property);

            foreach (var valueAttribute in valueAttributes)
            {
                var objects = valueAttribute.Objects;
                if (objects.Length < 2)
                {
                    continue;
                }

                if (objects[0] is not AbilityId abilityId || objects[1] is not bool value)
                {
                    continue;
                }

                values[abilityId] = value;
            }

            menuItem = parentMenu.AddAbilityToggler(
                (name, displayName),
                (bool?)parameterAttribute?.Value ?? false ? MenuTogglerOptions.PriorityToggler : MenuTogglerOptions.Toggler,
                values);
        }
        else if (propertyType == typeof(MenuHeroToggler))
        {
            var parameterAttribute = property.GetCustomAttributes<ParameterAttribute>(false).LastOrDefault(x => x.Name.Equals("priority", StringComparison.InvariantCultureIgnoreCase))
                 ?? property.GetCustomAttributes<ParameterAttribute>().LastOrDefault(x => x.Name.Equals("priority", StringComparison.InvariantCultureIgnoreCase));

            var values = new Dictionary<HeroId, bool>();

            var valueAttributes = GetValueAttributes(property);

            foreach (var valueAttribute in valueAttributes)
            {
                var objects = valueAttribute.Objects;
                if (objects.Length < 2)
                {
                    continue;
                }

                if (objects[0] is not HeroId heroId || objects[1] is not bool value)
                {
                    continue;
                }

                values[heroId] = value;
            }

            menuItem = parentMenu.AddHeroToggler(
                (name, displayName),
                (bool?)parameterAttribute?.Value ?? false ? MenuTogglerOptions.PriorityToggler : MenuTogglerOptions.Toggler,
                values);
        }
        else if (propertyType == typeof(MenuHoldKey))
        {
            var objects = GetValueAttributes(property).Select(x => x.Objects).LastOrDefault();
            if (objects == null || objects.Length < 1)
            {
                menuItem = parentMenu.AddHoldKey((name, displayName));
            }
            else
            {
                menuItem = parentMenu.AddHoldKey((name, displayName), (Key)objects[0]);
            }
        }
        else if (propertyType == typeof(MenuItemToggler))
        {
            var parameterAttribute = property.GetCustomAttributes<ParameterAttribute>(false).LastOrDefault(x => x.Name.Equals("priority", StringComparison.InvariantCultureIgnoreCase))
                 ?? property.GetCustomAttributes<ParameterAttribute>().LastOrDefault(x => x.Name.Equals("priority", StringComparison.InvariantCultureIgnoreCase));

            var values = new Dictionary<ItemId, bool>();

            var valueAttributes = GetValueAttributes(property);

            foreach (var valueAttribute in valueAttributes)
            {
                var objects = valueAttribute.Objects;
                if (objects.Length < 2)
                {
                    continue;
                }

                if (objects[0] is not ItemId itemId || objects[1] is not bool value)
                {
                    continue;
                }

                values[itemId] = value;
            }

            menuItem = parentMenu.AddItemToggler(
                (name, displayName),
                (bool?)parameterAttribute?.Value ?? false ? MenuTogglerOptions.PriorityToggler : MenuTogglerOptions.Toggler,
                values);
        }
        else if (propertyType == typeof(MenuSelector))
        {
            var objects = GetValueAttributes(property).Select(x => x.Objects).LastOrDefault();
            if (objects != null)
            {
                menuItem = parentMenu.AddSelector((name, displayName), objects.Cast<string>().ToArray());
            }
        }
        else if (propertyType == typeof(MenuSlider))
        {
            var objects = GetValueAttributes(property).Select(x => x.Objects).LastOrDefault(x => x.Length >= 3);
            if (objects != null)
            {
                menuItem = parentMenu.AddSlider((name, displayName), (int)objects[0], (int)objects[1], (int)objects[2]);
            }
        }
        else if (propertyType == typeof(MenuAbilityToggler))
        {
            var parameterAttribute = property.GetCustomAttributes<ParameterAttribute>(false).LastOrDefault(x => x.Name.Equals("priority", StringComparison.InvariantCultureIgnoreCase))
                 ?? property.GetCustomAttributes<ParameterAttribute>().LastOrDefault(x => x.Name.Equals("priority", StringComparison.InvariantCultureIgnoreCase));

            var values = new Dictionary<AbilityId, bool>();

            var valueAttributes = GetValueAttributes(property);

            foreach (var valueAttribute in valueAttributes)
            {
                var objects = valueAttribute.Objects;
                if (objects.Length < 2)
                {
                    continue;
                }

                if (objects[0] is not AbilityId abilityId || objects[1] is not bool value)
                {
                    continue;
                }

                values[abilityId] = value;
            }

            menuItem = parentMenu.AddAbilityToggler(
                (name, displayName),
                (bool?)parameterAttribute?.Value ?? false ? MenuTogglerOptions.PriorityToggler : MenuTogglerOptions.Toggler,
                values);
        }
        else if (propertyType == typeof(MenuSwitcher))
        {
            var objects = GetValueAttributes(property).Select(x => x.Objects).LastOrDefault(x => x.Length >= 1);
            menuItem = parentMenu.AddSwitcher((name, displayName), (bool?)objects?[0] ?? true);
        }
        else if (propertyType == typeof(MenuText))
        {
            menuItem = parentMenu.AddText((name, displayName));
        }
        else if (propertyType == typeof(MenuToggleKey))
        {
            var objects = GetValueAttributes(property).Select(x => x.Objects).LastOrDefault();
            if (objects == null || objects.Length < 1)
            {
                menuItem = parentMenu.AddToggleKey((name, displayName));
            }
            else if (objects.Length == 1)
            {
                menuItem = parentMenu.AddToggleKey((name, displayName), (Key)objects[0]);
            }
            else
            {
                menuItem = parentMenu.AddToggleKey((name, displayName), (Key)objects[0], (bool)objects[1]);
            }
        }
        else if (propertyType == typeof(MenuToggler))
        {
            var parameterAttribute = property.GetCustomAttributes<ParameterAttribute>(false).LastOrDefault(x => x.Name.Equals("priority", StringComparison.InvariantCultureIgnoreCase))
                 ?? property.GetCustomAttributes<ParameterAttribute>().LastOrDefault(x => x.Name.Equals("priority", StringComparison.InvariantCultureIgnoreCase));

            var values = new Dictionary<string, bool>();

            var valueAttributes = GetValueAttributes(property);

            foreach (var valueAttribute in valueAttributes)
            {
                var objects = valueAttribute.Objects;
                if (objects.Length < 2)
                {
                    continue;
                }

                if (objects[0] is not string str || objects[1] is not bool value)
                {
                    continue;
                }

                values[str] = value;
            }

            menuItem = parentMenu.AddToggler(
                (name, displayName),
                (bool?)parameterAttribute?.Value ?? false ? MenuTogglerOptions.PriorityToggler : MenuTogglerOptions.Toggler,
                values);
        }
        //else if (propertyType == typeof(MenuUnitToggler))
        //{
        //    var parameterAttribute = property.GetCustomAttributes<ParameterAttribute>(false).LastOrDefault(x => x.Name.Equals("priority", StringComparison.InvariantCultureIgnoreCase))
        //         ?? property.GetCustomAttributes<ParameterAttribute>().LastOrDefault(x => x.Name.Equals("priority", StringComparison.InvariantCultureIgnoreCase));

        //    var values = new Dictionary<string, bool>();

        //    var valueAttributes = GetValueAttributes(property);

        //    foreach (var valueAttribute in valueAttributes)
        //    {
        //        var objects = valueAttribute.Objects;
        //        if (objects.Length < 2)
        //        {
        //            continue;
        //        }

        //        if (objects[0] is not string str || objects[1] is not bool value)
        //        {
        //            continue;
        //        }

        //        values[str] = value;
        //    }

        //    menuItem = parentMenu.CreateUnitToggler(name, displayName, values, (bool?)parameterAttribute?.Value ?? false);
        //}

        if (menuItem != null)
        {
            var imageAttribute = property.GetCustomAttributes<ImageAttribute>(false).LastOrDefault()
                ?? property.GetCustomAttributes<ImageAttribute>().LastOrDefault();

            if (imageAttribute != null)
            {
                if (imageAttribute is HeroImageAttribute heroImageAttribute)
                {
                    menuItem.SetImage(heroImageAttribute.HeroId);
                }
                else if (imageAttribute is AbilityImageAttribute abilityImageAttribute)
                {
                    menuItem.SetImage(abilityImageAttribute.AbilityId);
                }
            }

            var tooltipAttribute = property.GetCustomAttributes<TooltipAttribute>(false).LastOrDefault()
                ?? property.GetCustomAttributes<TooltipAttribute>().LastOrDefault();

            if (tooltipAttribute != null)
            {
                menuItem.SetTooltip(tooltipAttribute.Text);
            }

            property.SetValue(obj, menuItem);
        }
    }

    public static void DeregisterMenu(object obj)
    {
    }

    private static IEnumerable<ValueAttribute> GetValueAttributes(PropertyInfo property)
    {
        var valueAttributes = property.GetCustomAttributes<ValueAttribute>(false);
        if (!valueAttributes.Any())
        {
            valueAttributes = property.GetCustomAttributes<ValueAttribute>();
        }

        return valueAttributes;
    }

    private static int Priority(PropertyInfo[] propertyInfos, PropertyInfo propertyInfo)
    {
        var priorityAttribute = propertyInfo.GetCustomAttribute<PriorityAttribute>();
        if (priorityAttribute != null)
        {
            return priorityAttribute.Value;
        }

        return Array.IndexOf(propertyInfos, propertyInfo);
    }
}