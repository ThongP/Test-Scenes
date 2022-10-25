using UnityEngine;
using UnityEngine.SceneManagement;
using Random=System.Random;

namespace Game
{
    public class demoBird : MonoBehaviour
    {
        public demoFigure figure;

        public int maxHealth = 32;
        public int currentHealth;

        public HealthBar healthBar;

        public int seconds = 1 * 1000;
        private void Start()
        {
            if (transform.tag == "AtkBird")
            {
                maxHealth = 16;
            }
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);

            figure = gameObject.GetComponentInChildren<demoFigure>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A) && currentHealth > 0)
            {
                figure?.DoAtkAnim();
                DamageCal(DamageRnd());
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                figure?.DoJumpAnim();
            }

        }

        private void DamageCal(int damage)
        {   
            currentHealth -= damage;
            healthBar.SetHealth(currentHealth);
        }

        private int DamageRnd()
        {
            int rndATK, rndDEF;
            if (transform.tag == "AtkBird")
            {
                Random rnd = new Random();
                rndATK = rnd.Next(3);
                return rndATK;
            }
            else
            {
                Random rnd = new Random();
                rndDEF = rnd.Next(3);
                return rndDEF;
            }
        }
    }
}
