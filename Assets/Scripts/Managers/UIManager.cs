using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


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

    [Header("SpeechUI")]
    [SerializeField] private Sprite microphoneOn;
    [SerializeField] private Sprite microphoneOff;
    [SerializeField] private Image keybindBackground;
 

    public Image microphoneImage;

    [Header("Tooltips")]
    private bool showText;
    [SerializeField] private GameObject interactTooltip;
    [SerializeField] private GameObject dragTooltip;


    [Header("Inventory")]
    [SerializeField] private Image slotOneSprite;
    [SerializeField] private Image slotTwoSprite;
    [SerializeField] private Image slotThreeSprite;
    [SerializeField] private Image slotOneBackground;
    [SerializeField] private Image slotTwoBackground;
    [SerializeField] private Image slotThreeBackground;
    [SerializeField] private Color unselectedSlot;
    #region Components

    [SerializeField] private TMP_Text replyText;
    [SerializeField] private TMP_Text nameText;
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
            AudioManager.Instance.PlayRandomSoundOnMainSource(AudioManager.Instance.speakingSounds);
            yield return new WaitForSeconds(delay);  // Wait for the specified delay
        }

   
        
    }

    public void SetNameText(string name)
    {
        nameText.text = name;
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

    public void SetMicrophoneIndicator(bool on)
    {
        if (on)
        {
            microphoneImage.sprite = microphoneOn;
            Color newColor;
            if (ColorUtility.TryParseHtmlString("#FE3738", out newColor))
            {
                keybindBackground.color = newColor;
            }
        }
        else
        {
            microphoneImage.sprite = microphoneOff;
            Color newColor;
            if (ColorUtility.TryParseHtmlString("#6F6F6F", out newColor))
            {
                keybindBackground.color = newColor;
            }
        }
    }

    public void SetInventorySprite(Sprite sprite, int index)
    {
        if (index == 0)
        {
            slotOneSprite.sprite = sprite;
            Color fullOpacity = slotOneSprite.color;
            fullOpacity.a = 1;
            slotOneSprite.color = fullOpacity;
        }
        else if (index == 1)
        {
            slotTwoSprite.sprite = sprite;
            Color fullOpacity = slotTwoSprite.color;
            fullOpacity.a = 1;
            slotTwoSprite.color = fullOpacity;
        }
        else if(index == 2)
        {
             slotThreeSprite.sprite = sprite;
            Color fullOpacity = slotThreeSprite.color;
            fullOpacity.a = 1;
            slotThreeSprite.color = fullOpacity;
        }
    }

    public void SelectInventorySlot(int index)
    {
        if (index == 0)
        {
            slotOneBackground.color = Color.white;
            slotTwoBackground.color = unselectedSlot;
            slotThreeBackground.color = unselectedSlot;
        }
        else if (index == 1)
        {
            slotTwoBackground.color = Color.white;
            slotOneBackground.color = unselectedSlot;
            slotThreeBackground.color = unselectedSlot;
        }
        else if (index == 2)
        {
            slotThreeBackground.color = Color.white;
            slotOneBackground.color = unselectedSlot;
            slotTwoBackground.color = unselectedSlot;
        }
    }

    public void NoSelectedSlot()
    {
        slotOneBackground.color = unselectedSlot;
        slotTwoBackground.color = unselectedSlot;
        slotThreeBackground.color = unselectedSlot;
    }

    public void RemoveInventorySprite(int index)
    {
        if (index == 0)
        {
            Color noOpacity = slotOneSprite.color;
            noOpacity.a = 0;
            slotOneSprite.color = noOpacity;
        }
        else if (index == 1)
        {
            Color noOpacity = slotTwoSprite.color;
            noOpacity.a = 0;
            slotTwoSprite.color = noOpacity;
        }
        else if (index == 2)
        {
            Color noOpacity = slotThreeSprite.color;
            noOpacity.a = 0;
            slotThreeSprite.color = noOpacity;
        }
    }


    public void ShowTooltip(string tooltip)
    {
        

        interactTooltip.SetActive(tooltip == "INTERACT");
        dragTooltip.SetActive(tooltip == "DRAG");

        showText = (tooltip == "INTERACT" || tooltip == "DRAG");
    }
}
