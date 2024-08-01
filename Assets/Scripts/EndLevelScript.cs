using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelScript : MonoBehaviour
{

    [SerializeField]
    public GameObject _youWinText;
    public Behaviour _disablePlayerScript;
    [SerializeField]
    private int _timeUntilLevelResets;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Player")
        {
            _disablePlayerScript.enabled = false;
            _youWinText.SetActive(true);
            StartCoroutine (EndGame());
        }
    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(_timeUntilLevelResets);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
