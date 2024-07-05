using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    TMP_Text PotionText;

    [SerializeField]
    HealthManager HealthManager;

    void Start()
    {

    }
    void Update()
    {
        PotionText.text = "Potions: " + HealthManager._healingPotions;
    }
}
