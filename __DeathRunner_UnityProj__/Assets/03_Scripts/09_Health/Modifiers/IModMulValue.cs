namespace DeathRunner.Attributes.Modifiers
{
    public interface IModMulValue<T> : IMod<T>
    {
        public T Value { get; }
    }
}