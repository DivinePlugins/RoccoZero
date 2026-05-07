using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Divine.Game;
using Divine.Numerics;
using Divine.Renderer;

namespace Divine.Core.Helpers
{
    public sealed class DivineMessage
    {
        private static readonly List<DivineMessage> DivineMessages = new List<DivineMessage>();

        static DivineMessage()
        {
            RendererManager.LoadImageFromResources(@"others\middle_message.png");
        }

        private static bool isActivate;

        private static bool Activate
        {
            set
            {
                if (isActivate == value || isActivate && DivineMessages.Count > 0)
                {
                    return;
                }

                if (value)
                {
                    RendererManager.Draw += OnDraw;
                }
                else
                {
                    RendererManager.Draw -= OnDraw;
                }

                isActivate = value;
            }
        }

        public static readonly Vector2 DefaultPosition = new Vector2(HUDInfo.ScreenSize.X / 2, HUDInfo.ScreenSize.Y * 0.15f);

        private readonly Vector2 Position;

        private readonly int StayTime;

        private readonly int DecreaseAlpha;

        private readonly Message[] Message;

        public DivineMessage(params Message[] message)
            : this(false, DefaultPosition, 3, 40, message)
        {
        }

        public DivineMessage(bool warn, params Message[] message)
            : this(warn, DefaultPosition, 3, 40, message)
        {
        }

        public DivineMessage(bool warn, Vector2 position, params Message[] message)
            : this(warn, position, 3, 40, message)
        {
        }

        public DivineMessage(bool warn, Vector2 position, int stayTime, params Message[] message)
            : this(warn, position, stayTime, 40, message)
        {
        }

        public DivineMessage(bool warn, Vector2 position, int stayTime, int decreaseAlpha, params Message[] message)
        {
            Position = position;
            StayTime = stayTime;
            DecreaseAlpha = decreaseAlpha;
            Message = message;

            DivineMessages.Add(this);

            MessageAdd(warn);

            Activate = true;
        }

        private float Size;

        private bool IsOldMessage;

        private async void MessageAdd(bool isWarn)
        {
            try
            {
                if (!isWarn)
                {
                    await AnimationMessage();
                }
                else
                {
                    await AnimationWarn();
                }

                var rawGameTime = GameManager.RawGameTime;
                var time = 0.0f;
                do
                {
                    time = GameManager.RawGameTime - rawGameTime;
                    await Task.Delay(100);
                }
                while (time < StayTime);

                do
                {
                    Size -= 5f;
                    await Task.Delay(1);
                }
                while (Size > 5);

                DivineMessages.Remove(this);
                Activate = false;
            }
            catch
            {
                DivineMessages.Remove(this);
                Activate = false;
            }
        }

        private async Task AnimationMessage()
        {
            do
            {
                Size += 5f;
                await Task.Delay(1);
            }
            while (Size < 50 && !IsOldMessage || Size < 30);

            if (!IsOldMessage)
            {
                do
                {
                    Size -= 5f;
                    await Task.Delay(1);
                }
                while (Size > 40);
            }
        }

        private async Task AnimationWarn()
        {
            if (!IsOldMessage)
            {
                for (var i = 0; i < 3; i++)
                {
                    var iSStop = false;

                    do
                    {
                        Size += 5f;

                        if (IsOldMessage && Size > 35)
                        {
                            iSStop = true;
                            break;
                        }

                        await Task.Delay(1);
                    }
                    while (Size < 50);

                    if (iSStop)
                    {
                        break;
                    }

                    do
                    {
                        Size -= 5f;
                        await Task.Delay(1);
                    }
                    while (Size > 40);
                }
            }

        }

        private void DrawDivineMessage()
        {
            var slide = 0;
            var id = DivineMessages.Where(x => x.Position == Position).ToArray();
            var count = id.Count();
            for (var i = 0; i < count; i++)
            {
                if (this != id[i])
                {
                    continue;
                }

                slide = count - 1 - i;
            }

            IsOldMessage = slide > 0;

            var oldMessagePos = Vector2.Zero;
            if (IsOldMessage)
            {
                if (Size > 25)
                {
                    Size -= 0.25f;
                }

                oldMessagePos = new Vector2(0, 20);
            }

            var size = Size * HUDInfo.RatioPercentage;
            var allText = string.Empty;
            foreach (var message in Message)
            {
                allText += message.Text;
            }

            var measureText = RendererManager.MeasureText(allText, "Arial", size);
            var textPosition = new Vector2(measureText.X / 2, (measureText.Y / 2) - (slide * 30 * HUDInfo.RatioPercentage));
            var pos = Position - textPosition;

            if (!IsOldMessage)
            {
                var p = pos - new Vector2(20, 0);
                var s = measureText + new Vector2(40, 0);
                RendererManager.DrawImage(@"others\middle_message.png", new RectangleF(p.X, p.Y, s.X, s.Y));
            }

            var extraPos = Vector2.Zero;
            foreach (var message in Message)
            {
                var color = message.Color;

                color = new Color(color.R, color.G, color.B, 255 - (slide * DecreaseAlpha));

                var text = message.Text;
                RendererManager.DrawText(text, pos + oldMessagePos + extraPos, color, "Arial", size);
                extraPos += new Vector2(RendererManager.MeasureText(text, "Arial", size).X, 0);
            }
        }

        private static void OnDraw()
        {
            foreach (var divineMessage in DivineMessages.ToArray())
            {
                divineMessage.DrawDivineMessage();
            }
        }
    }

    public sealed class Message
    {
        public Message(string text, Color color)
        {
            Text = text;
            Color = color;
        }

        public string Text { get; }

        public Color Color { get; }
    }
}
