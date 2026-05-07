//namespace DotaMapPlus;

//using System;

//using Divine.GameConsole;
//using Divine.Input;
//using Divine.Input.EventArgs;
//using Divine.Menu.EventArgs;
//using Divine.Menu.Items;

//internal sealed class ZoomHack
//{
//    private const int DefaultZoomValue = 1200;

//    private const int MaxZoomValue = 9000;

//    private const int MinZoomValue = 1134;

//    private readonly MenuHoldKey ZoomKeyItem;

//    private readonly MenuSlider ZoomSliderItem;

//    public ZoomHack(RootMenu rootMenu)
//    {
//        var zoomHackMenu = rootMenu.CreateMenu("Zoom Hack");
//        ZoomKeyItem = zoomHackMenu.CreateHoldKey("Key", Key.LeftCtrl);
//        ZoomSliderItem = zoomHackMenu.CreateSlider("Camera Distance", DefaultZoomValue, MinZoomValue, MaxZoomValue);

//        GameConsoleManager.SetValue("r_farz", 18000);

//        ZoomSliderItem.ValueChanged += OnZoomSliderValueChanged;
//        InputManager.MouseWheel += OnInputManagerMouseWheel;

//        GameConsoleManager.ExecuteCommand("dota_camera_disable_zoom true");
//    }

//    public void Dispose()
//    {
//        GameConsoleManager.ExecuteCommand("dota_camera_disable_zoom false");

//        GameConsoleManager.SetValue("dota_camera_distance", DefaultZoomValue);

//        ZoomSliderItem.ValueChanged -= OnZoomSliderValueChanged;
//        InputManager.MouseWheel -= OnInputManagerMouseWheel;
//    }

//    private void OnZoomSliderValueChanged(MenuSlider slider, SliderEventArgs e)
//    {
//        GameConsoleManager.SetValue("dota_camera_distance", e.NewValue);
//    }

//    private void OnInputManagerMouseWheel(MouseWheelEventArgs e)
//    {
//        if (ZoomKeyItem)
//        {
//            var zoomValue = GameConsoleManager.GetInt32("dota_camera_distance");

//            if (e.Up)
//            {
//                zoomValue -= 50;
//                zoomValue = Math.Max(zoomValue, MinZoomValue);
//            }
//            else
//            {
//                zoomValue += 50;
//                zoomValue = Math.Min(zoomValue, MaxZoomValue);
//            }

//            ZoomSliderItem.Value = zoomValue;
//        }
//    }
//}