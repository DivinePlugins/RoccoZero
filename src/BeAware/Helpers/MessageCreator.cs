namespace BeAware.Helpers;

using System;

using BeAware.MenuManager;

using Divine.Numerics;
using Divine.Renderer;

internal class MessageCreator
{
    private const string Textures = "BeAware.Resources.Textures";

    private MenuConfig MenuConfig { get; }

    private float MsgX { get; set; }

    private float MsgY { get; set; }

    private float HeroSizeX { get; set; }

    private float HeroSpellSizeY { get; set; }

    private float ItemSizeX { get; set; }

    private float HeroSpellY { get; set; }

    private float HeroAllyX { get; set; }

    private float HeroX { get; set; }

    private float SpellX { get; set; }

    private float ItemX { get; set; }

    private string Lang => MenuConfig.LanguageItem;

    private readonly Random random = new();

    public MessageCreator(Common common)
    {
        MenuConfig = common.MenuConfig;

        Resolution();
    }

    public void MessageAllyCreator(string heroTextureName, string spellTextureName)
    {
        var msg0TextureKey = $"{Textures}.msg0_{Lang.ToLower()}.png";

        RendererManager.LoadImage(heroTextureName, ImageType.Unit);
        RendererManager.LoadImage(spellTextureName, ImageType.Ability);
        RendererManager.LoadImageFromAssembly(msg0TextureKey);

        var sideMessage = new SideMessage(random.Next(99999999).ToString(), new Vector2(MsgX, MsgY), stayTime: 5000);
        sideMessage.AddElement(new Vector2(0, 0), new Vector2(MsgX, MsgY), msg0TextureKey);
        sideMessage.AddElement(new Vector2(HeroAllyX, HeroSpellY), new Vector2(HeroSizeX, HeroSpellSizeY), heroTextureName, ImageType.Unit);
        sideMessage.AddElement(new Vector2(HeroX, HeroSpellY), new Vector2(HeroSpellSizeY, HeroSpellSizeY), spellTextureName, ImageType.Ability);
        sideMessage.CreateMessage();
    }

    public void MessageEnemyCreator(string heroTextureName, string spellTextureName)
    {
        var msg1TextureKey = $"{Textures}.msg1_{Lang.ToLower()}.png";

        RendererManager.LoadImage(heroTextureName, ImageType.Unit);
        RendererManager.LoadImage(spellTextureName, ImageType.Ability);
        RendererManager.LoadImageFromAssembly(msg1TextureKey);

        var sideMessage = new SideMessage(random.Next(99999999).ToString(), new Vector2(MsgX, MsgY), stayTime: 5000);
        sideMessage.AddElement(new Vector2(0, 0), new Vector2(MsgX, MsgY), msg1TextureKey);
        sideMessage.AddElement(new Vector2(HeroX, HeroSpellY), new Vector2(HeroSizeX, HeroSpellSizeY), heroTextureName, ImageType.Unit);
        sideMessage.AddElement(new Vector2(SpellX, HeroSpellY), new Vector2(HeroSpellSizeY, HeroSpellSizeY), spellTextureName, ImageType.Ability);
        sideMessage.CreateMessage();
    }

    public void MessageRuneCreator(string heroTextureName, string runeTextureName)
    {
        var msg2TextureKey = $"{Textures}.msg2_{Lang.ToLower()}.png";

        RendererManager.LoadImage(heroTextureName, ImageType.Unit);
        RendererManager.LoadImage(runeTextureName, ImageType.Ability);
        RendererManager.LoadImageFromAssembly(msg2TextureKey);

        var sideMessage = new SideMessage(random.Next(99999999).ToString(), new Vector2(MsgX, MsgY), stayTime: 5000);
        sideMessage.AddElement(new Vector2(0, 0), new Vector2(MsgX, MsgY), msg2TextureKey);
        sideMessage.AddElement(new Vector2(HeroX, HeroSpellY), new Vector2(HeroSizeX, HeroSpellSizeY), heroTextureName, ImageType.Unit);
        sideMessage.AddElement(new Vector2(SpellX, HeroSpellY), new Vector2(HeroSpellSizeY, HeroSpellSizeY), runeTextureName, ImageType.Ability);
        sideMessage.CreateMessage();
    }

    public void MessageItemCreator(string heroTextureName, string itemTextureName)
    {
        var msg3TextureKey = $"{Textures}.msg3_{Lang.ToLower()}.png";

        RendererManager.LoadImage(heroTextureName, ImageType.Unit);
        RendererManager.LoadImage(itemTextureName, ImageType.Item);
        RendererManager.LoadImageFromAssembly(msg3TextureKey);

        var sideMessage = new SideMessage(random.Next(99999999).ToString(), new Vector2(MsgX, MsgY), stayTime: 5000);
        sideMessage.AddElement(new Vector2(0, 0), new Vector2(MsgX, MsgY), msg3TextureKey);
        sideMessage.AddElement(new Vector2(HeroX, HeroSpellY), new Vector2(HeroSizeX, HeroSpellSizeY), heroTextureName, ImageType.Unit);
        sideMessage.AddElement(new Vector2(ItemX, HeroSpellY), new Vector2(ItemSizeX, HeroSpellSizeY), itemTextureName, ImageType.Item);
        sideMessage.CreateMessage();
    }

    public void MessageCheckRuneCreator()
    {
        var msg4TextureKey = $"{Textures}.msg4_{Lang.ToLower()}.png";
        RendererManager.LoadImageFromAssembly(msg4TextureKey);

        var sideMessage = new SideMessage("check_rune", new Vector2(MsgX, MsgY), stayTime: 5000);
        sideMessage.AddElement(new Vector2(0, 0), new Vector2(MsgX, MsgY), msg4TextureKey);
        sideMessage.CreateMessage();
    }
    public void MessageUseMidasCreator()
    {
        var msg5TextureKey = $"{Textures}.msg5_{Lang.ToLower()}.png";
        RendererManager.LoadImageFromAssembly(msg5TextureKey);

        var sideMessage = new SideMessage("use_midas", new Vector2(MsgX, MsgY), stayTime: 5000);
        sideMessage.AddElement(new Vector2(0, 0), new Vector2(MsgX, MsgY), msg5TextureKey);
        sideMessage.CreateMessage();
    }

    public void MessageRoshanAliveCreator()
    {
        var msg6TextureKey = $"{Textures}.msg6_{Lang.ToLower()}.png";
        RendererManager.LoadImageFromAssembly(msg6TextureKey);

        var sideMessage = new SideMessage("roshan_alive", new Vector2(MsgX, MsgY), stayTime: 5000);
        sideMessage.AddElement(new Vector2(0, 0), new Vector2(MsgX, MsgY), msg6TextureKey);
        sideMessage.CreateMessage();
    }

    public void MessageRoshanMBAliveCreator()
    {
        var msg7TextureKey = $"{Textures}.msg7_{Lang.ToLower()}.png";
        RendererManager.LoadImageFromAssembly(msg7TextureKey);

        var sideMessage = new SideMessage("roshan_mb_alive", new Vector2(MsgX, MsgY), stayTime: 5000);
        sideMessage.AddElement(new Vector2(0, 0), new Vector2(MsgX, MsgY), msg7TextureKey);
        sideMessage.CreateMessage();
    }

    private void Resolution()
    {
        var screenSize = new Vector2(RendererManager.ScreenSize.X, RendererManager.ScreenSize.Y);

        //1920x1080 (16:9)
        if (screenSize == new Vector2(1920, 1080))
        {
            MsgX = 256;
            MsgY = 128;
            HeroSizeX = 97;
            HeroSpellSizeY = 55;
            ItemSizeX = 80;
            HeroSpellY = 62;
            HeroAllyX = 152;
            HeroX = 9;
            SpellX = 193;
            ItemX = 170;
        }

        //1768x992 (16:9)
        else if (screenSize == new Vector2(1768, 992))
        {
            MsgX = 248;
            MsgY = 120;
            HeroSizeX = 97;
            HeroSpellSizeY = 55;
            ItemSizeX = 80;
            HeroSpellY = 56;
            HeroAllyX = 145;
            HeroX = 9;
            SpellX = 186;
            ItemX = 165;
        }

        //1600x900 (16:9)
        else if (screenSize == new Vector2(1600, 900))
        {
            MsgX = 235;
            MsgY = 110;
            HeroSizeX = 88;
            HeroSpellSizeY = 50;
            ItemSizeX = 80;
            HeroSpellY = 52;
            HeroAllyX = 140;
            HeroX = 8;
            SpellX = 177;
            ItemX = 157;
        }

        //1360x768 (16:9)
        else if (screenSize == new Vector2(1360, 768))
        {
            MsgX = 195;
            MsgY = 95;
            HeroSizeX = 68;
            HeroSpellSizeY = 42;
            ItemSizeX = 80;
            HeroSpellY = 46;
            HeroAllyX = 121;
            HeroX = 7;
            SpellX = 146;
            ItemX = 130;
        }
        //1366x768 (16:9)
        else if (screenSize == new Vector2(1366, 768))
        {
            MsgX = 195;
            MsgY = 95;
            HeroSizeX = 68;
            HeroSpellSizeY = 42;
            ItemSizeX = 80;
            HeroSpellY = 46;
            HeroAllyX = 121;
            HeroX = 7;
            SpellX = 146;
            ItemX = 130;
        }

        //1280x720 (16:9)
        else if (screenSize == new Vector2(1280, 720))
        {
            MsgX = 190;
            MsgY = 90;
            HeroSizeX = 64;
            HeroSpellSizeY = 39;
            ItemSizeX = 80;
            HeroSpellY = 44;
            HeroAllyX = 121;
            HeroX = 6;
            SpellX = 146;
            ItemX = 130;
        }

        //1680x1050 (16:10)
        else if (screenSize == new Vector2(1680, 1050))
        {
            MsgX = 256;
            MsgY = 128;
            HeroSizeX = 97;
            HeroSpellSizeY = 55;
            ItemSizeX = 80;
            HeroSpellY = 62;
            HeroAllyX = 152;
            HeroX = 9;
            SpellX = 193;
            ItemX = 170;
        }

        //1600x1024 (16:10)
        else if (screenSize == new Vector2(1600, 1024))
        {
            MsgX = 256;
            MsgY = 128;
            HeroSizeX = 92;
            HeroSpellSizeY = 55;
            ItemSizeX = 80;
            HeroSpellY = 62;
            HeroAllyX = 152;
            HeroX = 9;
            SpellX = 190;
            ItemX = 170;
        }

        //1440x960 (16:10) //1440x900 (16:10)
        else if (screenSize == new Vector2(1440, 960) || screenSize == new Vector2(1440, 900))
        {
            MsgX = 220;
            MsgY = 110;
            HeroSizeX = 73;
            HeroSpellSizeY = 48;
            ItemSizeX = 92;
            HeroSpellY = 53;
            HeroAllyX = 141;
            HeroX = 8;
            SpellX = 166;
            ItemX = 150;
        }

        //1280x800 (16:10)
        else if (screenSize == new Vector2(1280, 800))
        {
            MsgX = 195;
            MsgY = 110;
            HeroSizeX = 70;
            HeroSpellSizeY = 48;
            ItemSizeX = 88;
            HeroSpellY = 53;
            HeroAllyX = 120;
            HeroX = 7;
            SpellX = 142;
            ItemX = 130;
        }

        //1280x768 (16:10)
        else if (screenSize == new Vector2(1280, 768))
        {
            MsgX = 195;
            MsgY = 110;
            HeroSizeX = 70;
            HeroSpellSizeY = 48;
            ItemSizeX = 88;
            HeroSpellY = 53;
            HeroAllyX = 120;
            HeroX = 7;
            SpellX = 142;
            ItemX = 130;
        }

        //1280x1024 (4:3)
        else if (screenSize == new Vector2(1280, 1024))
        {
            MsgX = 228;
            MsgY = 110;
            HeroSizeX = 75;
            HeroSpellSizeY = 48;
            ItemSizeX = 92;
            HeroSpellY = 53;
            HeroAllyX = 146;
            HeroX = 8;
            SpellX = 174;
            ItemX = 159;
        }

        //1280x960 (4:3)
        else if (screenSize == new Vector2(1280, 960))
        {
            MsgX = 228;
            MsgY = 110;
            HeroSizeX = 75;
            HeroSpellSizeY = 48;
            ItemSizeX = 92;
            HeroSpellY = 53;
            HeroAllyX = 146;
            HeroX = 8;
            SpellX = 174;
            ItemX = 159;
        }
        else
        {
            //Console.WriteLine(@"Your screen resolution is not supported and drawings might have wrong size (" + screenSize + ")");
            MsgX = 256;
            MsgY = 128;
            HeroSizeX = 97;
            HeroSpellSizeY = 55;
            ItemSizeX = 80;
            HeroSpellY = 62;
            HeroAllyX = 152;
            HeroX = 9;
            SpellX = 193;
            ItemX = 170;
        }
    }
}