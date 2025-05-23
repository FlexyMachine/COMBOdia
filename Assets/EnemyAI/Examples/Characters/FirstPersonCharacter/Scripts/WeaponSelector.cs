using UnityEngine;

public class WeaponSelector : MonoBehaviour
{
    [System.Serializable]
    public class Weapon
    {
        public string key;
        public GameObject prefab; // Prefab to instantiate
    }

    public Weapon[] weapons; // Array of weapons

    private GameObject currentWeapon;

    void Update()
    {
        // Check for input to switch weapons
        foreach (var weapon in weapons)
        {
            if (Input.GetKeyDown(weapon.key))
            {
                SelectWeapon(weapon.prefab);
                break;
            }
        }
    }

    void SelectWeapon(GameObject weaponPrefab)
    {
        // Destroy the current weapon if it exists
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }

        // Instantiate the new weapon
        currentWeapon = Instantiate(weaponPrefab, transform.position, transform.rotation);
        currentWeapon.transform.SetParent(transform); // Set the parent to the player or character
    }
}