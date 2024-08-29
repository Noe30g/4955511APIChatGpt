
using Newtonsoft.Json;
using System.Text;

namespace _4955511APIChatGpt
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _client;
        private const string ApiKey = "ApiKey";
        public MainPage()
        {
            InitializeComponent();
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
        }

        private async void OnAskButtonClicked(object sender, EventArgs e)
        {
            var query = userInput.Text;
            var response = await GetChatGptResponse(query);
            responseLabel.Text = response;
        }

        private async Task<string> GetChatGptResponse(string query)
        {
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "user", content = query }
                }
            };

            var jsonContent = JsonConvert.SerializeObject(requestBody);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await _client.PostAsync("https://api.openai.com/v1/chat/completions", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var gptResponse = JsonConvert.DeserializeObject<GptResponse>(responseBody);
                    return gptResponse?.choices?.FirstOrDefault()?.message?.content ?? "No response";
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return $"Failed to get response from ChatGPT. Status Code: {response.StatusCode}, Error: {errorMessage}";
                }
            }
            catch (HttpRequestException httpRequestException)
            {
                return $"Request error: {httpRequestException.Message}";
            }
            catch (Exception ex)
            {
                return $"Unexpected error: {ex.Message}";
            }
        }



    }
}