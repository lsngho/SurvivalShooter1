using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : LivingEntity
{
    public readonly static int IdDie = Animator.StringToHash("Die");

    public Image uiHealth;

    public AudioClip hitClip;
    public AudioClip deathClip;

    private AudioSource playerAudioSource;
    private Animator playerAnimator;

    private PlayerMovement plyaerMovement;
    private PlayerInput playerInput;

    private void Awake()
    {
        playerAudioSource = GetComponent<AudioSource>();
        playerAnimator = GetComponent<Animator>();
        plyaerMovement = GetComponent<PlayerMovement>();
        playerInput = GetComponent<PlayerInput>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        plyaerMovement.enabled = true;
        playerInput.enabled = true;

        uiHealth.gameObject.SetActive(true);
        uiHealth.fillAmount = Health / startingHealth;
    }

    public override void Ondamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!IsDead)
        {
            playerAudioSource.PlayOneShot(hitClip);
        }
        base.Ondamage(damage, hitPoint, hitNormal);
        uiHealth.fillAmount = Health / startingHealth;
    }
    public override void Die()
    {
        base.Die();
        playerAnimator.SetTrigger(IdDie);
        playerAudioSource.PlayOneShot(deathClip);

        plyaerMovement.enabled = false;
        playerInput.enabled = false;

        uiHealth.gameObject.SetActive(false);
    }


}