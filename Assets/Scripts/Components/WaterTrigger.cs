using System;
using UnityEngine;

namespace Assets.Scripts.Components
{
    class WaterTrigger: MonoBehaviour
    {
        public Action<Water> OnWaterEnter { get; set; }
        public Action OnWaterExit { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Water"))
            {
                var water = other.GetComponent<Water>();
                OnWaterEnter?.Invoke(water);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Water"))
            {
                OnWaterExit?.Invoke();
            }
        }
    }
}
