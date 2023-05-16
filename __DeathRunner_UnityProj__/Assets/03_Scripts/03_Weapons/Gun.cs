using UnityEngine;
using UnityEngine.InputSystem;

using UltEvents;
using static UnityEngine.Time;

public class Gun : MonoBehaviour
{
    [SerializeField] private Transform muzzle;

    [SerializeField] protected WeaponSO weaponSettings;
    
    [SerializeField] private InputActionReference attackInput;
        
    [SerializeField] private UltEvent onAttack;

    [SerializeField] private Camera mainCamera;

    //This code is for the new InputSystem
    /*
    private LookAround actions;

    public void Start()
    {
        actions = transform.root.GetComponent<LookAround>();
    }
    */
    //private LookWithMouse _actions;
    //private PlayerMovement _inputActions;
    
    //InputAction _isShooting;

    private bool _isShooting;
    
    
    
    private void OnEnable()
    {
        attackInput.action.Enable();
    }
        
    private void OnDisable()
    {
        attackInput.action.Disable();
    }

    private void Reset()
    {
        muzzle = transform.Find("Muzzle");

        if (muzzle == null)
        {
            muzzle = new GameObject(name: "Muzzle").transform;
            muzzle.parent = transform;
        }
    }
    /*
    public void Start()
    {
        _actions = transform.root.GetComponent<LookWithMouse>();
        _inputActions = transform.root.GetComponent<PlayerMovement>();
    }
    */
    private float _nextTimeToFire = float.NegativeInfinity;
    private bool CanFire => (time >= _nextTimeToFire);

    public void Update()
    {
        if (!CanFire) return;

        if (weaponSettings.shotType == WeaponSO.ShotTypeTutorial.Auto)
        {
            if (attackInput.action.IsPressed())
            {
                Fire(muzzle);   
            }
        }
        else if (weaponSettings.shotType == WeaponSO.ShotTypeTutorial.Single)
        {
            if (attackInput.action.WasPressedThisFrame())
            {
                Fire(muzzle);
            }
        }
    }

    public GameObject Fire(Transform origin)
    {
        GameObject __projectile = Instantiate(
            original: weaponSettings.bulletPrefab,
            position: origin.position,
            rotation: origin.rotation,
            parent:   null
        );
        print("Lookin");
        _nextTimeToFire = time + (1f / weaponSettings.fireRate);
        
        return __projectile;
    }

    //public abstract void Shoot();
}
