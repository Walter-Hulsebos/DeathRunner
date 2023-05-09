using System;
using GenericScriptableArchitecture;

using Bool = System.Boolean;

namespace DeathRunner.Health
{
    public interface IChangeable<T> where T : struct, IConvertible, IComparable, IFormattable
    {
        public Constant<T>           Max     { get; }
        public T                     Current { get; }
        
        public ScriptableEvent<T, T> OnChanged   { get; }
        public ScriptableEvent       OnDecreased { get; }
        public ScriptableEvent       OnIncreased { get; }
        public ScriptableEvent       OnDepleted  { get; }
        
        public Bool                  IsZero      { get; }
    }
}