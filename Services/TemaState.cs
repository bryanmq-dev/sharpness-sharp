namespace sharpness_sharp.Services
{
    public class TemaState
    {
        public string Color { get; private set; } = "#4a2f20";

        public event Action? OnCambio;

        public void Cambiar(string color)
        {
            Color = color;
            OnCambio?.Invoke();
        }
    }
}
