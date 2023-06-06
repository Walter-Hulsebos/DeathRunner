namespace DeathRunner.Attributes.Modifiers
{
    public interface IModSubPercentage<T> : IMod<T>
    {
        public T Percentage { get; }
    }
}