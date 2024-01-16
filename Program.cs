using Azure;
using Azure.AI.OpenAI;
using static System.Environment;
using azure_openai_quickstart;



string endpoint = GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
string key = GetEnvironmentVariable("AZURE_OPENAI_KEY");

OpenAIClientOptions options = new OpenAIClientOptions()
{
    Retry =
            {
        Delay = TimeSpan.FromSeconds(2),
        MaxRetries = 10,
        Mode = RetryMode.Fixed


            }
};

OpenAIClient client = new(new Uri(endpoint), new AzureKeyCredential(key), options);


OpenAIClient client = new(new Uri(endpoint), new AzureKeyCredential(key));

FileHandler fileHandler = new FileHandler();


String response = "";
List<(string, string)> conversationHistory = [];


while (true)
{
    Console.WriteLine("_____________________________________________________________");


    Console.WriteLine("Commands: \n1: Convert code to C#\n2: Explain code \n3: Explain and convert \n4: Follow-up question to answer" +
       "\n5: See current conversation history\n6: Save current conversation \n8: See all saved conversations \n9: Load and continue on previous conversations\n10: Delete specific conversations");



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
            if (response == "")
            {
                Console.WriteLine("No current conversation history");
                break;
            }
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

            Console.WriteLine("Enter a file number or 'exit' to go back");
            string fileNumber = Console.ReadLine();

            if(fileNumber == "exit")
            {
                break;
            }

            // Parse string to int
            if (!(int.Parse(fileNumber) >= fileHandler.getNumberOfFiles()))
            {
                loadConversationHistory(fileNumber, conversationHistory);
                Console.WriteLine("File number " + fileNumber + " loaded");
                response = "   ";
                break;
            }
            else
            {
                Console.WriteLine("File number " + fileNumber + " does not exist");
                break;
            }

        case "10":

            Console.WriteLine("Enter a file number to delete");
            string fileToDelete = Console.ReadLine();

            // Parse string to int
            if (!(int.Parse(fileToDelete) >= fileHandler.getNumberOfFiles()))
            {
                fileHandler.deleteFile(int.Parse(fileToDelete));
                Console.WriteLine("File number " + fileToDelete + " deleted\nNOTE: Files are renumbered");
                break;
            }
            else
            {
                Console.WriteLine("File number " + fileToDelete + " does not exist");
                break;
            }

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
            new ChatRequestAssistantMessage("You are assisting developers translating code written in Smalltalk to C#. You will help to your best ability, and end every message with 'DONE' ")
        },
        MaxTokens = 3000
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
            new ChatRequestSystemMessage("You are assisting developers translating code written in Smalltalk to C#. You will explain what the code does and how it works but you will not reply with any translated code, and end every message with 'DONE'"),
            new ChatRequestUserMessage(code),
        },
        MaxTokens = 1500
    };
    Response<ChatCompletions> response = client.GetChatCompletions(chatCompletionsOptions);

    return response.Value.Choices[0].Message.Content;

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
    string userMessage = "";
    string assistantMessage = "";

    using (StreamReader sr = File.OpenText(path))
    {
        string s;
        bool userSection = false;
        bool assistantSection = false;
        string currentMessage = "";

        while ((s = sr.ReadLine()) != null)
        {


            if (s.Length > 6 && s.Substring(0, 6) == "(user,")
            {

                if (assistantSection == true)
                {
                    conversationHistory.Add(("assistant", assistantMessage));
                }
                s = s.Substring(6);
                conversationHistory.Add(("user", s));
                userSection = true;
                assistantSection = false;

            }
            else if (s.Length > 11 && s.Substring(0, 11) == "(assistant,")
            {
                s = s.Substring(11);
                assistantMessage += s + "\n";
                userSection = false;
                assistantSection = true;
            }
            else if (userSection == true)
            {
                userMessage += s;
            }
            else if (assistantSection == true)
            {
                assistantMessage += s+"\n";
            }

        }

    }

    conversationHistory.Add(("assistant", assistantMessage));

}
String readInputFromUser()
{
    String userInput = "";
    while (true)
    {
        string codeToConvert = Console.ReadLine();
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
