using System;
using UnityEngine;
using UnityEngine.AI;

public class Monster : LivingEntity
{
    public enum Status
    {
        Idle,
        Trace,
        Attack,
        Die,
    }

    public Transform target;
    public LayerMask targetLayer;

    public float traceDistance = 10f;
    public float monsterAttackDistance = 1f;
    private float attackInterval = 1f;

    public ParticleSystem bloodEffect;
    public Collider monsterCollider;
    public AudioClip hitClip;
    public AudioClip deathClip;
    protected NavMeshAgent agent;
    protected Animator monsterAnimator;
    protected AudioSource monsterAudioSource;
    public float damage = 10f;
    protected float lastAttackTime;
    protected Status currentStatus;
    public static readonly int IdHasTarget = Animator.StringToHash("HasTarget");
    public static readonly int IdDie = Animator.StringToHash("Die");
    public Status CurrentStatus
    {
        get => currentStatus;
        set
        {
            currentStatus = value;
            Debug.Log(currentStatus);
            switch (currentStatus)
            {
                case Status.Idle:
                    monsterAnimator.SetBool(IdHasTarget, false);
                    agent.isStopped = true;
                    break;
                case Status.Trace:
                    monsterAnimator.SetBool(IdHasTarget, true);
                    agent.isStopped = false;
                    break;
                case Status.Attack:
                    agent.isStopped = true;
                    monsterAnimator.SetBool(IdHasTarget, false);
                    monsterAudioSource.PlayOneShot(hitClip);
                    break;
                case Status.Die:
                    agent.isStopped = true;
                    monsterAnimator.SetTrigger(IdDie);
                    monsterCollider.enabled = false;
                    monsterAudioSource.PlayOneShot(deathClip);

                    break;
            }
        }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        monsterAnimator = GetComponent<Animator>();
        monsterAudioSource = GetComponent<AudioSource>();
    }
    protected override void OnEnable()
    {
        base.OnEnable();

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
        monsterCollider.enabled = true;
        CurrentStatus = Status.Idle;
    }

    private void OnDisable()
    {
        agent.enabled = true;
        if (agent.isOnNavMesh)
        {
            agent.ResetPath();
        }
    }

    private void Update()
    {
        switch (CurrentStatus)
        {
            case Status.Idle:
                UpdateIdle();
                break;
            case Status.Trace:
                UpdateTrace();
                break;
            case Status.Attack:
                UpdateAttack();
                break;
            case Status.Die:
                UpdateDie();
                break;
        }
    }

    private void UpdateDie()
    {

    }

    private void UpdateAttack()
    {
        if (target == null || Vector3.Distance(transform.position, target.position) > monsterAttackDistance)
        {
            CurrentStatus = Status.Trace;
            return;
        }
        if (Time.time >= lastAttackTime + attackInterval)
        {
            lastAttackTime = Time.time;
            IDamagable damagable = target.GetComponent<IDamagable>();
            var hitNormal = (transform.position - target.position).normalized;
            damagable.Ondamage(damage, target.position, hitNormal);
        }
    }

    private void UpdateTrace()
    {
        if (target != null && Vector3.Distance(transform.position, target.position) < monsterAttackDistance)
        {
            CurrentStatus = Status.Attack;
            return;
        }

        if (target == null || Vector3.Distance(transform.position, target.position) > traceDistance)
        {
            target = null;
            CurrentStatus = Status.Idle;
            return;
        }
        agent.SetDestination(target.position);
    }

    private void UpdateIdle()
    {
        if (target != null && Vector3.Distance(transform.position, target.position) < traceDistance)
        {
            CurrentStatus = Status.Trace;
            return;
        }

        target = FindTarget(traceDistance);
    }

    private Transform FindTarget(float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, targetLayer);
        Transform closet = null;
        float closetDistance = float.MaxValue;
        for (int i = 0; i < colliders.Length; ++i)
        {
            float distance = Vector3.Distance(transform.position, colliders[i].transform.position);
            if (distance < closetDistance)
            {
                closetDistance = distance;
                closet = colliders[i].transform;
            }
        }
        return closet;
    }

    public override void Ondamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (IsDead)
        {
            return;
        }
        base.Ondamage(damage, hitPoint, hitNormal);

        monsterAudioSource.PlayOneShot(hitClip);
        bloodEffect.transform.position = hitPoint;
        bloodEffect.transform.forward = hitNormal;
        bloodEffect.Play();
    }

    public override void Die()
    {
        base.Die();

        CurrentStatus = Status.Die;
    }
}