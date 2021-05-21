using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using WebExtension.Net.WebRequest;

namespace Newbe.Blazors.GithubReleaseMirror.Pages
{
    public partial class Background
    {
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await WebExtension.WebRequest.OnBeforeRequest.AddListener(details =>
            {
                var d = (JsonElement)details;
                var url = d.GetProperty("url").ToString();
                var targetUrl = GetGithubReleaseMirrorUrl(url);

                return new BlockingResponse
                {
                    RedirectUrl = targetUrl,
                };
            }, new RequestFilter
            {
                Urls = new[] {"*://github.com/*"},
            }, new[]
            {
                OnBeforeRequestOptions.Blocking
            });
            // this opens index.html in the extension as a new tab when the background page is loaded
            var extensionUrl = await WebExtension.Runtime.GetURL("index.html");
            await WebExtension.Tabs.Create(new
            {
                url = extensionUrl
            });
        }

        // e.g. https://github.com/dapr/cli/releases/download/v1.1.0/dapr_windows_amd64.zip
        private static readonly Regex ReleaseRegex = new(
            @"https://github.com/(?<author>\w+)/(?<repo>\w+)/releases/download/(?<tag>\S+)/(?<file>\S+)",
            RegexOptions.Compiled, 
            TimeSpan.FromMilliseconds(500));

        private string GetGithubReleaseMirrorUrl(string source)
        {
            Logger.LogInformation("url: {Source}",source);
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }

            try
            {
                var match = ReleaseRegex.Match(source);
                if (match.Success)
                {
                    Logger.LogInformation("match!");
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