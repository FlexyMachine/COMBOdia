using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
public class LevelEndManager : MonoBehaviour
{
    public GameObject levelEndPanel; // Level bittiğinde gösterilecek UI paneli
    public string nextSceneName; // Geçilecek bir sonraki sahnenin adı
    public FirstPersonController playerController;

    void Start()
    {
        if (levelEndPanel != null) levelEndPanel.SetActive(false); // Başta paneli gizle

        if (playerController == null) 
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null) playerController = playerObject.GetComponent<FirstPersonController>();
        }
        if (playerController == null) Debug.LogError("PlayerController referansı LevelEndManager'da ayarlanmamış!");
    }

    void OnTriggerEnter(Collider other) // Oyuncu bu objeye çarptığında çalışır
    {
        if (other.CompareTag("Player")) ShowLevelEndPanel();
    }

    void ShowLevelEndPanel() // Level sonu panelini göster
    {
        if (levelEndPanel == null) return;

        levelEndPanel.SetActive(true); // Paneli aktif et
        Time.timeScale = 0f; // Zamanı durdur

        if (playerController != null)
        {
            playerController.enabled = false; // Oyuncu hareketini durdur
            if (playerController.m_MouseLook != null) // Mouse kontrolünü serbest bırak
            {
                playerController.m_MouseLook.SetCursorLock(false);
            }
            else 
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        else 
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void PrepareForRestart() // Sahneyi yeniden başlatmak için hazırlık yap
    {
        Time.timeScale = 1f; // Zamanı tekrar başlat
        if (levelEndPanel != null) levelEndPanel.SetActive(false);

        if (playerController != null)
        {
            playerController.enabled = true; // Hareketi tekrar aktif et
            if (playerController.m_MouseLook != null)
            {
                playerController.m_MouseLook.SetCursorLock(true);
            }
            else 
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        else 
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void PrepareForNextSceneLoad() // Sonraki sahne yüklemesi için hazırlık yap
    {
        Time.timeScale = 1f;
        if (levelEndPanel != null) levelEndPanel.SetActive(false);

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        Cursor.lockState = CursorLockMode.None; // Cursor’u serbest bırak
        Cursor.visible = true;
    }

    public void RestartLevel() // Sahneyi yeniden yükle
    {
        PrepareForRestart();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Aynı sahneyi yeniden yükle
    }

    public void LoadNextLevel() // Belirtilen sahneye geç
    {
        PrepareForNextSceneLoad(); 
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName); // Yeni sahneyi yükle
        }
        else
        {
            Debug.LogError("Next Scene Name is not set in the LevelEndManager!");
        }
    }
}