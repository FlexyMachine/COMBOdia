using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson; // FirstPersonController için

public class LevelEndManager : MonoBehaviour
{
    public GameObject levelEndPanel;
    public string nextSceneName;
    public FirstPersonController playerController;

    void Start()
    {
        if (levelEndPanel != null) levelEndPanel.SetActive(false);

        if (playerController == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null) playerController = playerObject.GetComponent<FirstPersonController>();
        }
        if (playerController == null) Debug.LogError("PlayerController referansı LevelEndManager'da ayarlanmamış!");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) ShowLevelEndPanel();
    }

    void ShowLevelEndPanel()
    {
        if (levelEndPanel == null) return;

        levelEndPanel.SetActive(true);
        Time.timeScale = 0f;

        if (playerController != null)
        {
            playerController.enabled = false;
            // MouseLook'a fareyi serbest bırakmasını ve görünür yapmasını söyle
            if (playerController.m_MouseLook != null)
            {
                playerController.m_MouseLook.SetCursorLock(false); // Bu metod Cursor.visible = true yapar
            }
            else // m_MouseLook yoksa veya erişilemiyorsa doğrudan ayarla
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        else // playerController yoksa da fareyi serbest bırak
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Sadece mevcut leveli yeniden başlatmak için
    void PrepareForRestart()
    {
        Time.timeScale = 1f;
        if (levelEndPanel != null) levelEndPanel.SetActive(false);

        if (playerController != null)
        {
            playerController.enabled = true;
            // MouseLook'a fareyi kilitlemesini söyle (MouseLook bunu kendi halledecektir)
            if (playerController.m_MouseLook != null)
            {
                playerController.m_MouseLook.SetCursorLock(true);
            }
            else // m_MouseLook yoksa veya erişilemiyorsa doğrudan ayarla
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        else // playerController yoksa da fareyi oyun için ayarla
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Sadece yeni bir sahneye geçiş için (farenin serbest kalması beklenir)
    void PrepareForNextSceneLoad()
    {
        Time.timeScale = 1f; // Zamanı normale döndür
        if (levelEndPanel != null) levelEndPanel.SetActive(false);

        // PlayerController'ı etkinleştir (eğer sonraki sahne de bir oyun sahnesiyse ve FPC'ye ihtiyaç duyuyorsa)
        // Eğer sonraki sahne kesinlikle menü ise bu satır opsiyoneldir veya kaldırılabilir.
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        // YENİ SAHNE İÇİN FAREYİ KESİNLİKLE SERBEST BIRAK VE GÖRÜNÜR YAP
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartLevel()
    {
        PrepareForRestart();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        PrepareForNextSceneLoad(); // Yeni sahne için hazırlık fonksiyonunu çağır
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Next Scene Name is not set in the LevelEndManager!");
        }
    }
}