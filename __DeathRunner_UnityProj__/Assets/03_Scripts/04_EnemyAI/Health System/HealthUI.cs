using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Damageable
{
    public class HealthUI : MonoBehaviour
    {
        private Image image;

        private void Start()
        {
            image = GetComponent<Image>();
        }

        public void UpdateHealthUI(Damageable damageable)
        {
            float fillAmount = damageable.currentHitPoints / (1.0f *damageable.maxHitPoints);
            image.fillAmount = fillAmount;
        }
    }
}
