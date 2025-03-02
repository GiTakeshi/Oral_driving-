using UnityEngine;

public class finish : MonoBehaviour
{
    public CarController carcontrol;
    void Start(){
        carcontrol = GameObject.Find("Simple Retro Car").GetComponent<CarController>();;
    }
    // Этот метод вызывается, когда другой коллайдер входит в триггер
    private void OnTriggerEnter(Collider other)
    {

        // Проверяем, что вошел игрок (или другой объект с тегом "Player")
        if (other.CompareTag("Player"))
        {
            //Debug.Log("ZOV");
            carcontrol.isRunning = false;

            // Здесь можно добавить логику завершения уровня
            // Например, загрузить следующую сцену или показать сообщение о победе
        }
    }
}