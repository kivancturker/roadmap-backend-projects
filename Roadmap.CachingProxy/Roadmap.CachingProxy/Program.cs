using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualBasic.CompilerServices;

namespace Roadmap.CachingProxy;

/*
 * PROJECT DESCRIPTION
 * It is a CLI tool that redirect the request to a target url and cache the response.
 * Example Command: caching-proxy --port <number> --origin <url>
 * Even after program termination cached responses are stored and reused each program execution.
 * If --clear-cache provided as argument when starting program then it removes the stored responses.
 * If --clear-cache provided without other arguments that means program only clears the cache and
 * doesn't start caching feature.
 * Example: caching-proxy --clear-cache
 */


/*
 * TODO: (DONE) Initialize git and push the initial project.
 * TODO: (DONE) Program should accept the arguments as expected.
 * TODO: (DONE) Start forwarding requests to the target url without caching.
 * TODO: (DONE) Start InMemory caching. No store.
 * TODO: (DONE) Store the cached responses.
 * TODO: Remove the stored cached responses if --clear-cache flag provided.
 */

class Program
{
    private const int DEFAULT_PORT = 3030;
    static async Task Main(string[] args)
    {
        if (!HttpListener.IsSupported)
        {
            Console.WriteLine("Http Listener Does not support in this OS.");
        }
        // Parsing arguments step
        int? port;
        string origin;
        bool isClearCache;
        Argument argument = new Argument();
        CacheManager cacheManager = new CacheManager();
        try
        {
            (port, origin, isClearCache) = argument.ParseArguments(args);

            // Forwarding step
            // Listen on provided port
            int validPort = port ?? DEFAULT_PORT;
            HttpListener listener = StartServer(validPort);

            HttpClient httpClient = new HttpClient();
            Uri baseUri = new Uri(origin);
            // Get Request
            // https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistener?view=net-9.0&redirectedfrom=MSDN
            while (listener.IsListening)
            {
                // Get request
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                CachedRequest cachedRequest = new CachedRequest(request.HttpMethod, request.Url.PathAndQuery);

                // Check if request is made before
                if (cacheManager.Contains(cachedRequest))
                {
                    HttpResponseMessage cachedResponse = cacheManager.Get(cachedRequest);
                    await SendResponse(cachedResponse, context, true);
                    continue;
                }

                HttpRequestMessage requestToForward =
                    ConvertHttpListenerRequestToHttpRequestMessage(request, baseUri);

                // Forward the requst to the origin
                HttpResponseMessage responseFromOrigin = httpClient.Send(requestToForward);
                cacheManager.Add(cachedRequest, responseFromOrigin);
                cacheManager.Persist();
                await SendResponse(responseFromOrigin, context, false);
            }
        }
        catch (CustomException exception)
        {
            Console.Error.WriteLine(exception.Message);
            Environment.Exit(1);
        }
        finally
        {
            cacheManager.Persist();
        }
    }

    private static HttpListener StartServer(int port)
    {
        string url = $"http://localhost:{port}/";
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add(url);
        listener.Start();
        if (listener.IsListening)
        {
            Console.WriteLine($"Listening on {url}");
            return listener;
        }
        throw new CustomException($"Problem While listening on {url}");
    }

    private static HttpRequestMessage ConvertHttpListenerRequestToHttpRequestMessage(HttpListenerRequest request, Uri baseUri)
    {
        Uri uri = new Uri(baseUri, request.Url.AbsolutePath);
        HttpRequestMessage requestMessage = new HttpRequestMessage();
        requestMessage.RequestUri = uri;
        HttpMethod method = new HttpMethod(request.HttpMethod);
        requestMessage.Method = method;

        return requestMessage;
    }

    private static async Task SendResponse(HttpResponseMessage responseMessage, HttpListenerContext context, bool isCacheHit = false) 
    {
        HttpListenerResponse response = context.Response;
        byte[] buffer = await responseMessage.Content.ReadAsByteArrayAsync();
        response.ContentLength64 = buffer.Length;
        if (isCacheHit)
        {
            response.Headers.Add("X-Cache", "HIT");
        }
        else
        {
            response.Headers.Add("X-Cache", "MISS");
        }
        
        using Stream output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();
    }

}