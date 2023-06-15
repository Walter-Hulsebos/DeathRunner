using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DeathRunner
{
    public class CardSelectionManager : MonoBehaviour
    {
        public static CardSelectionManager instance;

        public GameObject[] Buttons;
        
        public GameObject LastSelected { get; set; }
        public int LastSelectedIndex { get; set; }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        private void OnEnable()
        {
            StartCoroutine(SetSelectedAfterOneFrame());
        }

        private void Update()
        {
            // If we move Down
            if (InputManager.instance.NavigationInput.y < 0)
            {
                // Select next Button
                HandleNextCardSelection(1);
            }
            
            
            // If we move Up
            if (InputManager.instance.NavigationInput.y > 0)
            {
                // Select privious Button
                HandleNextCardSelection(-1);
            }
            
        }

        private IEnumerator SetSelectedAfterOneFrame()
        {
            yield return null;
            EventSystem.current.SetSelectedGameObject(Buttons[0]);
        }

        private void HandleNextCardSelection(int addition)
        {
            if (EventSystem.current.currentSelectedGameObject == null && LastSelected != null)
            {
                int newIndex = LastSelectedIndex + addition;
                newIndex = Mathf.Clamp(newIndex, 0, Buttons.Length - 1);
                EventSystem.current.SetSelectedGameObject(Buttons[newIndex]);
            }
        }
    }
}
