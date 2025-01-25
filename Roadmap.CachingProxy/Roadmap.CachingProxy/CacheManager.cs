using System.Net;

namespace Roadmap.CachingProxy;

public class CacheManager
{
    private Dictionary<CachedRequest, HttpResponseMessage> _cache;

    public CacheManager()
    {
        _cache = new Dictionary<CachedRequest, HttpResponseMessage>();
    }

    public bool Contains(CachedRequest request)
    {
        return _cache.ContainsKey(request);
    }

    public void Add(CachedRequest request, HttpResponseMessage response)
    {
        _cache.Add(request, response);
    }

    public HttpResponseMessage Get(CachedRequest request)
    {
        if (_cache.TryGetValue(request, out var response))
        {
            return response;
        }

        throw new CustomException("Request not found on cache");
    }
}

public record CachedRequest(string Method, string PathAndQuery);