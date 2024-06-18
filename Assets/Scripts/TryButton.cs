using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TryButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject trybutton = GameObject.FindWithTag("trybutton");
        UnityEngine.UI.Button myButton = trybutton.GetComponent<UnityEngine.UI.Button>();
        myButton.onClick.RemoveAllListeners();
        myButton.onClick.AddListener(OnButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnButtonClick()
    {
        SceneManager.LoadScene("game");
    }
}
