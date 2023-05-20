// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// using F32 = System.Single;
// using U16 = System.UInt16;
//
// namespace DeathRunner
// {
//     public sealed class Flicker : MonoBehaviour
//     {
//         [SerializeField] private Renderer renderer;
//         
//         [SerializeField] private F32 duration = 0.15f;
//         
//         [Tooltip("The frequency at which to flicker.")]
//         [SerializeField] private F32 octave = 0.04f;
//
//         [SerializeField] private Color color = Color.white;
//         
//         [SerializeField] private U16[] materialIndices = {0};
//         
//         private Color[] _initialFlickerColors;
//
//         public void Invoke()
//         {
//             foreach (U16 __materialIndex in materialIndices)
//             {
//                 StartCoroutine(FlickerCoroutine(materialIndex: __materialIndex, initialColor: renderer.materials[__materialIndex].color, flickerColor: color, flickerSpeed: octave, flickerDuration: duration));
//             }
//         }
//
//         private void OnEnable()
//         {
//             _initialFlickerColors = new Color[materialIndices.Length];
//             
//             foreach (U16 __materialIndex in materialIndices)
//             {
//                 _initialFlickerColors[__materialIndex] = renderer.materials[__materialIndex].color;
//             }
//         }
//
//         private IEnumerator FlickerCoroutine(U16 materialIndex, Color initialColor, Color flickerColor, F32 flickerSpeed, F32 flickerDuration)
//         {
//             // float __startTime = Time.time;
//             //
//             // while (Time.time - __startTime < duration)
//             // {
//             //     renderer.material.color = color * Mathf.Sin(Time.time * octave);
//             //     yield return null;
//             // }
//             
//             if (renderer == null)
//             {
//                 yield break;
//             }
//
//             if (!_propertiesFound[materialIndex])
//             {
//                 yield break;
//             }
//
//             if (initialColor == flickerColor)
//             {
//                 yield break;
//             }
//
//             float flickerStop = FeedbackTime + flickerDuration;
//             IsPlaying = true;
//             
//             while (FeedbackTime < flickerStop)
//             {
//                 SetColor(materialIndex, flickerColor);
//                 if (Timing.TimescaleMode == TimescaleModes.Scaled)
//                 {
//                     yield return MMFeedbacksCoroutine.WaitFor(flickerSpeed);
//                 }
//                 else
//                 {
//                     yield return MMFeedbacksCoroutine.WaitForUnscaled(flickerSpeed);
//                 }
//                 SetColor(materialIndex, initialColor);
//                 if (Timing.TimescaleMode == TimescaleModes.Scaled)
//                 {
//                     yield return MMFeedbacksCoroutine.WaitFor(flickerSpeed);
//                 }
//                 else
//                 {
//                     yield return MMFeedbacksCoroutine.WaitForUnscaled(flickerSpeed);
//                 }
//             }
//
//             SetColor(materialIndex, initialColor);
//             IsPlaying = false;
//         }
//         
//         protected void SetColor(int materialIndex, Color color)
//         {
//             // if (!_propertiesFound[materialIndex])
//             // {
//             //     return;
//             // }
//             //
//             // if (UseMaterialPropertyBlocks)
//             // {
//             //     BoundRenderer.GetPropertyBlock(_propertyBlock);
//             //     _propertyBlock.SetColor(_colorPropertyName, color);
//             //     BoundRenderer.SetPropertyBlock(_propertyBlock, materialIndex);
//             // }
//             // else
//             // {
//             //     BoundRenderer.materials[materialIndex].color = color;
//             // }
//             
//             // else
//             // {
//             //     if (UseMaterialPropertyBlocks)
//             //     {
//             //         BoundRenderer.GetPropertyBlock(_propertyBlock);
//             //         _propertyBlock.SetColor(_propertyIDs[materialIndex], color);
//             //         BoundRenderer.SetPropertyBlock(_propertyBlock, materialIndex);
//             //     }
//             //     else
//             //     {
//             //         BoundRenderer.materials[materialIndex].SetColor(_propertyIDs[materialIndex], color);
//             //     }
//             // }            
//         }
//         
//         //private 
//     }
// }
