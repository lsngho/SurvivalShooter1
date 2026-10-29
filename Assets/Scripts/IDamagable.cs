using UnityEngine;

public interface IDamagable
{
    void Ondamage(float damage, Vector3 hitPoint, Vector3 hitNormal);
}