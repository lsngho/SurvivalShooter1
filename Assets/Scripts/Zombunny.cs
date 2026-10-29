using UnityEngine;

public class Zombunny : Monster
{
    protected override void OnEnable()
    {
        base.OnEnable();
        startingHealth *= 0.6f;

        damage *= 0.5f;
        agent.speed *= 1.3f;
    }
}
