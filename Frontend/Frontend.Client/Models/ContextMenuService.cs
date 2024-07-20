using Frontend.Client.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

public static class ContextMenuService
{
    // make a targetUser property
    public static string? TargetUser { get; set; }
    public static double Top { get; set; }
    public static double Left { get; set; }

    public static event Action? OnChange;
    public static bool IsBeingShown { get; private set; }

    public static async Task ShowContextMenu(MouseEventArgs e, IJSRuntime jsRuntime)
    {
        Top = e.ClientY;
        Left = e.ClientX;

        double pageWidth = await jsRuntime.InvokeAsync<double>("getPageWidth");
        double offset = ContextMenu.BoxWidth - (pageWidth - e.ClientX);

        if (offset > 0)
        {
            Left -= offset;
        }

        IsBeingShown = true;
        OnChange?.Invoke();
    }

    public static void HideContextMenu()
    {
        IsBeingShown = false;
        OnChange?.Invoke();
    }
}
