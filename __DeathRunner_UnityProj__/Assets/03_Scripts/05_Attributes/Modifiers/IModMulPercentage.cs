namespace DeathRunner.Attributes.Modifiers
{
    public interface IModMulPercentage<T> : IMod<T>
    {
        public T Percentage { get; }
    }
}