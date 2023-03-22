// using UnityEngine;
//
// using Bool = System.Boolean;
//
// namespace CGTK.Utils.PluggableStateMachine
// {
//     [CreateAssetMenu]
//     public class State : ScriptableObject
//     {
//         #region Variables
//
//         [SerializeField] private Transition[] transitions; 
//
//         #endregion
//         
//         public Bool IsActive { get; private set; }
//
//         internal virtual void Enter() 
//         {
//             IsActive = true;
//         }
//         
//         internal virtual void Exit() 
//         {
//             IsActive = false;
//         }
//
//         internal virtual void Update() { }
//
//         internal virtual void FixedUpdate() { }
//
//         internal virtual void LateUpdate() { }
//     }
// }
