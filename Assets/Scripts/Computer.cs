using Samples.Whisper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Computer : MonoBehaviour, IInteractable
{
    public Material powerOff;
    public Material powerOn;
    public bool power;
    public GameObject powerButton;
    Player player;
    public GameObject UI;
   
    public GameObject settingsWindow;
    public GameObject hireMenu;
 


    public List<HireData> hireCandidates = new List<HireData>();
    private int hireIndex;
    private int correctlyHired;
    public TMP_Text hireName;
    public TMP_Text hireAge;
    public TMP_Text hireDOB;
    public TMP_Text hireQualifications;
    public TMP_Text progressBarText;
    public Slider progressBar;
    public GameObject quotaMenu;
    public List<Sprite> characterSprites = new List<Sprite>();
    public Image characterImage;


    private string[] names = { "Joe Banana", "Sally McDonut", "Rick McTaco", "Fiona Giggles", "Bob Banana", "Harry Hipster" };
    private string[] qualifications =
{
    // good
    "Master of Engineering",
    "Doctor of Medicine",
    "Certified Accountant",
    "Professional Software Developer",
    "Law Degree",
    "Licensed Pilot",
    "Certified Mechanical Technician",
    "Bachelor of Business Administration",
    "Certified Cybersecurity Expert",
    "Professional Chef",
    "PhD in Astrophysics",
    "Licensed Architect",
    "Certified Public Speaker",
    "Veterinary Medicine Degree",
    "Certified Emergency Responder",
    "Master of Data Science",
    "Certified Electrical Technician",
    "AI and Machine Learning Specialist",
    "Licensed Pharmacist",
    "Structural Engineer",

    //bad
    "Certified Couch Potato",
    "Self-Proclaimed Wizard",
    "Master of Procrastination",
    "Champion of Doing Nothing",
    "Alien Abduction Survivor",
    "YouTube Conspiracy Theorist",
    "Degree in Underwater Basket Weaving",
    "Professional Napper",
    "Certified Meme Expert",
    "Master of Clicking ‘Remind Me Tomorrow’",
    "Junior Chicken Nugget Taster",
    "Diploma in Extreme Laziness",
    "Expert at Googling Things",
    "Certified Time Traveler",
    "Degree in Watching Paint Dry",
    "PhD in Overthinking",
    "Award-Winning TikTok Commenter",
    "Master of Hitting Snooze",
    "Professional ‘Reply All’ Emailer",
    "Licensed Daydreamer"
};

    public void EnableOrDisableText(bool enable)
    {
        
    }

    public void Interact()
    {
        
        player.stateMachine.ChangeState(player.computerState);
    }

    public bool IsGrabbableItem()
    {
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();

        CloseAllWindows();
        UI.SetActive(false);
        GenerateRandomHireData(50);
        PopulateHireData();
    }

    private void CloseAllWindows()
    {
        
        settingsWindow.SetActive(false);
        

    }

    // Update is called once per frame
    void Update()
    {
        // Check if the mouse is over a GameObject and if it's the correct one
        
    }

    public void ToggleRecording(bool toggle)
    {
        player.GetComponent<Whisper>().menuOpen = toggle;
    }

    public void OnPowerButtonClicked()
    {
        
        power = !power;
        UI.SetActive(power);
        if (power)
        {
            powerButton.GetComponent<Renderer>().material = powerOn;
            
        }
        else
        {
            powerButton.GetComponent<Renderer>().material = powerOff;
            CloseAllWindows();
        }

        
    }

    void GenerateRandomHireData(int count)
    {
        for (int i = 0; i < count; i++)
        {
            string name = names[UnityEngine.Random.Range(0, names.Length)];
            int age = UnityEngine.Random.Range(18, 65); // Random age between 18 and 65
            string dateOfBirth = DateTime.Now.AddYears(-age).ToString("yyyy-MM-dd");

            string qualification = qualifications[UnityEngine.Random.Range(0, qualifications.Length)];

            bool acceptOrDeny = IsQualified(qualification); // Accept or deny based on qualification

            HireData newHire = new HireData(name, age, dateOfBirth, qualification, acceptOrDeny);
            hireCandidates.Add(newHire);
        }
    }

    
    bool IsQualified(string qualification)
    {
        
        string[] goodQualifications =
        {
        "Master of Engineering",
    "Doctor of Medicine",
    "Certified Accountant",
    "Professional Software Developer",
    "Law Degree",
    "Licensed Pilot",
    "Certified Mechanical Technician",
    "Bachelor of Business Administration",
    "Certified Cybersecurity Expert",
    "Professional Chef",
    "PhD in Astrophysics",
    "Licensed Architect",
    "Certified Public Speaker",
    "Veterinary Medicine Degree",
    "Certified Emergency Responder",
    "Master of Data Science",
    "Certified Electrical Technician",
    "AI and Machine Learning Specialist",
    "Licensed Pharmacist",
    "Structural Engineer",
    };

      
        string[] badQualifications =
        {
        "Certified Couch Potato",
    "Self-Proclaimed Wizard",
    "Master of Procrastination",
    "Champion of Doing Nothing",
    "Alien Abduction Survivor",
    "YouTube Conspiracy Theorist",
    "Degree in Underwater Basket Weaving",
    "Professional Napper",
    "Certified Meme Expert",
    "Master of Clicking ‘Remind Me Tomorrow’",
    "Junior Chicken Nugget Taster",
    "Diploma in Extreme Laziness",
    "Expert at Googling Things",
    "Certified Time Traveler",
    "Degree in Watching Paint Dry",
    "PhD in Overthinking",
    "Award-Winning TikTok Commenter",
    "Master of Hitting Snooze",
    "Professional ‘Reply All’ Emailer",
    "Licensed Daydreamer"
    };

        
        if (goodQualifications.Contains(qualification)) return true;  
        if (badQualifications.Contains(qualification)) return false;  

        // Any other qualifications default to denied
        return false;
    }

    
    public void PopulateHireData()
    {
        if (hireIndex >= hireCandidates.Count) hireIndex = 0;
        hireName.text ="Name: " + hireCandidates[hireIndex].name;
        hireAge.text = "Age: " + hireCandidates[hireIndex].age;
        hireDOB.text = "D.O.B: " + hireCandidates[hireIndex].dateOfBirth;
        hireQualifications.text = "Qualifications : " + hireCandidates[hireIndex].qualifications;
        characterImage.sprite = characterSprites[UnityEngine.Random.Range(0, characterSprites.Count)];
    }

    public void CheckHireAnswer(bool answer)
    {
        if (answer == hireCandidates[hireIndex].acceptOrDeny)
        {
            correctlyHired++;
            
        }
        else
        {
            correctlyHired--;
            
            //play wrong sound
        }
        progressBarText.text = correctlyHired + " / 10";
        progressBar.value = correctlyHired;
        hireIndex++;

        if (correctlyHired == 10)
        {
            quotaMenu.SetActive(true);
        }
        PopulateHireData();
    }

}
