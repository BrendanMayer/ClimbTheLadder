using OpenAI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.Whisper
{
    public class Whisper : MonoBehaviour
    {
     
        [SerializeField] private Dropdown dropdown;
        [SerializeField] private TMP_Text dropdownLabel;
        public ChatGPTManager currentTalkingToNPC;
        
        private readonly string fileName = "output.wav";
        public int duration = 10;
        
        private AudioClip clip;
        private bool isRecording;

        private OpenAIApi openai = new OpenAIApi();
        public int sampleDataLength = 1024;
        private float[] clipSampleData;

        public bool menuOpen;
        private bool recordingInMenu = false;



        private void Start()
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
            dropdown.options.Add(new Dropdown.OptionData("Microphone not supported on WebGL"));
            #else
            foreach (var device in Microphone.devices)
            {
                dropdown.options.Add(new Dropdown.OptionData(device));
            }
            clipSampleData = new float[sampleDataLength];
            
            dropdown.onValueChanged.AddListener(ChangeMicrophone);
            
            var index = PlayerPrefs.GetInt("user-mic-device-index");
            dropdown.SetValueWithoutNotify(index);
            dropdownLabel.text = dropdown.options[dropdown.value].text;
            #endif
        }

        private void ChangeMicrophone(int index)
        {
            PlayerPrefs.SetInt("user-mic-device-index", index);
        }
        
        public void StartRecording()
        {
            
            UIManager.instance.ChangeTextColor(Color.black);
            
            isRecording = true;
           

            var index = PlayerPrefs.GetInt("user-mic-device-index");
            
            #if !UNITY_WEBGL
            clip = Microphone.Start(dropdown.options[index].text, false, duration, 44100);
            
            #endif
        }

        public async void EndRecording()
        {
            
            UIManager.instance.StartTyping("Transcribing...");

#if !UNITY_WEBGL
            Microphone.End(null);
            #endif
            
            byte[] data = SaveWav.Save(fileName, clip);
            
            var req = new CreateAudioTranscriptionsRequest
            {
                FileData = new FileData() {Data = data, Name = "audio.wav"},
                // File = Application.persistentDataPath + "/" + fileName,
                Model = "whisper-1",
                Language = "en"
            };
            var res = await openai.CreateAudioTranscription(req);

            currentTalkingToNPC.AskChatGPT(res.Text);

           // UIManager.instance.StartTyping(res.Text);
            
        }

        private void Update()
        {
            if (isRecording)
            {

                UpdateSliderBasedOnLoudness();

                UIManager.instance.SetMicVolValue(0);
            }
            

            if (menuOpen)
            {
                if (!recordingInMenu)
                {
                    
                    StartRecordingInMenu();
                }
                UpdateLoudnessForMenu();
                
            }
            else
            {
                recordingInMenu = false;
            }
        }

        private void UpdateSliderBasedOnLoudness()
        {
            RecordAndProcessLoudness();
        }

        private void UpdateLoudnessForMenu()
        {
            RecordAndProcessLoudness();
        }

        private void StartRecordingInMenu()
        {
            recordingInMenu = true;
            var index = PlayerPrefs.GetInt("user-mic-device-index");

            #if !UNITY_WEBGL
            clip = Microphone.Start(dropdown.options[index].text, false, duration*100, 44100);

            #endif
        }

        private void RecordAndProcessLoudness()
        {
            if (clip != null)
            {
                // Fetch the latest microphone data
                var index = PlayerPrefs.GetInt("user-mic-device-index");
                int micPosition = Microphone.GetPosition(dropdown.options[index].text); // Current position in the recording
                int startPosition = Mathf.Max(0, micPosition - sampleDataLength);

                clip.GetData(clipSampleData, startPosition);

                // Calculate RMS value (Root Mean Square) of the samples
                float sum = 0f;
                for (int i = 0; i < sampleDataLength; i++)
                {
                    sum += clipSampleData[i] * clipSampleData[i];
                }
                float rmsValue = Mathf.Sqrt(sum / sampleDataLength);

                // Map RMS value to 0-1 range for slider
                float loudness = Mathf.Clamp01(rmsValue * 10); // Adjust scaling as needed
                UIManager.instance.SetMicVolValue(loudness);
                
            }
        }
    }
}
