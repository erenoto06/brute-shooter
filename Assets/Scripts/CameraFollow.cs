using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Takip Edilecek Hedef")]
    public Transform target; // Karakterimizin Transform bileşeni

    [Header("Kamera Ayarları")]
    public float smoothSpeed = 5f; // Takip yumuşatma hızı
    public Vector3 offset = new Vector3(0f, 0f, -10f); // Kameranın Z mesafesi (-10 olmalı)

    void Start()
    {
        // Eğer Inspector'dan elle verilmediyse otomatik bul
        if (target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Hedef pozisyon (Karakter + Z ofseti)
        Vector3 targetPosition = target.position + offset;

        // Kamerayı hedefe doğru yumuşakça kaydırıyoruz
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        // Kameranın pozisyonunu güncelle
        transform.position = smoothedPosition;
    }
}
