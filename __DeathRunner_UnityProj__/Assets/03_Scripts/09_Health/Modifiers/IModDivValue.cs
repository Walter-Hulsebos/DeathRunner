namespace DeathRunner.Attributes.Modifiers
{
    public interface IModDivValue<T> : IMod<T>
    {
        public T Value { get; }
    }
}