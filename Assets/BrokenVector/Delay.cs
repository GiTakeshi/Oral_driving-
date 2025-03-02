using UnityEngine;

public class Delay : MonoBehaviour
{
    GameObject text;
    public float delay; // Время до отключения объекта (в секундах)

    void Update(){
        delay = Time.deltaTime;
        if (delay > 10){
            text.SetActive(false);
        }
    }
}