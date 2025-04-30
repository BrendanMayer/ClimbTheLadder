using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TerminalManager : MonoBehaviour
{
    public GameObject directoryLine;
    public GameObject responseLine;

    public TMP_InputField terminalInput;
    public GameObject userInputLine;
    public ScrollRect sr;
    public GameObject msgList;

    Interpreter interpreter;

    public bool terminalFocused;

    private void Start()
    {
        interpreter = GetComponent<Interpreter>();
    }

    private void OnGUI()
    {
        if (terminalInput.isFocused && terminalInput.text != "" && Input.GetKeyDown(KeyCode.Return))
        {

            
            // store what user typed
            string userInput = terminalInput.text;

            // clear input field
            ClearInputField();

            AddDirectoryLine(userInput);

            // Add interpretation lines
            int lines = AddInterpreterLines(interpreter.Interpret(userInput));

            ScrollToBottom(lines);

            // move user input line to end
            userInputLine.transform.SetAsLastSibling();
            
            // refocus input
            terminalInput.ActivateInputField();
            terminalInput.Select();
        }
        if (terminalInput.isFocused)
        {
            terminalFocused = true;
        }
        else
        {
            terminalFocused = false;
           
        }

        GameObject.Find("Player").GetComponent<Player>().terminalOpen = terminalFocused;
    }

    void ClearInputField()
    {
        terminalInput.text = "";
    }


    public void AddDirectoryLine(string userInput)
    {
        Vector2 msgListSize = msgList.GetComponent<RectTransform>().sizeDelta;
        msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(msgListSize.x, msgListSize.y + 35f);

        GameObject msg = Instantiate(directoryLine, msgList.transform);
        // set its child index

        msg.transform.SetSiblingIndex(msgList.transform.childCount -1);

        // Set text

        msg.GetComponentsInChildren<TMP_Text>()[1].text = userInput;



    }

    int AddInterpreterLines(List<string> lines)
    {
        for(int i =0; i < lines.Count; i++)
        {
            GameObject res = Instantiate(responseLine, msgList.transform);

            // set to end of messages
            res.transform.SetAsLastSibling();

            // get size of message list
            Vector2 msgListSize = msgList.GetComponent<RectTransform>().sizeDelta;
            msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(msgListSize.x, msgListSize.y + 35f);

            res.GetComponentInChildren<TMP_Text>().text = lines[i];

        }
        return lines.Count;
    }

    private void ScrollToBottom(int lines)
    {
        if (lines > 4)
        {
            sr.velocity = new Vector2 (0, 450);
        }
        else
        {
            sr.verticalNormalizedPosition = 0;
        }
    }

    public void ClearTerminal()
    {
        int count = msgList.transform.childCount;
        string userInput = terminalInput.text;
        ClearInputField();
        for (int i = 0; i < count - 2; i++)
        {
            Destroy(msgList.transform.GetChild(i).gameObject);
        }
        
        
    }
}

