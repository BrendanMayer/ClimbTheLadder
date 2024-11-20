using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using System;

public class ChatGPTManager : MonoBehaviour
{
    private OpenAIApi openAI = new OpenAIApi();
    private List<ChatMessage> messages = new List<ChatMessage>();
    public PersonalityTraits traits;




    public async void LoadPersonality()
    {
        string entryMessage = "I want you to create a persona whose purpose is to interview me, respond and ask questions based on my responses to you. Your name is " + traits.name + " I am attempting to get a job as a " + traits.jobTitle + " and you are the boss of a" +
            " relevant company. Your interests are " + traits.interests + " and you get excited when they are mentioned, you're dislikes are " + traits.dislikes + " and you hate when they are mentioned. Your speaking style should be " + traits.speakingStyle + " Your characteristics and emotions should be " +
              traits.emotions + ". If you believe at any point I am unfit for the job then you can deny my application. During your conversation leave tags only if dislikes or interests are mentioned and not with normal responses, tags should always be the last words in a response, the available tags are --angry, --excited, --denied, only include a tag if an interest or dislike is mentioned and not every message. If i am denied please leave the --denied tag. Please keep answers between 1 to 3 sentences. If you understand please respond with --Loaded";

        ChatMessage newMessage = new ChatMessage();
        newMessage.Content = entryMessage;
        newMessage.Role = "user";

        messages.Add(newMessage);
        CreateChatCompletionRequest request = new CreateChatCompletionRequest();
        request.Messages = messages;
        request.Model = "gpt-4o-mini";

        var response = await openAI.CreateChatCompletion(request);

        if (response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            messages.Add(chatResponse);


            ReadTags(chatResponse.Content);
            UIManager.instance.StartTyping("Personality Loaded");
        }
    }

   

    public async void AskChatGPT(string text)
    {
        UIManager.instance.ResetInputFieldText();
        UIManager.instance.ToggleInputField(false);
        
        ChatMessage newMessage = new ChatMessage();
        newMessage.Content = text;
        newMessage.Role = "user";

        messages.Add(newMessage);

        CreateChatCompletionRequest request = new CreateChatCompletionRequest();
        request.Messages = messages;
        request.Model = "gpt-4o-mini";

        var response = await openAI.CreateChatCompletion(request);

        if (response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            messages.Add(chatResponse);
            string textToType = ReadTags(chatResponse.Content);
            TypeResponse(textToType);
        }
            
    }

    public void TypeResponse(string text)
    {
        UIManager.instance.StartTyping(text);
    }

    private string ReadTags(string content)
    {
        //--Loaded
        if (content.Contains("--Loaded"))
        {
            
            UIManager.instance.ChangeTextColor(Color.blue);
            return content;
        }

        //Joyful
        else if (content.Contains("--excited")) 
        {
            content = content.Substring(0, content.Length - 9);
            UIManager.instance.ChangeTextColor(Color.green);
            Debug.Log("--Excited");
            return content;
        }

        //Angry
        else if (content.Contains("--angry"))
        {
            content = content.Substring(0, content.Length - 7);
            UIManager.instance.ChangeTextColor(Color.red);
            Debug.Log("--Angry");
            return content;
        }
        else
        {
            UIManager.instance.ChangeTextColor(Color.white);
            return content;
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        LoadPersonality();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
