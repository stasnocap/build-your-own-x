
using System.Net;
using System.Text;

var pageViews = 0;
var page = """
           <DOCTYPE! html>
           <body>
                Page views: {0}
                <form action="shutdown" method="get">
                    <button type="submit">Shutdown</button>
                </form>
           </body>
           """;

using var listener = new HttpListener();

listener.Prefixes.Add("http://localhost:12345/");

listener.Start();

while (true)
{
    var context = await listener.GetContextAsync();

    if (context.Request.Url?.AbsolutePath == "/shutdown")
    {
        break;
    }

    if (context.Request.Url?.AbsolutePath != "/favicon.ico")
    {
        pageViews++;
    }

    var formattedPage = string.Format(page, pageViews);

    var bytes = Encoding.UTF8.GetBytes(formattedPage);

    context.Response.ContentType = "text/html";

    await context.Response.OutputStream.WriteAsync(bytes);

    context.Response.Close();
}

listener.Close();