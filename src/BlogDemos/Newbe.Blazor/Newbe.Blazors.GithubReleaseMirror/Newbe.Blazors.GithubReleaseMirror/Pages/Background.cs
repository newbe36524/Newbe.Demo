using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Newbe.Blazors.GithubReleaseMirror.Pages
{
    public partial class Background
    {
        // e.g. https://github.com/dapr/cli/releases/download/v1.1.0/dapr_windows_amd64.zip
        private static readonly Regex ReleaseRegex = new Regex(
            @"https://github.com/(?<author>\w+)/(?<repo>\w+)/releases/download/(?<tag>\S+)/(?<file>\S+)",
            RegexOptions.Compiled, TimeSpan.FromMilliseconds(500));

        [JSInvokable]
        public static string GetGithubReleaseMirrorUrl(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }

            try
            {
                var match = ReleaseRegex.Match(source);
                if (match.Success)
                {
                    // e.g. "https://github.com.cnpmjs.org/dapr/cli/releases/download/$($release.tag_name)/dapr_windows_amd64.zip"
                    var mirror =
                        $"https://github.com.cnpmjs.org/{match.Groups["author"]}/{match.Groups["repo"]}/releases/download/{match.Groups["tag"]}/{match.Groups["file"]}";
                    return mirror;
                }

                return source;
            }
            catch (Exception e)
            {
                return source;
            }
        }
    }
}