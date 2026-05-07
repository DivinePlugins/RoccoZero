namespace BeAware.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;

using Divine.Game;
using Divine.Numerics;
using Divine.Renderer;

public class SideMessage
{
    private static bool mSideMessageInitialized;

    private static IDictionary<string, SideMessage> sideMessages;

    public Vector2 MessagePosition;

    private readonly IDictionary<int, MessageComponent> components;

    private int elements;

    static SideMessage()
    {
        Initialize();
    }

    public SideMessage(
        string name,
        Vector2 size,
        Color? bgColor = null,
        Color? bdColor = null,
        int? enterTime = null,
        int? stayTime = null,
        int? exitTime = null)
    {
        this.MessageName = name;
        this.Size = size;
        this.MessagePosition = new Vector2(RendererManager.ScreenSize.X, (float)(RendererManager.ScreenSize.Y * 0.64));
        this.BackgroundColor = bgColor ?? new Color(0xC0111111);
        this.BackgroundOutlineColor = bdColor ?? new Color(0xFF444444);
        this.EnterTime = enterTime ?? 650;
        this.StayTime = stayTime ?? 2500;
        this.ExitTime = exitTime ?? 650;
        this.components = new Dictionary<int, MessageComponent>();
    }

    public static int? LastTick { get; set; }

    public Color BackgroundColor { get; private set; }

    public Color BackgroundOutlineColor { get; private set; }

    public int CreateTick { get; set; }

    public int EnterTime { get; private set; }

    public int ExitTime { get; private set; }

    public string MessageName { get; set; }

    public Vector2 Size { get; set; }

    public int StayTime { get; private set; }

    public bool Visible { get; set; }

    private static float TickCount
    {
        get
        {
            if (!GameManager.IsInGame)
            {
                return Environment.TickCount & int.MaxValue;
            }

            return GameManager.RawGameTime * 1000;
        }
    }

    public static void Initialize()
    {
        if (mSideMessageInitialized)
        {
            return;
        }

        sideMessages = new Dictionary<string, SideMessage>();
        mSideMessageInitialized = true;
        RendererManager.Draw += OnDraw;
    }

    public void AddElement(Vector2 position, Vector2 size, string textureKey, ImageType imageType = ImageType.Default)
    {
        var element = new MessageComponent(position, size, textureKey, imageType)
        {
            Parent = this,
            ComponentType = "TextureKey"
        };
        this.components.Add(this.elements, element);
        this.elements++;
    }

    public void CreateMessage()
    {
        this.CreateTick = (int)TickCount;
        foreach (var message in sideMessages.Where(message => message.Value.Visible))
        {
            message.Value.ShiftVec(new Vector2(0, -message.Value.Size.Y - 3));
        }

        this.Visible = true;
        sideMessages[this.MessageName] = this;
    }

    public void DestroyMessage()
    {
        this.Visible = false;
        this.components.Clear();
        sideMessages.Remove(this.MessageName);
    }

    private static void OnDraw()
    {
        if (!GameManager.IsInGame || !sideMessages.Any())
        {
            return;
        }

        if (LastTick != null)
        {
            for (var i = 0; i < sideMessages.Count; i++)
            {
                var message = sideMessages.ElementAt(i).Value;
                if (!message.Visible)
                {
                    continue;
                }

                var span = TickCount - message.CreateTick;
                if (span < message.EnterTime)
                {
                    message.SetX(RendererManager.ScreenSize.X - (message.Size.X - 1) * span / message.EnterTime);
                }
                else if (span < message.EnterTime + message.StayTime)
                {
                    message.SetX(RendererManager.ScreenSize.X - message.Size.X + 1);
                }
                else if (span < message.EnterTime + message.StayTime + message.ExitTime)
                {
                    message.SetX(RendererManager.ScreenSize.X - (message.Size.X - 1) * (message.EnterTime + message.StayTime + message.ExitTime - span) / message.ExitTime);
                }
                else
                {
                    message.DestroyMessage();
                }
            }

            for (var i = 0; i < sideMessages.Count; i++)
            {
                var message = sideMessages.ElementAt(i).Value;
                if (!message.Visible)
                {
                    continue;
                }

                //Drawing.DrawRect(message.MessagePosition, message.Size, message.BackgroundColor);
                //Drawing.DrawRect(message.MessagePosition, message.Size, message.BackgroundOutlineColor, true);

                RendererManager.DrawFilledRectangle(new RectangleF(message.MessagePosition.X, message.MessagePosition.Y, message.Size.X, message.Size.Y), message.BackgroundColor, message.BackgroundColor, 0);
                RendererManager.DrawRectangle(new RectangleF(message.MessagePosition.X, message.MessagePosition.Y, message.Size.X, message.Size.Y), message.BackgroundOutlineColor, 1);

                foreach (var component in message.components)
                {
                    component.Value.Draw();
                }
            }
        }

        LastTick = (int?)TickCount;
    }

    private void SetX(float x)
    {
        this.MessagePosition.X = x;
    }

    private void ShiftVec(Vector2 vector)
    {
        this.MessagePosition += vector;
    }

    public class MessageComponent
    {
        public MessageComponent(Vector2 position, Vector2 size, Color color, bool outline = false)
        {
            this.Position = position;
            this.Size = size;
            this.Color = color;
            this.IsOutline = outline;
        }

        public MessageComponent(Vector2 position, Vector2 size, string textureKey, ImageType imageType)
        {
            this.Position = position;
            this.Size = size;
            this.TextureKey = textureKey;
            this.ImageType = imageType;
        }

        public MessageComponent(string text, Vector2 position, Color color, FontFlags fontFlags)
        {
            this.Text = text;
            this.Position = position;
            this.Color = color;
            this.Flags = fontFlags;
        }

        public MessageComponent(string text, Vector2 position, Vector2 size, Color color, FontFlags fontFlags)
        {
            this.Text = text;
            this.Position = position;
            this.Size = size;
            this.Color = color;
            this.Flags = fontFlags;
        }

        public MessageComponent(
            string text,
            string fontName,
            Vector2 position,
            Vector2 size,
            Color color,
            FontFlags fontFlags)
        {
            this.Text = text;
            this.Position = position;
            this.FontFace = fontName;
            this.Size = size;
            this.Color = color;
            this.Flags = fontFlags;
        }

        public MessageComponent(Vector2 position, Vector2 size, Color color)
        {
            this.Position = position;
            this.Size = size;
            this.Color = color;
        }

        public string ComponentType { get; set; }

        public SideMessage Parent { get; set; }

        private Color Color { get; set; }

        private FontFlags Flags { get; set; }

        private string FontFace { get; set; }

        private bool IsOutline { get; set; }

        private Vector2 Position { get; set; }

        private Vector2 Size { get; set; }

        private string Text { get; set; }

        private string TextureKey { get; set; }
        
        private ImageType ImageType { get; set; }

        public void Draw()
        {
            var position = Parent.MessagePosition + this.Position;
            RendererManager.DrawImage(TextureKey, new RectangleF(position.X, position.Y, Size.X, Size.Y), ImageType);
        }
    }
}