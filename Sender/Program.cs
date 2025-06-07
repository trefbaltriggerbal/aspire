using System.Net.Http;

var client = new HttpClient();
Console.WriteLine("Sending request to receiver...");
var response = await client.GetStringAsync("http://localhost:5000/ping");
Console.WriteLine($"Received response: {response}");
