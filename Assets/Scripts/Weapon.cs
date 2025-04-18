using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon : MonoBehaviour
{
    public bool isActiveWeapon;
    public int weaponDamage;

 
    [Header("Shooting")]
    public bool isShooting, readyShoot;

    private bool allowReset = true;

    public float shootingDelay = 0.1f;


    [Header("Burst")]
    public int bulletsPerBurst = 3;

    private int burstBulletsLeft;


    [Header("Spread")]
    public float spreadIntensity ;

    public float hipSpreadIntensity ;

    public float adsSpreadIntensity ;


    [Header("Bullet")]

    public GameObject bulletPrefab;

    public Transform bulletSpawn;

    public float bulletVelocity = 30;

    public float bulletPrefabLifeTime = 3f;

    public  GameObject muzzleEffect;

    internal Animator animator;

    [Header("Loading")]

    public float reloadTime;

    public int magazineSize, bulletsLeft;

    public bool isReloading;

    public Vector3 spawnPosition ;

    public Vector3 spawnRotation;

    bool isADS;


public enum WeaponModel{
    Pistol1911,
    M16
}
public WeaponModel thisWeaponModel;
    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }
    public ShootingMode currentShootingMode;

    private void Awake()
    {
        readyShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        animator = GetComponent<Animator>();
        bulletsLeft = magazineSize;
        spreadIntensity = hipSpreadIntensity;
    }

    void Update()
    {

        if (isActiveWeapon){

            Debug.Log("Children count: " + transform.childCount);
          foreach (Transform child in transform)
          {
          Debug.Log("Setting layer for " + child.name);
          child.gameObject.layer = LayerMask.NameToLayer("WeaponRender");
          }


            if(Input.GetMouseButtonDown(1)){
                EnterADS();
            }
            if(Input.GetMouseButtonUp(1)){
                ExitADS();
            }
            

            GetComponent<Outline>().enabled=false;
            if (bulletsLeft == 0 && isShooting){
            SoundManager.Instance.emptyManagizeSound1911.Play();
         }
            if (currentShootingMode == ShootingMode.Auto)
          {
            isShooting = Input.GetKey(KeyCode.Mouse0);
         }
            else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
         {
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
         }
            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloading == false && WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > 0){
             Reload();
         }
            if(readyShoot && isShooting == false && isReloading == false && bulletsLeft <= 0){

         }
            if (readyShoot && isShooting && bulletsLeft > 0)
           {
            burstBulletsLeft = bulletsPerBurst;
            FireWeapon();
           }
        
          }
        else {
            foreach (Transform child in transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Default");
            }
        }
      
    }
private void EnterADS(){
    animator.SetTrigger("enterADS");
    isADS=true;
    HUDManager.Instance.middleDot.SetActive(false);
    spreadIntensity = adsSpreadIntensity;

}

private void ExitADS(){
    animator.SetTrigger("exitADS");
                isADS=false;
                HUDManager.Instance.middleDot.SetActive(true);
                spreadIntensity = hipSpreadIntensity;
} 
    private void FireWeapon()
    {
        bulletsLeft--;

        muzzleEffect.GetComponent<ParticleSystem>().Play();
        
        if(isADS){
            //RECOIL_ADS
            animator.SetTrigger("RECOIL_ADS");

        }
         else{
              animator.SetTrigger("RECOIL");
         }
        

       // SoundManager.Instance.shootingSound1911.Play();

        SoundManager.Instance.PlayShootingSound(thisWeaponModel);

        readyShoot = false;

        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        Bullet bul =bullet.GetComponent<Bullet>();
        bul.bulletDamage = weaponDamage;

        bullet.transform.forward = shootingDirection;

        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;

            Invoke("FireWeapon", shootingDelay);
        }
    }
private void Reload(){
    //SoundManager.Instance.reloadingSound1911.Play();
    SoundManager.Instance.PlayReloadSound(thisWeaponModel);
    animator.SetTrigger("RELOAD");
    isReloading = true;
    Invoke("ReloadCompleted" , reloadTime);
}
private void ReloadCompleted(){
    
if(WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > magazineSize){
    bulletsLeft=magazineSize;
    WeaponManager.Instance.DecreaseTotalAmmo(bulletsLeft , thisWeaponModel);
}
else{
    bulletsLeft=WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel);
    WeaponManager.Instance.DecreaseTotalAmmo(bulletsLeft , thisWeaponModel);
}

    isReloading = false;
}
    private void ResetShot()
    {
        readyShoot = true;
        allowReset = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;
        float z = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        return direction + new Vector3(0, y, z);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float bulletPrefabLifeTime)
    {
        yield return new WaitForSeconds(bulletPrefabLifeTime);
        Destroy(bullet);
    }
}



