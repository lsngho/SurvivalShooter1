using System.Collections;
using UnityEngine;
using UnityEngine.UnityConsent;

public class Gun : MonoBehaviour
{
    public Transform fireTransfrom;
    public ParticleSystem muzzleFlashEffect;
    private LineRenderer bulletLineRenderer;
    private AudioSource gunAudioPlayer;
    public GunData gunData;
    private float fireDistance = 50f;
    private float lastFireTime;

    private void Awake()
    {
        bulletLineRenderer = GetComponent<LineRenderer>();
        gunAudioPlayer = GetComponent<AudioSource>();

        bulletLineRenderer.positionCount = 2;
        bulletLineRenderer.enabled = false;
    }

    private void OnEnable()
    {
        lastFireTime = 0f;
    }
   public void Fire()
{
    if (Time.time >= lastFireTime + gunData.timeBetFire)
    {
        lastFireTime = Time.time;
        Shoot();
    }
}

    public LayerMask targetLayer;
    private void Shoot()
    {
        Vector3 hitPosition;

        Ray ray = new Ray(fireTransfrom.position, fireTransfrom.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, fireDistance, targetLayer))
        {
            hitPosition = hit.point;

            IDamagable target = hit.collider.GetComponent<IDamagable>();
            if (target != null)
            {
                target.Ondamage(gunData.damage, hit.point, hit.normal);
            }
        }
        else
        {
            hitPosition = fireTransfrom.position + fireTransfrom.forward * fireDistance;
        }
        StartCoroutine(ShotEffect(hitPosition));
    }

    private IEnumerator ShotEffect(Vector3 hitPosition)
    {
        muzzleFlashEffect.Play();
        gunAudioPlayer.PlayOneShot(gunData.shotClip);

        bulletLineRenderer.SetPosition(0, fireTransfrom.position);
        bulletLineRenderer.SetPosition(1, hitPosition);
        bulletLineRenderer.enabled = true;
        yield return new WaitForSeconds(0.03f);

        bulletLineRenderer.enabled = false;
    }
}