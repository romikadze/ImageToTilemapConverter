using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ImageToTilemapConverter.Scripts
{
    [RequireComponent(typeof(Converter))]
    public class ConverterGizmosDrawer : MonoBehaviour
    {
        [Range(1, 20)][SerializeField] private float _gizmosRadiusMultiplier;
        private Converter _converter;
        
        public void OnDrawGizmosSelected()
        {
            SetupConverter();
            
            foreach (var data in _converter.GizmosData)
            {
                Gizmos.color = data.Value;
                Gizmos.DrawSphere(data.Key, _converter.ColorPickRadius * _gizmosRadiusMultiplier);
            }
        }

        private void SetupConverter()
        {
            if (_converter != null) return;

            _converter = GetComponent<Converter>();
            Debug.Log("Load");
        }
    }
}