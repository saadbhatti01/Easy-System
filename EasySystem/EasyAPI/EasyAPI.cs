using System;
using System.Net.Http;
using System.Text;


namespace EasySystem.EasyAPI
{

    public class EasySysAPI
    {
        //private readonly GetAPI _myAPI;

        public HttpClient Initial()
        {
            var Client = new HttpClient();

            //Local Server Path
            Client.BaseAddress = new Uri("http://localhost:54383/api/");

            //Live Server Path
            //Client.BaseAddress = new Uri("https://api.leskills.com/api/");
            //Client.BaseAddress = new Uri("http://sajawal-001-site3.ctempurl.com/api/");

            //New Path
            //Client.BaseAddress = new Uri("http://fabricaerp-001-site3.gtempurl.com/api/");

            return Client;
        }

        public static string URLFriendly(string title)
        {
            if (title == null) return "";

            const int maxlen = 80;
            int len = title.Length;
            bool prevdash = false;
            var sb = new StringBuilder(len);
            char c;

            for (int i = 0; i < len; i++)
            {
                c = title[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                    prevdash = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    // tricky way to convert to lowercase
                    sb.Append((char)(c | 32));
                    prevdash = false;
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' ||
                    c == '\\' || c == '-' || c == '_' || c == '=')
                {
                    if (!prevdash && sb.Length > 0)
                    {
                        sb.Append('-');
                        prevdash = true;
                    }
                }
                else if ((int)c >= 128)
                {
                    int prevlen = sb.Length;
                    sb.Append(RemapInternationalCharToAscii(c));
                    if (prevlen != sb.Length) prevdash = false;
                }
                if (i == maxlen) break;
            }

            if (prevdash)
                return sb.ToString().Substring(0, sb.Length - 1);
            else
                return sb.ToString();
        }

        public static string RemapInternationalCharToAscii(char c)
        {
            string s = c.ToString().ToLowerInvariant();
            if ("àåáâäãåą".Contains(s))
            {
                return "a";
            }
            else if ("èéêëę".Contains(s))
            {
                return "e";
            }
            else if ("ìíîïı".Contains(s))
            {
                return "i";
            }
            else if ("òóôõöøőð".Contains(s))
            {
                return "o";
            }
            else if ("ùúûüŭů".Contains(s))
            {
                return "u";
            }
            else if ("çćčĉ".Contains(s))
            {
                return "c";
            }
            else if ("żźž".Contains(s))
            {
                return "z";
            }
            else if ("śşšŝ".Contains(s))
            {
                return "s";
            }
            else if ("ñń".Contains(s))
            {
                return "n";
            }
            else if ("ýÿ".Contains(s))
            {
                return "y";
            }
            else if ("ğĝ".Contains(s))
            {
                return "g";
            }
            else if (c == 'ř')
            {
                return "r";
            }
            else if (c == 'ł')
            {
                return "l";
            }
            else if (c == 'đ')
            {
                return "d";
            }
            else if (c == 'ß')
            {
                return "ss";
            }
            else if (c == 'Þ')
            {
                return "th";
            }
            else if (c == 'ĥ')
            {
                return "h";
            }
            else if (c == 'ĵ')
            {
                return "j";
            }
            else
            {
                return "";
            }
        }
    }

    //public static class IsLocalExtension
    //{
    //    private const string NullIpAddress = "::1";

    //    public static bool IsLocal(this HttpRequest req)
    //    {
    //        var connection = req.HttpContext.Connection;
    //        if (connection.RemoteIpAddress.IsSet())
    //        {
    //            //We have a remote address set up
    //            return connection.LocalIpAddress.IsSet()
    //                //Is local is same as remote, then we are local
    //                ? connection.RemoteIpAddress.Equals(connection.LocalIpAddress)
    //                //else we are remote if the remote IP address is not a loopback address
    //                : IPAddress.IsLoopback(connection.RemoteIpAddress);
    //        }

    //        return true;
    //    }

    //    private static bool IsSet(this IPAddress address)
    //    {
    //        return address != null && address.ToString() != NullIpAddress;
    //    }
    //}

}
