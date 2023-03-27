// using System;
//
// using Bool = System.Boolean;
//
// using CGTK.Utils.SerializableCallbacks;
//
// namespace CGTK.Utils.PluggableStateMachine
// {
//     public sealed class Transition : ScriptableObject
//     {
//         [field:SerializeField] public State From { get; private set; }
//         [field:SerializeField] public State To   { get; private set; }
//         
//         protected Func<Bool>[] conditions;
//
//         internal Bool ConditionsAreMet
//         {
//             get
//             {
//                 foreach (Func<Boolean> __condition in conditions)
//                 {
//                     if (__condition.Invoke() == false)
//                     {
//                         return false;
//                     }
//                 }
//             
//                 return true;   
//             }
//         }
//     }
//
//     [Serializable]
//     public struct Transition
//     {
//         //public State from;
//         public State to;
//         
//         //public Func<Bool>[] conditions;
//
//         public SerializableFunc<Bool>[] conditions;
//         
//         internal Bool ConditionsAreMet
//         {
//             get
//             {
//                 foreach (SerializableFunc<Bool> __condition in conditions)
//                 {
//                     if (__condition.Invoke() == false)
//                     {
//                         return false;
//                     }
//                 }
//             
//                 return true;   
//             }
//         }
//     }
// }
