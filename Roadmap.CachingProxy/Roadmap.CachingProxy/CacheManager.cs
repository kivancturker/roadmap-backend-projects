using System.Net;

namespace Roadmap.CachingProxy;

public class CacheManager
{
    private Dictionary<CachedRequest, HttpResponseMessage> _cache;
    private CacheFileManager _cacheFileManager;
    
    public CacheManager()
    {
        _cacheFileManager = new CacheFileManager();
        _cache = _cacheFileManager.GetAllCachedRequests();
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

    public void Persist()
    {
        _cacheFileManager.SaveCacheContentToFile(_cache);
    }
}

public record CachedRequest(string Method, string PathAndQuery);