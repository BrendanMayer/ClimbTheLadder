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

    private void Start()
    {
        LoadPersonality();
    }

    public async void LoadPersonality()
    {
        string entryMessage = "Clear all previous Requests. Please keep answers fairly short with a max of 96 characters. I want you to create a persona whose purpose is to interact with me as a coworker in an office setting. Your name is " + traits.name + ". I work alongside you in a " + traits.jobTitle + " role at our company. " +
    "Your interests are " + traits.interests + " and you get excited when they are mentioned. Your dislikes are " + traits.dislikes + " and you hate when they are mentioned. Your speaking style should be " + traits.speakingStyle + ". " +
    "Your characteristics and emotions should be " + traits.emotions + ". You will respond naturally to workplace conversations, providing insights,  and sharing opinions. " +
    "Please keep answers between 1 to 3 sentences. If you understand, please respond with --Loaded only." ;
       

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


           // ReadTags(chatResponse.Content);
            
        }
    }

   

    public async void AskChatGPT(string text)
    {
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
    public async void AskChatGPTNoReturnMessage(string text)
    {
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
        else if (content.Contains("--givetask"))
        {
            NPC npc;
            content = content.Substring(0, content.Length - 10);
            UIManager.instance.ChangeTextColor(Color.yellow);
            // move to task generation super state, npc for now
            if (npc = GetComponent<NPC>())
            {
                npc.GiveTask();
            }
            return content;
        }
        else if (content.Contains("--giveitem"))
        {
            Debug.Log("Give Key");
            content = content.Substring(0, content.Length - 10);
            GetComponent<NPC>().GiveItem();
            return content;
        }
        else
        {
            UIManager.instance.ChangeTextColor(Color.black);
            return content;
        }
    }

}
