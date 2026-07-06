using Evolution.Commons.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Evolution.Commons
{
    public class ResetButton : MonoBehaviour
    {
        public void Reset()
        {
            GameManager.Instance.IsSimulationRunning = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
