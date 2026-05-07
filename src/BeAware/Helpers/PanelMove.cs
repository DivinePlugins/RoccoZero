namespace BeAware.Helpers;

using System;
using System.Threading.Tasks;

using Divine.Extensions;
using Divine.Game;
using Divine.Helpers;
using Divine.Input;
using Divine.Input.EventArgs;
using Divine.Numerics;
using Divine.Common.Log;

public class PanelMove
{
    public Vector2 Position { get; set; }

    public Vector2 Size { get; set; }

    public PanelMove(Vector2 position = default, Vector2 size = default)
    {
        Position = position;
        Size = size;
    }

    private bool IsActivateMove { get; set; }

    public bool ActivateMove
    {
        get
        {
            return IsActivateMove;
        }

        set
        {
            if (value == IsActivateMove)
            {
                return;
            }

            if (value)
            {
                InputManager.MouseKeyDown += OnMouseKeyDown;
                InputManager.MouseKeyUp += OnMouseKeyUp;
                InputManager.MouseMove += OnMouseMove;
                DisableTime();
                LogManager.Info("Activate Move");
            }
            else
            {
                InputManager.MouseKeyDown -= OnMouseKeyDown;
                InputManager.MouseKeyUp -= OnMouseKeyUp;
                InputManager.MouseMove -= OnMouseMove;
                LogManager.Info("Deactivate Move");
            }

            IsActivateMove = value;
        }
    }

    public delegate void MoveChanged(bool isTimeout, Vector2 position);

    public event MoveChanged ValueChanged;

    private float LastTime { get; set; }

    public int Time { get; private set; }

    private async void DisableTime()
    {
        LastTime = GameManager.RawGameTime;
        do
        {
            Time = 10 - (int)(GameManager.RawGameTime - LastTime);
            await Task.Delay(150);
        }
        while (GameManager.RawGameTime - LastTime < 10 && IsActivateMove);

        ValueChanged?.Invoke(true, Position);
        if (ActivateMove)
        {
            ActivateMove = false;
            LogManager.Info("Time is Over Move");
        }
    }

    private bool PanelDragged { get; set; }

    private Vector2 DragMouseDiff { get; set; }

    private void OnMouseKeyDown(MouseEventArgs e)
    {
        if (e.MouseKey != MouseKey.Left)
        {
            return;
        }

        if (e.Position.IsUnderRectangle(new RectangleF(Position.X - 5, Position.Y - 5, Size.X, Size.Y)))
        {
            DragMouseDiff = e.Position - Position;

            PanelDragged = true;
        }
    }

    private void OnMouseKeyUp(MouseEventArgs e)
    {
        if (e.MouseKey != MouseKey.Left)
        {
            return;
        }

        PanelDragged = false;
    }

    private void OnMouseMove(MouseMoveEventArgs e)
    {
        if (!PanelDragged)
        {
            return;
        }

        var position = e.Position - DragMouseDiff;
        position.X = Math.Max(5, Math.Min(HUDInfo.ScreenSize.X - (Size.X - 5), position.X));
        position.Y = Math.Max(5, Math.Min(HUDInfo.ScreenSize.Y - (Size.Y - 5), position.Y));
        Position = position;

        LastTime = GameManager.RawGameTime;
        ValueChanged?.Invoke(false, Position);
    }
}