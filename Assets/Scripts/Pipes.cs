using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipes : MonoBehaviour
{
    public float speed = 5f;
    private float leftEdge;

    private void Start()
    {
        leftEdge = Camera.main.ScreenToWorldPoint(Vector3.zero).x - 1f; // получаем левый край по левому краю камеры, значение которой по x это 0 (zero тут). 
        // Вычитаем единицу так как при нуле трубы исчезнут не зайдя за экран
    }
    private void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime; // изменение позиции труб влево, со скоростью 5, time.deltatime это правильный расчет на кадры

        if (transform.position.x < leftEdge)
        {
            Destroy(gameObject);
        }
    }
}
