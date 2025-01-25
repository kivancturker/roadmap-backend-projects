using System.Net;
using System.Net.Security;

namespace Roadmap.CachingProxy;

public class CacheFileManager
{
    private const string FILENAME = ".proxyCache";

    public void SaveCacheContentToFile(Dictionary<CachedRequest, HttpResponseMessage> content)
    {
        using var stream = new FileStream(FILENAME, FileMode.OpenOrCreate, FileAccess.Write);
        using var writer = new StreamWriter(stream);
        var serializedObject = Newtonsoft.Json.JsonConvert.SerializeObject(content);
        writer.Write(serializedObject);
    }

    public Dictionary<CachedRequest, HttpResponseMessage> GetAllCachedRequests()
    {
        using var stream = new FileStream(FILENAME, FileMode.OpenOrCreate, FileAccess.Read);
        using var reader = new StreamReader(stream);
        string fileContent = reader.ReadToEnd();
        Dictionary<CachedRequest, HttpResponseMessage> deserializedObject = Newtonsoft.Json.JsonConvert
            .DeserializeObject<Dictionary<CachedRequest, HttpResponseMessage>>(fileContent) ?? new Dictionary<CachedRequest, HttpResponseMessage>();
        
        return deserializedObject;
    }

    public void Remove()
    {
        File.Delete(FILENAME);
    }
}