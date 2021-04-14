using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    // player
    public Transform player;

    // Keys
    readonly KeyCode fireKey = KeyCode.Mouse0;
    readonly KeyCode reloadKey = KeyCode.R;

    // gun properties
    public bool g_Automatic = true;
    public int g_AmmoCapacity = 100;
    public int g_AmmoMagazine = 10;
    private int g_AmmoCurrent = 0;

    // bullet properties
    public float b_speed = 100f;
    public Rigidbody projectile;

    public float g_ShootingSpeed = 1f;

    // Limits
    bool bCanShoot = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
    }

    void GetInputs()
    {
        bool shooting;
        if (g_Automatic) shooting = Input.GetKey(fireKey);
        else shooting = Input.GetKeyDown(fireKey);
        if (shooting) Shoot();

        if (Input.GetKeyDown(reloadKey)) Reload();
    }

    void Shoot()
    {
        if (!bCanShoot) return;

        Rigidbody instantiatedProjectile = Instantiate(projectile, transform.position, transform.rotation);
        instantiatedProjectile.velocity = transform.TransformDirection(new Vector3(0, 0, b_speed));
        Invoke("ResetShoot", g_ShootingSpeed);
    }

    void ResetShoot()
    {
        bCanShoot = true;
    }

    void Reload()
    {
        g_AmmoCapacity -= (g_AmmoMagazine - g_AmmoCurrent);
        g_AmmoCurrent = g_AmmoMagazine;
    }
}
