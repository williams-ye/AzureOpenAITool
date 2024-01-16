namespace azure_openai_quickstart
{
    internal class FileHandler
    {
        public void storeConversationHistory(List<(string, string)> conversationHistory)
        {

            int totalFiles = getNumberOfFiles();
            string path = @"./conversationHistory" + totalFiles + ".txt";

            using (StreamWriter sw = File.CreateText(path))
            {
                foreach (var message in conversationHistory)
                {
                    sw.WriteLine(message);
                }
            }
        }

        public int getAllFiles(string path, string prefix)
        {
            while (true)
            {
                int count = 0;
                List<string> files = new List<string>();

                foreach (string file in Directory.EnumerateFiles(path, prefix + "*"))
                {
                    files.Add(file);
                    count++;
                }
                Console.WriteLine("___________________________________________");

                for (int i = 0; i < files.Count; i++)
                {
                    Console.WriteLine(i + " " + files[i]);
                }

                Console.WriteLine("Select file to read by entering number, or enter 'exit' to go back");
                string input = Console.ReadLine();


                if (input == "exit")
                {
                    return count;
                }
                else if (!int.TryParse(input, out int number))
                {
                    Console.WriteLine("Please enter a number or 'exit'");
                    continue;
                }

                else if (number < 0 || number > files.Count - 1)
                {
                    Console.WriteLine("Please enter a number between 0 and " + (files.Count - 1));
                    continue;
                }
                else
                {
                    readConversationHistory(input);
                }
            }

        }
        public void readConversationHistory(string fileNumber)
        {
            Console.WriteLine("Reading file number " + fileNumber + ": ");

            string path = @"./conversationHistory" + fileNumber + ".txt";
            using (StreamReader sr = File.OpenText(path))
            {
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    Console.WriteLine(s);
                }
            }
        }


        public int getNumberOfFiles()
        {
            int count = 0;
            foreach (string file in Directory.EnumerateFiles("./", "conversationHistory*"))
            {
                count++;
            }

            return count;
        }

        public void deleteFile(int fileNumber)
        {
            string path = @"./conversationHistory" + fileNumber + ".txt";
            File.Delete(path);

            //renumber files
            int totalFiles = getNumberOfFiles();

            for (int i = fileNumber + 1; i <= totalFiles; i++)
            {
                string oldPath = @"./conversationHistory" + i + ".txt";
                string newPath = @"./conversationHistory" + (i - 1) + ".txt";
                File.Move(oldPath, newPath);
            }
        }
    }

}
