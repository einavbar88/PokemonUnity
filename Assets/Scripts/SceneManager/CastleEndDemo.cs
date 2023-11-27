using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleEndDemo : MonoBehaviour
{
    [SerializeField] Dialog endOfDemo;
    bool started = false;
    void Update()
    {

        if (!started && DataPersistance.dp["Talia"] && !DataPersistance.dp["DemoEnd"])
        {
            StartCoroutine(FinishDemo());
        }
    }

    IEnumerator FinishDemo()
    {
        if (!started)
        {
            started = true;
            yield return GameObjectsDialogs.Instance.Open(endOfDemo);
            DataPersistance.dp["DemoEnd"] = true;
        }
    }
}
