using System.Linq;
using System.Threading.Tasks;
using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;

namespace Newbe.Blazors.Slider.Pages
{
    public partial class Index
    {
        private Carousel _carousel;
        private readonly int _count = 38;
        private int _height = 800;
        [Inject] public IWebAssemblyHostEnvironment Environment { get; set; }
        public string[] ImgUrls { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            ImgUrls = Enumerable.Range(1, 38).Select(j => $"{_environment.BaseAddress}img/Slide{j}.PNG").ToArray();
        }

        private void OnKeyDownAsync(KeyboardEventArgs args)
        {
            Logger.LogInformation(args.Code.ToString());
            switch (args.Code.ToString())
            {
                case "ArrowRight":
                    _carousel.Next();
                    break;
                case "ArrowLeft":
                    _carousel.Previous();
                    break;
            }
        }
    }
}