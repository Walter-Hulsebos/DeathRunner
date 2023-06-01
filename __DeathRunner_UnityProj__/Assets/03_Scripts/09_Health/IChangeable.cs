using System;
using GenericScriptableArchitecture;
using Bool = System.Boolean;

namespace DeathRunner.Attributes
{
    using Modifiers;
    
    public interface IChangeable<T> where T : struct, IConvertible, IComparable, IFormattable
    {
        public Constant<T>           Max         { get; }
        public Reference<Bool>       UseInfinity { get; }
        public T                     Value       { get; set; }


        //public IMod<T>[]             Modifiers   { get; }
        
        public EventReference<T, T> OnChanged   { get; }
        public EventReference<T, T> OnDecreased { get; }
        public EventReference       OnDepleted  { get; }
        public EventReference<T, T> OnIncreased { get; }

        public Bool                 IsZero      { get; }
    }
}