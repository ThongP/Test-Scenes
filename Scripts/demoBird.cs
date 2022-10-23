using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class demoBird : MonoBehaviour
    {
        private demoFigure figure;

        private void Start()
        {
            figure = gameObject.GetComponentInChildren<demoFigure>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                figure?.DoAtkAnim();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                figure?.DoJumpAnim();
            }
        }
    }
}
