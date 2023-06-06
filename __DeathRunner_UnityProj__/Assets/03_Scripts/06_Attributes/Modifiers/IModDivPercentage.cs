namespace DeathRunner.Attributes.Modifiers
{
    public interface IModDivPercentage<T> : IMod<T>
    {
        public T Percentage { get; }
    }
}