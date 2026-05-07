using Divine.Core.Managers;

namespace Divine.Core.ComboFactory.Helpers
{
    public abstract class BaseAbilities
    {
        protected BaseAbilities()
        {
            //foreach (var method in GetType().GetMethods())
            //{
            //    var name = method.ReturnType.Namespace;
            //    if (name.Contains("Spells"))
            //    {
            //        var spellName = method.ReturnType.GetCustomAttribute<SpellAttribute>().AbilityId.ToString();
            //        LoadFromDivine(spellName, directoryTextures + $@"spells\{spellName}.png");
            //    }

            //    if (name.Contains("Items"))
            //    {
            //        var itemName = method.ReturnType.GetCustomAttributes<ItemAttribute>().Last().AbilityId.ToString();
            //        LoadFromDivine(itemName, directoryTextures + $@"square_items\{itemName}.png");
            //    }
            //}
        }

        public virtual void Dispose()
        {
            ActionManager.ActionRemove(this);
        }

        //private readonly static ITextureManager textureManager = DivineService.Context.TextureManager;

        //private static readonly string directoryTextures = DivineDirectory.Textures;

        //private static void LoadFromDivine(string textureKey, string divineTexture)
        //{
        //    if (!File.Exists(divineTexture))
        //    {
        //        textureManager.LoadFromResource(textureKey, "Resources.Empty.png");
        //        return;
        //    }

        //    textureManager.LoadFromFile(textureKey, divineTexture);
        //}
    }
}