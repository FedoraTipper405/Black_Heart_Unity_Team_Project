using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Image HealthBar;
    public float healthAmount = 100f;
    public static HealthManager Instance;
    public bool _invincible;
    public int _healingPotions;
    // Start is called before the first frame update

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (healthAmount <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }


        if (Input.GetKeyDown(KeyCode.H) && healthAmount != 100)
        {
            Heal(10);
        }
    }

    public void TakeDamage(float damage)
    {
        healthAmount -= damage;
        HealthBar.fillAmount = healthAmount / 100;
        StartCoroutine(StopTakingDamage());
    }

    IEnumerator StopTakingDamage()
    {
        _invincible = true;
        yield return new WaitForSeconds(1f);
        _invincible = false;
    }

    public void GainPotion()
    {
        _healingPotions++;
    }

    public void Heal(float healingAmount)
    {
        if (_healingPotions > 0)
        {
            healthAmount += healingAmount;
            healingAmount = Mathf.Clamp(healthAmount, 0, 100);
            HealthBar.fillAmount = healthAmount / 100f;
            _healingPotions--;
        }
    }
}
