namespace DeathRunner.Attributes.Modifiers
{
    public interface IModAddPercentage<T> : IMod<T>
    {
        public T Percentage { get; }
    }
}
    