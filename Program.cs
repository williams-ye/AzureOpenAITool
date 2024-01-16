using Azure;
using Azure.AI.OpenAI;
using static System.Environment;
using azure_openai_quickstart;



string endpoint = GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
string key = GetEnvironmentVariable("AZURE_OPENAI_KEY");

OpenAIClient client = new(new Uri(endpoint), new AzureKeyCredential(key));

FileHandler fileHandler = new FileHandler();


String response = "";
List<(string, string)> conversationHistory = [];


while (true)
{
    Console.WriteLine("_____________________________________________________________");

    if (response.Length > 0)
    {
        Console.WriteLine("Commands: \n1: Convert code to C#\n2: Explain code \n3: Explain and convert \n4: Followup question" +
            "\n5: See current conversation history\n6: Save current conversation \n8: See all saved conversations");
    }

    else
    {
        Console.WriteLine("Commands: \n1: Convert code to C#\n2: Explain code \n3: Explain and convert\n5: See current conversation history" +
            "\n6: Save current conversation \n8: See all saved conversations");
    }


    String userInput = Console.ReadLine();


    switch (userInput)
    {
        case "1":
            conversationHistory = [];
            Console.WriteLine("Paste code to convert to C#, followed by a newline with 'exit'");
            string codeToConvert = readInputFromUser();
            conversationHistory.Add(("user", codeToConvert));
            response = convertCode(codeToConvert);
            conversationHistory.Add(("assistant", response));
            Console.WriteLine(response);
            break;

        case "2":
            conversationHistory = [];
            Console.WriteLine("Enter code to explain, followed by a newline with 'exit'");
            string codeToExplain = readInputFromUser();
            conversationHistory.Add(("user", codeToExplain));
            response = explainCode(codeToExplain);
            conversationHistory.Add(("assistant", response));
            Console.WriteLine(response);
            break;

        case "3":
            conversationHistory = [];
            Console.WriteLine("Enter code to explain and convert, followed by a newline with 'exit' ");
            string codeToConvertAndExplain = readInputFromUser();
            conversationHistory.Add(("user", codeToConvertAndExplain));
            response = explainAndConvert(codeToConvertAndExplain);
            conversationHistory.Add(("assistant", response));
            Console.WriteLine(response);
            break;

        case "4":
            Console.WriteLine("Enter question, followed by a newline with 'exit' ");
            string followUp = readInputFromUser();
            conversationHistory.Add(("user", followUp));
            response = followUpQuestion(followUp, conversationHistory);
            conversationHistory.Add(("assistant", response));
            Console.WriteLine(response);
            break;

        case "5":

            if (conversationHistory.Count == 0)
            {
                Console.WriteLine("No current conversation history");
                break;
            }
            else
            {
                Console.WriteLine("Current conversation history length " + conversationHistory.Count);

                foreach (var message in conversationHistory)
                {
                    Console.WriteLine(message.Item1 + ": " + message.Item2);
                }
                break;
            }

        case "6":
            if (conversationHistory.Count == 0)
            {
                Console.WriteLine("No current conversation history");
                break;
            }
            fileHandler.storeConversationHistory(conversationHistory);
            break;

        case "7":
            fileHandler.readConversationHistory("0");
            break;

        case "8":
            fileHandler.getAllFiles("./", "conversationHistory");
            break;

        case "9":

            Console.WriteLine("Enter a file number");
            string fileNumber = Console.ReadLine();
            loadConversationHistory(fileNumber, conversationHistory);
            break;


        default:
            Console.WriteLine("Invalid input");
            break;
    }
}

string followUpQuestion(string followUp, List<(string, string)> conversationHistory)
{
    List<ChatRequestMessage> MessageConversation = [];

    var chatCompletionsOptions = new ChatCompletionsOptions()
    {
        DeploymentName = "Testing", //This must match the custom deployment name you chose for your model
        Messages =
        {
            new ChatRequestAssistantMessage("You are assisting developers translating code written in Smalltalk to C#. You will only reply with the code, and end every message with 'DONE' ")
        },
        MaxTokens = 1500
    };

    foreach (var message in conversationHistory)
    {
        if (message.Item1 == "user")
        {
            chatCompletionsOptions.Messages.Add(new ChatRequestUserMessage(message.Item2));
        }
        else if (message.Item1 == "assistant")
        {
            chatCompletionsOptions.Messages.Add(new ChatRequestAssistantMessage(message.Item2));
        }
    }


    Response<ChatCompletions> response = client.GetChatCompletions(chatCompletionsOptions);

    return response.Value.Choices[0].Message.Content;
}

String convertCode(string code)
{
    var chatCompletionsOptions = new ChatCompletionsOptions()
    {
        DeploymentName = "Testing", //This must match the custom deployment name you chose for your model
        Messages =
        {
            new ChatRequestSystemMessage("You are assisting developers translating code written in Smalltalk to C#. You will only reply with the code, and end every message with 'DONE' "),
        },
        MaxTokens = 1500
    };

    chatCompletionsOptions.Messages.Add(new ChatRequestUserMessage(code));
    Response<ChatCompletions> response = client.GetChatCompletions(chatCompletionsOptions);

    return response.Value.Choices[0].Message.Content;

}

String explainCode(string code)
{
    var chatCompletionsOptions = new ChatCompletionsOptions()
    {
        DeploymentName = "Testing", //This must match the custom deployment name you chose for your model
        Messages =
    {
            new ChatRequestSystemMessage("You are assisting developers translating code written in Smalltalk to C#. You will explain what the code does and how it works, and end every message with 'DONE'"),
            new ChatRequestUserMessage(code),
        },
        MaxTokens = 1500
    };
    Response<ChatCompletions> response = client.GetChatCompletions(chatCompletionsOptions);

    return response.Value.Choices[0].Message.Content;

    //Console.WriteLine(response.Value.Choices[0].Message.Content);
    //
}

String explainAndConvert(string code)
{
    var chatCompletionsOptions = new ChatCompletionsOptions()
    {
        DeploymentName = "Testing", //This must match the custom deployment name you chose for your model
        Messages =
    {
            new ChatRequestSystemMessage("You are assisting developers translating code written in Smalltalk to C#. You will explain what the code does and how it works and also convert it to C# code, and end every message with 'DONE'"),
            new ChatRequestUserMessage(code),
        },
        MaxTokens = 3000
    };
    Response<ChatCompletions> response = client.GetChatCompletions(chatCompletionsOptions);

    return response.Value.Choices[0].Message.Content;

}

void loadConversationHistory(string fileNumber, List<(string, string)> conversationHistory)
{
    string path = @"./conversationHistory" + fileNumber + ".txt";
    using (StreamReader sr = File.OpenText(path))
    {
        string s;
        bool currentIsUser = true;
        string currentMessage = "";

        while ((s = sr.ReadLine()) != null)
        {

            if (s.Length > 6)
            {
                if (currentIsUser)
                {
                    if (s.Substring(0, 6) == "(user,")
                    {
                        Console.WriteLine("THIS IS USER " + s);
                        currentMessage += "user";

                    }
                    else
                    {

                        Console.WriteLine(s);
                    }
                }
                else if (!currentIsUser && s.Length > 11)
                {

                    if (s.Substring(0, 11) == "(assistant,")
                    {
                        Console.WriteLine("THIS IS ASSISTANT " + s);
                        // conversationHistory.Add(("assistant", s));
                        currentIsUser = true;
                    }
                    else
                    {
                        currentMessage += s;
                        Console.WriteLine(s);
                    }
                }

            }
            else
            {
                currentMessage += s;
                Console.WriteLine(s);

            }
        }
        Console.WriteLine("CURRENT MESSAGE _________________________ " + currentMessage);


    }

}

String readInputFromUser()
{
    String userInput = "";
    while (true)
    {
        string codeToConvert = Console.ReadLine();
        //if (codeToConvert.Contains("exit"))
        if (codeToConvert == "exit")
        {
            break;
        }
        else
        {
            userInput += codeToConvert;
        }
    }
    return userInput;

}