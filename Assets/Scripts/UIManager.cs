using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject talkingWindow;
    //public TMP_Text NPCText;
    //public TMP_Text playerText;
    public List<string> openingLines = new List<string>();
    public Image micVolume;
    public GameObject tasksList;
    public GameObject taskItemPrefab;
    private List<GameObject> taskListitems = new List<GameObject>();
    #region Components

    [SerializeField] private TMP_Text replyText;
    public float delay = 0.1f;  // Delay between each character

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
        ChangeTextColor(Color.black);
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

   
        
    }


    public void ChangeTextColor(Color _color)
    {
        replyText.color = _color;
    }

    public void ToggleChatWindow(bool toggle)
    {
        talkingWindow.SetActive(toggle);
    }

    public void InitiateDialogue()
    {
        int roll = Random.Range(0, openingLines.Count);
        //NPCText.text = openingLines[roll];
        replyText.text = openingLines[roll];
    }

    internal void SetMicVolValue(float loudness)
    {
        micVolume.fillAmount = loudness;
    }

    public void CreateTaskListItem(Task task)
    {
        GameObject taskList = Instantiate(taskItemPrefab, tasksList.transform);
        taskList.GetComponent<TaskList>().SetTask(task);
        taskListitems.Add(taskList);
    }

    public void CompleteTaskListItem(Task task, bool isCompleted)
    {
        foreach (GameObject item in taskListitems)
        {
            if (item.GetComponent<TaskList>() != null && item.GetComponent<TaskList>().task == task && isCompleted)
            {
                item.GetComponent<TaskList>().SetTaskCompleted();
            }
        }
    }

    public void StopTypingAndClear()
    {
        ChangeTextColor(Color.black);
        StopAllCoroutines();
        replyText.text = "";
    }
}
