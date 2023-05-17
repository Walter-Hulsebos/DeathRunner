using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
[InlineEditor(Expanded = true)]
#endif
[CreateAssetMenu(menuName = "Weapon SO", order = 1)]
public class WeaponSO : ScriptableObject, ISerializationCallbackReceiver
{
    //public string weaponName;
    public ShotTypeTutorial shotType;
    public float fireRate;

    [SerializeField]
    public GameObject bulletPrefab;
    
    public void Init()
    {
        
    }
    
    public void OnBeforeSerialize()
    {
        Init();
    }

    public void OnAfterDeserialize()
    {
        
    }

    public enum WeaponTypeTutorial
    {
        Ar,
        Smg,
        Pistol,
        Shotgun,
        Sniper
    }
    
    public enum ShotTypeTutorial
    {
        Auto,
        Single,
        Burst
    }
}
