namespace DeathRunner.Attributes.Modifiers
{
    public interface IModAddValue<T> : IMod<T>
    {
        public T Value { get; }
    }
}