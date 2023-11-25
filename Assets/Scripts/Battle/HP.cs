using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP : MonoBehaviour
{
    [SerializeField] GameObject hp;

    public void Set(float hp)
    {
        this.hp.transform.localScale = new Vector2(hp, 1f);
    }

    public IEnumerator BarAnimation(float remainingHp)
    {
        float currentHp = hp.transform.localScale.x;
        float change = currentHp - remainingHp;

        while (currentHp - remainingHp > 0.001f)
        {
            currentHp -= change * Time.deltaTime;
            hp.transform.localScale = new Vector2(currentHp, 1f);
            yield return null;
        }

        Set(remainingHp);
    }
}
