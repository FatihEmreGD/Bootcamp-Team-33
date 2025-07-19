using UnityEngine;
using System.Collections;

public class DishwashingStation : MonoBehaviour
{
    public Transform washingPoint; // Eşyanın yıkanırken duracağı nokta
    public float washingTime = 5f; // Yıkama süresi (saniye)

    // Bu metot, PlayerInteraction tarafından çağrılacak
    public IEnumerator StartWashing(GameObject itemToWash, PlayerInteraction playerInteraction)
    {
        Debug.Log("DishwashingStation: StartWashing coroutine started.");
        // Eşyayı yıkama noktasına taşı
        itemToWash.transform.SetParent(washingPoint);
        itemToWash.transform.localPosition = Vector3.zero;
        itemToWash.transform.localRotation = Quaternion.identity;

        // Oyuncunun hareketini engelle
        playerInteraction.SetPlayerMovement(false);

        float timer = 0f;
        bool interrupted = false;
        while (timer < washingTime)
        {
            timer += Time.deltaTime;
            // Yıkama ilerlemesini UI'da gösterebiliriz
            // Debug.Log($"DishwashingStation: Washing progress: {timer / washingTime * 100:F0}%");

            // Eğer oyuncu tekrar etkileşim tuşuna basarsa işlemi iptal et
            if (playerInteraction.CheckWashingInterruptionRequest())
            {
                Debug.Log("DishwashingStation: Interaction button pressed during washing. Interrupting.");
                interrupted = true;
                break; // Döngüyü sonlandır
            }
            yield return null;
        }

        // Oyuncunun hareketini geri aç
        playerInteraction.SetPlayerMovement(true);
        Debug.Log("DishwashingStation: Player movement re-enabled.");

        if (!interrupted)
        {
            // Yıkama tamamlandı
            Pot pot = itemToWash.GetComponent<Pot>();
            if (pot != null)
            {
                pot.CleanPot();
                Debug.Log("DishwashingStation: Pot cleaned.");
            }

            Plate plate = itemToWash.GetComponent<Plate>();
            if (plate != null)
            {
                plate.CleanPlate();
                Debug.Log("DishwashingStation: Plate cleaned.");
            }
            Debug.Log("DishwashingStation: Washing completed.");
        }
        else
        {
            Debug.Log("DishwashingStation: Washing was interrupted. Item remains dirty.");
        }
        
        // Eşyayı oyuncunun eline geri ver
        playerInteraction.PickUpItem(itemToWash);
        Debug.Log("DishwashingStation: Item returned to player.");
    }
}
