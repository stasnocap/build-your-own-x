using var httpClient = new HttpClient();

var response = await httpClient.GetAsync("https://google.com");

var result = await response.Content.ReadAsByteArrayAsync();

using var fileStream = File.Create("index.html");

await fileStream.WriteAsync(result);