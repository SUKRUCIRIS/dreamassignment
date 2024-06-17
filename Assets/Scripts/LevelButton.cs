using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class LevelButton : MonoBehaviour
{
    private const string IntDataKey = "level";

    // Start is called before the first frame update
    void Start()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            //create event system if there is none
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }
        GameObject buttonObject = GameObject.FindWithTag("mmbutton");
        UnityEngine.UI.Button myButton = buttonObject.GetComponent<UnityEngine.UI.Button>();
        myButton.GetComponentInChildren<TextMeshProUGUI>().text = "Level " + LoadIntData().ToString();
        if (LoadIntData() > 10)
        {
            myButton.GetComponentInChildren<TextMeshProUGUI>().text = "Finished";
        }
        myButton.onClick.RemoveAllListeners();
        myButton.onClick.AddListener(OnButtonClick);
    }
    // Update is called once per frame
    void Update()
    {
    }
    public static void SaveIntData(int data)
    {
        PlayerPrefs.SetInt(IntDataKey, data);
        PlayerPrefs.Save();
    }
    public static int LoadIntData()
    {
        return PlayerPrefs.GetInt(IntDataKey, 1);
    }
    public void OnButtonClick()
    {
        SceneManager.LoadScene("game");
    }
}
