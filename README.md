# TextSimilarity
Text similarity processor using GPT-3.5-turbo-instructor and embeddings.

This is a C# console application.

A requirement to run is the OpenAI API key.  The key is not exposed via the code but rather is stored as an environment variable.  To store your API key in an environment key (Windows 11), open the Command Prompt and run the command setx OPENAI_API_KEY "your_api_key" (replace "your_api_key" with your actual API key value).  To verify that the value was saved, you can run the command echo %OPENAI_API_KEY% to see %OPENAI_API_KEY%, which indicates the value was saved.
