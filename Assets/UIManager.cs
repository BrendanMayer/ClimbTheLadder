using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    #region Components
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text replyText;
    public float delay = 0.1f;  // Delay between each character

    public GameObject go_inputField;
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   

    // Call this function to start typing out the text
    public void StartTyping(string message)
    {
        // Stop any previous typing coroutine before starting a new one
        StopAllCoroutines();
        StartCoroutine(TypeText(message));
    }

    // Coroutine to type the text letter by letter
    IEnumerator TypeText(string message)
    {
        replyText.text = "";  // Clear the text initially

        foreach (char letter in message.ToCharArray())
        {
            replyText.text += letter;  // Append each character to the UI text
            yield return new WaitForSeconds(delay);  // Wait for the specified delay
        }

        ToggleInputField(true);
        ResetInputFieldText();
        CameraController.instance.CameraAndCursorToggle(false);
    }

    public void ToggleInputField(bool toggle)
    {
        go_inputField.SetActive(toggle);
    }

    public void ResetInputFieldText()
    {
        inputField.text = "";
    }

    public void ChangeTextColor(Color _color)
    {
        replyText.color = _color;
    }
}
