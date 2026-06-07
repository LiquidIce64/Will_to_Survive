using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadMenuManager : MonoBehaviour
{
    public static DeadMenuManager Instance;

    [SerializeField] private GameObject deadMenuPanel;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        deadMenuPanel.SetActive(false);
    }

    public void ShowDeadMenu()
    {
        deadMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}