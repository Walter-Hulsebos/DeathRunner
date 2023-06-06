namespace DeathRunner.Attributes.Modifiers
{
    public interface IModSubValue<T> : IMod<T>
    {
        public T Value { get; }
    }
}