using UnityEngine;

public class Hellephant : Monster
{
    protected override void OnEnable()
    {
        base.OnEnable();
        startingHealth *= 2f;

        damage *= 0.8f;
        agent.speed *= 0.7f;
    }
}
