using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;

class Program
{
	private static readonly HttpClient httpClient = new HttpClient();
	private static readonly Uri endpoint = new Uri("http://localhost:5000/");

	static async Task Main(string[] args)
	{
		Console.WriteLine("Testing sentiment analysis...");
		await SentimentAnalysisExample("I had the worst day of my life. I wish you were there with me.");
		await SentimentAnalysisExample("I had the best day of my life. I wish you were there with me.");
		
		Console.Write("Press any key to exit.");
		Console.ReadKey();
	}

	static async Task SentimentAnalysisExample(string inputText)
	{
		try
		{
			// Create request using Azure Text Analytics API format
			var requestData = new
			{
				documents = new[]
				{
					new
					{
						id = "1",
						language = "en",
						text = inputText
					}
				}
			};

			var json = JsonSerializer.Serialize(requestData);
			var content = new StringContent(json);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			
			// Use the Azure Text Analytics v3.0 sentiment endpoint
			var url = $"{endpoint.ToString().TrimEnd('/')}/text/analytics/v3.0/sentiment";
			
			var response = await httpClient.PostAsync(url, content);
			
			if (response.IsSuccessStatusCode)
			{
				var responseContent = await response.Content.ReadAsStringAsync();
				Console.WriteLine($"Input: \"{inputText}\"");
				ParseAndDisplayResults(responseContent);
			}
			else
			{
				Console.WriteLine($"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error analyzing sentiment: {ex.Message}");
		}
	}

	static void ParseAndDisplayResults(string jsonResponse)
	{
		try
		{
			using var document = JsonDocument.Parse(jsonResponse);
			var root = document.RootElement;
			
			if (root.TryGetProperty("documents", out var documents) && documents.GetArrayLength() > 0)
			{
				var firstDoc = documents[0];
				
				if (firstDoc.TryGetProperty("sentiment", out var sentiment))
				{
					Console.WriteLine($"Document sentiment: {sentiment.GetString()}");
					
					if (firstDoc.TryGetProperty("confidenceScores", out var scores))
					{
						if (scores.TryGetProperty("positive", out var positive))
							Console.WriteLine($"Positive score: {positive.GetDouble():0.00}");
						if (scores.TryGetProperty("negative", out var negative))
							Console.WriteLine($"Negative score: {negative.GetDouble():0.00}");
						if (scores.TryGetProperty("neutral", out var neutral))
							Console.WriteLine($"Neutral score: {neutral.GetDouble():0.00}");
					}
				}
			}
			Console.WriteLine();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error parsing response: {ex.Message}");
		}
	}
}