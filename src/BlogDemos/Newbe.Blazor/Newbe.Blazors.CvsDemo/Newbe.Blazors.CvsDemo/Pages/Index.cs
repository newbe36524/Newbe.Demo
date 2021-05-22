using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Components;
using Newbe.Blazors.CvsDemo.Apis;

namespace Newbe.Blazors.CvsDemo.Pages
{
    public partial class Index
    {
        [Inject] public IDaprReleaseApi DaprReleaseApi { get; set; }
        private async Task OnClickExportAsync()
        {
            var releases = await DaprReleaseApi.GetLatestReleaseAsync();

            var releaseTableItems = releases
                .OrderByDescending(x => x.CreatedAt)
                .SelectMany(r => r.Assets.Select(a => new ReleaseTableItem
                {
                    File = a.Name,
                    Tag = r.TagName,
                    DownloadUrl = a.BrowserDownloadUrl?.ToString()
                }))
                .ToArray();
            _releaseFiles = releaseTableItems;
            StateHasChanged();
            
            await using var ms = new MemoryStream();
            await using var streamWriter = new StreamWriter(ms);
            await using var csvWriter = new CsvWriter(streamWriter, new CsvConfiguration(CultureInfo.CurrentCulture));
            await csvWriter.WriteRecordsAsync(releaseTableItems);
            await csvWriter.FlushAsync();
            _csvFile = Convert.ToBase64String(ms.ToArray());
        }
        
        private ReleaseTableItem[] _releaseFiles;
        private string _csvFile;

        public record ReleaseTableItem
        {
            public string Tag { get; set; }
            public string File { get; set; }
            public string DownloadUrl { get; set; }
        }
    }
}