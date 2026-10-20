namespace sharpness_sharp.Services
{
    public abstract class Contador
    {
        public Guid Id { get; } = Guid.NewGuid();
        public int Valor { get; private set; }

        public void Sumar() => Valor++;
    }

    public class ContadorSingleton : Contador { }

    public class ContadorScoped : Contador { }

    public class ContadorTransient : Contador { }
}
