namespace Frontend.Client.Models
{
    public static class ContextMenuModalService
    {
        public static bool modalShown { get; private set; }
        public static event Action? OnModalChange;
        public static string? Type;

        public static void showModal()
        {
            modalShown = true;
            OnModalChange?.Invoke();
        }

        public static void hideModal()
        {
            modalShown = false;
            OnModalChange?.Invoke();
        }
    }
}
