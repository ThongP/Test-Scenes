using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATKEntity : BaseEntity
{
    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    public demoFigure figure;

    void Awake()
    {
        figure = gameObject.GetComponentInChildren<demoFigure>();
    }

    void Update()
    {
        if (!HasEnemy)
        {
            FindTarget();
        }

        if (!HasEnemy)
            return;

        if (IsInRange && !moving)
        {
            if (canAttack)
            {
                Debug.Log("Attack!");
                Attack();
                figure?.DoAtkAnim();
            }
            
        }
        else
        {
            GetInRange();
            figure?.DoJumpAnim();
        }
    }

    protected override void Attack()
    {
        base.Attack();

        currentTarget.TakeDamage(Damage);
        

    }
}
