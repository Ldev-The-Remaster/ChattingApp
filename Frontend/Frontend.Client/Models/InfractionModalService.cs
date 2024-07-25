namespace Frontend.Client.Models
{
    public static class InfractionModalService
    {
        public static bool modalShown { get; private set; }
        public static event Action? OnModalChange;
        public static string? InfractionType;

        public static void showInfractionModal()
        {
            modalShown = true;
            OnModalChange?.Invoke();
        }

        public static void hideInfractionModal()
        {
            modalShown = false;
            OnModalChange?.Invoke();
        }
    }
}
