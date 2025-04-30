using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Interpreter : MonoBehaviour
{
    public List<CoworkerPCData> pcDatas = new List<CoworkerPCData>();
    public CoworkerPCData connectedPC;
    private TerminalManager terminalManager;
    public float timer;
    public float minHazardTime;
    public float maxHazardTime;
    public int minAffectedComputers;
    public int maxAffectedComputers;

    private readonly string[] virusFiles = { "trojan.exe", "malware.exe", "worm.bat", "spyware.dll", "ransom.zip" };
    private readonly string[] networkFields = { "ip", "gateway", "dns" };


    Dictionary<string, string> colors = new Dictionary<string, string>()
    {
        {"black", "#021b21" },
        {"gray", "#555d71" },
        {"red", "#ff5879" },
        {"yellow", "#f2f1b9" },
        {"blue", "#9ed9d8" },
        {"purple", "#d926ff" },
        {"orange", "#ef5847" },
    };


    List<string> response = new List<string>();

    private void Start()
    {

        Time.timeScale = 1.0f;
        pcDatas = FindObjectsByType<CoworkerPCData>(FindObjectsSortMode.None).ToList();
        InitializePCs();
        


        terminalManager = GetComponent<TerminalManager>();
        StartCoroutine(SpawnHazardsOverTime());
        
    }

    public List<string> Interpret(string userInput)
    {
        response.Clear();

        string[] args = userInput.Split();

        if (args[0] == "help")
        {
            ListEntry("help", "returns list of commands.");
            ListEntry("ls", "returns list of files.");
            ListEntry("pclist", "returns list of coworker computers.");
            ListEntry("rm", "removes a file, rm <file_name>.");
            ListEntry("connect", "connects to a computer, connect <pc_name>.");
            ListEntry("disconnect", "disconnects from coworker computer.");
            ListEntry("scan", "scan for errors on all computers.");
            ListEntry("ipconfig", "shows if a pc is connected to a network");
            ListEntry("ipreset", "resets a network field, ipreset ip/dns/gateway");
        }
        else if (args[0] == "ls")
        {
            if (connectedPC == null)
            {
                response.Add("Not connected to a pc. use 'connect <pc_name>'");
            }
            else
            {
                int count = 0;
                string fileList = "";
                foreach (string file in connectedPC.files)
                {
                    fileList += file + "   ";
                    count++;
                    if (count == 4)
                    {
                        response.Add(fileList);
                        fileList = "";
                        count = 0;
                    }
                }
                // Add any remaining files
                if (!string.IsNullOrEmpty(fileList))
                {
                    response.Add(fileList);
                }
            }
        }
        else if (args[0] == "pclist")
        {
            int count = 0;
            string fileList = "";
            foreach (CoworkerPCData data in pcDatas)
            {
                string coloredName = ColorString(data.pcName, "green"); 
                fileList += coloredName + "   ";

                count++;
                if (count == 2)
                {
                    response.Add(fileList);
                    fileList = "";
                    count = 0;
                }
            }
            if (!string.IsNullOrEmpty(fileList))
            {
                response.Add(fileList);
            }
        }
        else if (args[0] == "rm")
        {
            if (connectedPC == null)
            {
                response.Add("Not connected to a pc. use 'connect <pc_name>'");
            }
            else
            {
                if (connectedPC.files.Contains(args[1]))
                {
                    connectedPC.files.Remove(args[1]);
                    response.Add("Successfully removed " + "'" + args[1] + "'");
                    
                }
                else
                {
                    response.Add("No file found named " + "'" + args[1] + "'");
                }


            }
        }
        else if (args[0] == "connect")
        {
            bool foundPC = false;
            foreach (CoworkerPCData pc in pcDatas)
            {

                if (args[1] == pc.pcName)
                {
                    foundPC = true;
                    connectedPC = pc;
                    response.Add("Connected to " + pc.pcName);
                    break;
                }
                else
                {
                    foundPC = false;
                }
                
            }

            if (!foundPC)
            {
                response.Add("Could not find computer with that name");
            }

        }
        else if (args[0] == "disconnect")
        {
            response.Add("disconnected from " + connectedPC.pcName);
            connectedPC = null;
        }
        else if (args[0] == "clear")
        {
            terminalManager.ClearTerminal();
        }
        else if (args[0] == "scan")
        {
            ScanAllComputers();
        }
        else if (args[0] == "ipconfig")
        {
            if (connectedPC != null)
            {
                response.Add("Connected pc network information: ");
                ListEntry("IPV4 Address: ", connectedPC.ip);
                ListEntry("DNS: ", connectedPC.dns);
                ListEntry("Gateway ", connectedPC.gateway);
            }
            else
            {
                response.Add("Not connected to a pc. use 'connect <pc_name>'");
            }
            
        }
        else if (args[0] == "ipreset")
        {
            if (connectedPC != null)
            {
                if (args[1] == "ip" && connectedPC.ip != "Not Found")
                {
                    connectedPC.ip = ipPool[Random.Range(0, ipPool.Length)];
                    connectedPC.hasError = false;
                    response.Add("Network reset successfully");
                }
                else if (args[1] == "dns" && connectedPC.dns != "Not Found")
                {
                    connectedPC.dns = dnsPool[Random.Range(0, dnsPool.Length)];
                    connectedPC.hasError = false;
                    response.Add("Network reset successfully");
                }
                else if (args[1] == "gateway" && connectedPC.gateway != "Not Found")
                {
                    connectedPC.gateway = gatewayPool[Random.Range(0, gatewayPool.Length)];
                    connectedPC.hasError = false;
                    response.Add("Network reset successfully");
                }
                else
                {
                    response.Add("Field not found. please use ipreset ip/dns/gateway");
                }
            }
            else
            {
                response.Add("Not connected to a pc. use 'connect <pc_name>'");
            }

        }
        else
        {
            response.Add("Command not found. type 'help' for a list of commands.");

        }
        return response;
    }

    public string ColorString(string s, string color)
    {
        string leftTag = "<color=" + color + ">";
        string rightTag = "</color>";

        return leftTag + s + rightTag;
    }

    void ListEntry(string a, string b)
    {
        response.Add(ColorString(a, colors["orange"]) + ": " + ColorString(b, colors["yellow"]));
    }

    private IEnumerator SpawnHazardsOverTime()
    {
        while (true)
        {
            float waitTime = Random.Range(minHazardTime, maxHazardTime);
            
            yield return new WaitForSeconds(waitTime);

            int count = Random.Range(minAffectedComputers, maxAffectedComputers + 1);
            

           
            List<CoworkerPCData> shuffledList = new List<CoworkerPCData>(pcDatas);
            Shuffle(shuffledList); 

            for (int i = 0; i < Mathf.Min(count, shuffledList.Count); i++)
            {
                CoworkerPCData pc = shuffledList[i];
                if (pc.hasError) continue; // Skip already affected PCs

                ApplyRandomHazard(pc);
                Debug.Log("Applying hazard to " + pc.pcName);
            }
        }
    }

    private void ApplyRandomHazard(CoworkerPCData pc)
    {
        pc.hasError = true;

        bool isVirus = Random.value > 0.5f;
        if (isVirus)
        {
            string virus = virusFiles[Random.Range(0, virusFiles.Length)];
            pc.files.Add(virus);
            Debug.Log($"{pc.pcName} infected with {virus}");
        }
        else
        {
            string field = networkFields[Random.Range(0, networkFields.Length)];
            switch (field)
            {
                case "ip": pc.ip = "not found"; break;
                case "gateway": pc.gateway = "not found"; break;
                case "dns": pc.dns = "not found"; break;
            }
            Debug.Log($"{pc.pcName} has lost network config: {field}");
        }
    }


    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }


    private readonly string[] baseFiles = {
        "report.docx", "presentation.pptx", "budget.xlsx", "notes.txt", "image.png",
        "project1.cs", "summary.pdf", "todo.md", "email_backup.eml", "sprint_plan.docx",
        "mockup.jpg", "invoice.csv", "readme.txt", "logfile.log", "meeting.ics"
    };

    private readonly string[] ipPool = {
        "192.168.1.2", "192.168.1.15", "192.168.1.42", "192.168.1.78",
        "10.0.0.12", "10.0.0.55", "172.16.0.101", "172.16.0.45"
    };

    private readonly string[] gatewayPool = {
        "192.168.1.1", "10.0.0.1", "172.16.0.1"
    };

    private readonly string[] dnsPool = {
        "8.8.8.8", "8.8.4.4", "1.1.1.1", "9.9.9.9", "208.67.222.222"
    };

    private readonly string[] techPrefixes = {
        "dev", "build", "sys", "test", "node", "data", "byte", "alpha", "beta", "prod", "debug", "crunch", "ping", "net"
    };

    private readonly string[] techSuffixes = {
        "01", "99", "x0", "404", "7", "13", "42", "256", "9000", "v2", "001", "main", "dev", "stack"
    };

    private readonly string[] pcTypes = { "PC", "WORKSTATION", "NODE" };

    private void InitializePCs()
    {
        foreach (var pc in pcDatas)
        {
            // Generate a techy PC name
            string prefix = techPrefixes[Random.Range(0, techPrefixes.Length)];
            string suffix = techSuffixes[Random.Range(0, techSuffixes.Length)];
            string type = pcTypes[Random.Range(0, pcTypes.Length)];
            pc.pcName = $"{prefix}{suffix}-{type}";

            // Generate random files
            pc.files.Clear();
            int fileCount = Random.Range(1, 6); // 1–5 files
            List<string> filePool = new List<string>(baseFiles);
            for (int i = 0; i < fileCount; i++)
            {
                int index = Random.Range(0, filePool.Count);
                pc.files.Add(filePool[index]);
                filePool.RemoveAt(index); // Avoid duplicates
            }

            // Assign network settings
            pc.ip = ipPool[Random.Range(0, ipPool.Length)];
            pc.gateway = gatewayPool[Random.Range(0, gatewayPool.Length)];
            pc.dns = dnsPool[Random.Range(0, dnsPool.Length)];
            pc.hasError = false;
        }
    }

    private void ScanAllComputers()
    {
        foreach (CoworkerPCData pc in pcDatas)
        {
            bool hasVirus = pc.files.Any(f => virusFiles.Contains(f));
            bool hasNetworkError = pc.ip == "not found" || pc.gateway == "not found" || pc.dns == "not found";

            string scanResult = pc.pcName + ": ";

            if (hasVirus)
            {
                scanResult += ColorString("Virus detected!", "red");
            }
            else if (hasNetworkError)
            {
                scanResult += ColorString("Network error!", "red");
            }
            else
            {
                scanResult += ColorString("No issues found.", "green");
            }

            response.Add(scanResult);
        }
    }
}
