using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    public Transform target; 
    public Vector3 rotationOffset;// Целевой объект, за которым будем следить

    void Update()
    {
        if (target != null)
        {
            // Копируем позицию целевого объекта
            transform.position = target.position;
            Quaternion offset = Quaternion.Euler(rotationOffset);

            // Копируем ротацию целевого объекта и добавляем смещение
            transform.rotation = target.rotation * offset;
        }
    }
}