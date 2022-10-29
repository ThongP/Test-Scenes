using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager<M> : MonoBehaviour
    where M : Manager<M>
{
    public static M Instance;

    protected void Awake()
    {
        Instance = (M)this;
    }
}
