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
 * TODO: Initialize git and push the initial project.
 * TODO: Program should accept the arguments as expected.
 * TODO: Start forwarding requests to the target url without caching.
 * TODO: Start InMemory caching. No store.
 * TODO: Store the cached responses.
 * TODO: Remove the stored cached responses if --clear-cache flag provided.
 */

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }
}