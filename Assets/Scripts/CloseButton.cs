using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CloseButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject closebutton = GameObject.FindWithTag("closebutton");
        UnityEngine.UI.Button myButton = closebutton.GetComponent<UnityEngine.UI.Button>();
        myButton.onClick.RemoveAllListeners();
        myButton.onClick.AddListener(OnButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnButtonClick()
    {
        SceneManager.LoadScene("mainmenu");
    }
}
