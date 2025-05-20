using UnityEngine;

public class SlidingDoors : MonoBehaviour
{
    Animator _animataor;
    bool isOpen = false;

    void Awake()
    {
        _animataor = GetComponent<Animator>(); 
        _animataor.enabled = true; 
    }


    void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("Enemy")) && !isOpen) {
            Debug.Log("Kutu girdi");
            _animataor.SetTrigger("Door");
            isOpen = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("Enemy")) && isOpen) {
            Debug.Log("Kutu çıktı");
            _animataor.SetTrigger("rooD");
            isOpen = false;
        }
    }

}
