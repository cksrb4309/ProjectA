using System.Collections;
using UnityEngine;

public class ArmoroftheVoid : StackEffect
{
    public PlayerController pc;

    public ShieldGroup shield;

    public override void Play()
    {
        shield.enabled = true;
        StartCoroutine(ShieldCoroutine());
    }
    IEnumerator ShieldCoroutine()
    {
        pc.shield = true;
        shield.Enable();
        yield return new WaitForSeconds(1.5f);
        shield.Diasable();
        yield return new WaitForSeconds(0.5f);
        pc.shield = false;
        shield.enabled = false;
    }
}
