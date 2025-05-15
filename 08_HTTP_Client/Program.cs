
using System.Text;
using System.Text.Json;

class Post
{
    public string Title { get; set; }
    public string Body { get; set; }
    public int UserId { get; set; }
}
internal class Program
{
    private static async Task Main(string[] args)
    {
        /*var url = "https://jsonplaceholder.typicode.com/users";*/
        /*using var client = new HttpClient();

        var response = await client.GetAsync(url);
        Console.WriteLine($"Status - {response.StatusCode} {response.RequestMessage}");
        var res = await response.Content.ReadAsStringAsync();
        Console.WriteLine(res);*/

        var post = new Post()
        { 
            Title = "TestTitle",
            Body = "TestBody",
            UserId = 1
        };
        var url = "https://jsonplaceholder.typicode.com/posts";

        var json = JsonSerializer.Serialize(post);
        var data = new StringContent(json, Encoding.UTF8, "appication/json");
        using var client = new HttpClient();
        var response = await client.PostAsync(url, data);
        Console.WriteLine($"Status - {response.StatusCode}");
        var res = await response.Content.ReadAsStringAsync();
        Console.WriteLine(res);
    }
}