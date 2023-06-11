using UnityEngine;

namespace QFSW.QC.UI
{
    [ExecuteInEditMode]
    public class BlurShaderController : MonoBehaviour
    {
        [SerializeField] private Material _blurMaterial = null;
        [SerializeField] private float _blurRadius = 1f;
        [SerializeField] private Vector2 _referenceResolution = new(1920, 1080);

        private void LateUpdate()
        {
            if (_blurMaterial)
            {
                Vector2 resolution = new(Screen.width, Screen.height);
                float correction = resolution.y / _referenceResolution.y;
                _blurMaterial.SetFloat("_Radius", _blurRadius);
                _blurMaterial.SetFloat("_BlurMultiplier", correction);
            }
        }
    }
}
