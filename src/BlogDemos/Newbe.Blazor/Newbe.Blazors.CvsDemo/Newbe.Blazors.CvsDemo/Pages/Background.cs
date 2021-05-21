using System.Threading.Tasks;

namespace Newbe.Blazors.CvsDemo.Pages
{
    public partial class Background
    {
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            // this opens index.html in the extension as a new tab when the background page is loaded
            var extensionUrl = await WebExtension.Runtime.GetURL("/");
            await WebExtension.Tabs.Create(new
            {
                url = extensionUrl
            });
        }

    }
}