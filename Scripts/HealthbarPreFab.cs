using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthbarPreFab : MonoBehaviour
{
    public Transform bar;
    public Vector3 offset;

    private float maxHealth;
    private Transform target;

    public void Setup(Transform target, float maxHealth)
    {
        this.maxHealth = maxHealth;
        this.target = target;

        UpdateBar(maxHealth);

    }

    public void UpdateBar(float currentHealth)
    {
        float newScale = currentHealth / maxHealth; 
        Vector3 scale = bar.transform.localScale;
        scale.x = newScale;
        bar.transform.localScale = scale;

    }

    private void Update()
    {
        if (target != null)
        {
            this.transform.position = target.position + offset;
        }
    }

}
