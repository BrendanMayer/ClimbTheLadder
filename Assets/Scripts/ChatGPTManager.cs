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
    public Task currentTask;

    bool hasGivenPlayerTask;
    public bool taskCompleted;



    public async void LoadPersonality()
    {
        string entryMessage = "Please keep answers fairly short with a max of 96 characters. I want you to create a persona whose purpose is to interact with me as a coworker in an office setting. Your name is " + traits.name + ". I work alongside you in a " + traits.jobTitle + " role at our company. " +
    "Your interests are " + traits.interests + " and you get excited when they are mentioned. Your dislikes are " + traits.dislikes + " and you hate when they are mentioned. Your speaking style should be " + traits.speakingStyle + ". " +
    "Your characteristics and emotions should be " + traits.emotions + ". You will respond naturally to workplace conversations, providing insights,  and sharing opinions. " +
    "Please keep answers between 1 to 3 sentences. If you understand, please respond with --Loaded." ;

        if (currentTask != null)
        {
            Debug.Log("task available and should be, " + currentTask.taskDescription);
            entryMessage += "You also have a task to give. your task to give is " + currentTask.taskDescription + " and this is the only task you can give. when you are asked for a task you can respond with --givetask at the end of the message";
        }
        else
        {
            Debug.Log("no tasks to give player");
            entryMessage += "you have no tasks to give the player";
        }

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

    public async void IntroMessgage()
    {
        string text = "";
        if (hasGivenPlayerTask && !taskCompleted)
        {
            text = "You gave me your task already, tell me to complete it";
        }
        else if (hasGivenPlayerTask && taskCompleted)
        {
            text = "I have completed your task!";
        }
        else
        {
            text = "I started speaking to you again, Say something!";
        }
       
        Debug.Log("asking chatgpt");
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
        else if (content.Contains("--givetask"))
        {
            content = content.Substring(0, content.Length - 10);
            UIManager.instance.ChangeTextColor(Color.yellow);
            TaskManager.Instance.currentTasks.Add(currentTask);
            UIManager.instance.CreateTaskListItem(currentTask);
            hasGivenPlayerTask = true;
            return content;
        }
        else
        {
            UIManager.instance.ChangeTextColor(Color.black);
            return content;
        }
    }

}
