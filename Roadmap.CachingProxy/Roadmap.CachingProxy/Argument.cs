using System.Text;

namespace Roadmap.CachingProxy;

public class Argument
{
    private HashSet<string> validFlags = new HashSet<string>()
    {
        ArgumentOption.Port, 
        ArgumentOption.Origin, 
        ArgumentOption.ClearCache
    };
    
    public (int? port, string origin, bool isClearCache) ParseArguments(string[] args)
    {
        int? port = null;
        string origin = String.Empty;
        bool isClearCache = false;

        bool isPort = false;
        bool isOrigin = false;

        bool isPrevFlag = false;
        
        // Remove program name
        if (args.Length == 0)
            throw new CustomException($"Arguments are missing.\n Valid arguments are {ValidArguemntsInOrder()}");
        List<string> argsList = args.ToList();

        argsList.ForEach((string arg) =>
        {
            if (IsFlag(arg) && isPrevFlag)
            {
                throw new CustomException("You can't pass consecutive flags");
            }
            
            if (IsFlag(arg))
            {
                #region Actions according to given arg
                switch (arg)
                {
                    case ArgumentOption.Port:
                        isPort = true;
                        break;
                    case ArgumentOption.Origin:
                        isOrigin = true;
                        break;
                    case ArgumentOption.ClearCache:
                        isClearCache = true;
                        break;
                    default:
                        throw new CustomException($"{arg} is not a valid option.\nValid options are {ValidArguemntsInOrder()}");
                }
                isPrevFlag = true;
                #endregion
            }
            
            else
            {
                if (isPort)
                {
                    port = Int32.Parse(arg);
                    isPort = false;
                }

                if (isOrigin)
                {
                    origin = arg;
                    isOrigin = false;
                }

                isPrevFlag = false;
            }
        });

        return (port, origin, isClearCache);
    }

    public void CheckArgumentsValidity(int? port, string? originUrl)
    {
        throw new NotImplementedException();
    }

    private string ValidArguemntsInOrder()
    {
        StringBuilder builder = new StringBuilder();
        validFlags.ToList().ForEach(flag => builder.Append(flag + ", "));
        return builder.ToString();
    }

    private bool IsFlag(string arg)
    {
        return arg.StartsWith("--");
    }

    private bool IsValidFlag(string flag)
    {
        return validFlags.Contains(flag);
    }
}

file record ArgumentOption
{
    public const string Port = "--port";
    public const string Origin = "--origin";
    public const string ClearCache = "--clear-cache";
}