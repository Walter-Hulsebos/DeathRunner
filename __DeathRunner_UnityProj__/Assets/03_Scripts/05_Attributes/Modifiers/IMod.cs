namespace DeathRunner.Attributes.Modifiers
{
    public interface IMod<T>
    {
        T ApplyTo(T value);
    }
}
