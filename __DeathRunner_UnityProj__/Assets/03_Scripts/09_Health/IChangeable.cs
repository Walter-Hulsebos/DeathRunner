using System;
using GenericScriptableArchitecture;
using Bool = System.Boolean;

namespace DeathRunner.Attributes
{
    using Modifiers;
    
    public interface IChangeable<T> where T : struct, IConvertible, IComparable, IFormattable
    {
        public Constant<T>           Max     { get; }
        public T                     Value   { get; set; }
        
                
        public IMod<T>[]             Modifiers   { get; }
        
        public ScriptableEvent<T, T> OnChanged   { get; }
        public ScriptableEvent       OnDecreased { get; }
        public ScriptableEvent       OnIncreased { get; }
        public ScriptableEvent       OnDepleted  { get; }
        
        public Bool                  IsZero      { get; }
    }
}