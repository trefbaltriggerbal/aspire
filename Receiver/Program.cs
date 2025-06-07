using System.Net;

var listener = new HttpListener();
listener.Prefixes.Add("http://localhost:5000/");
listener.Start();
Console.WriteLine("Receiver listening on http://localhost:5000/");

while (true)
{
    var context = await listener.GetContextAsync();
    if (context.Request.Url?.AbsolutePath == "/ping")
    {
        var responseString = "Pong";
        var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        context.Response.ContentLength64 = buffer.Length;
        await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        context.Response.Close();
        Console.WriteLine("Responded with Pong");
    }
}
