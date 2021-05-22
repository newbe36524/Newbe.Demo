using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MLS.WasmCodeRunner;
using Newtonsoft.Json.Linq;
using WebExtension.Net.WebRequest;

namespace Newbe.Blazors.TryNet.Pages
{
    public partial class Index
    {
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            Code = @"using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(""Hello World!"");
        }
    }
}";
            runner = new CodeRunner(s => Logger.LogInformation(s));
        }

        public string Code { get; set; }
        public string CodeResult { get; set; }
        public string ExeBin { get; set; }
        public int Sequence { get; set; }
        [Inject] public IWebAssemblyHostEnvironment WebAssemblyHostEnvironment { get; set; }

        private async Task<string> CompileAndEncode(string text, params MetadataReference[] additionalReferences)
        {
            var compilation = await Compile(text);

            await using var stream = new MemoryStream();
            compilation.Emit(peStream: stream);
            var encodedAssembly = Convert.ToBase64String(stream.ToArray());
            return encodedAssembly;
        }

        private static HttpRequestMessage SetUri(HttpRequestMessage message, Uri uri)
        {
            var method =
                typeof(HttpRequestMessage).GetField("_requestUri", BindingFlags.NonPublic | BindingFlags.Instance);
            method.SetValue(message, uri);
            return message;
        }

        private static readonly HttpClient http = new();

        private static HashSet<string> baseAssemblyNames = new[]
            {
                typeof(AssemblyTargetedPatchBandAttribute).Assembly, // System.Private.CoreLib
                typeof(Uri).Assembly, // System.Private.Uri
                typeof(Console).Assembly, // System.Console
                typeof(IQueryable).Assembly, // System.Linq.Expressions
                typeof(HttpClient).Assembly, // System.Net.Http
                typeof(HttpClientJsonExtensions).Assembly, // System.Net.Http.Json
                typeof(RequiredAttribute).Assembly, // System.ComponentModel.Annotations
                typeof(Regex).Assembly, // System.Text.RegularExpressions
                typeof(NavLink).Assembly, // Microsoft.AspNetCore.Components.Web
                typeof(WebAssemblyHostBuilder).Assembly, // Microsoft.AspNetCore.Components.WebAssembly
            }
            .SelectMany(a => a.GetReferencedAssemblies().Concat(new[] {a.GetName()}))
            .Select(an => an.Name)
            .ToHashSet();

        private async Task<Compilation> Compile(string text, params MetadataReference[] additionalReferences)
        {
            var baseAddress = WebAssemblyHostEnvironment.BaseAddress;
            var refsStreams = await Task.WhenAll(baseAssemblyNames
                .Select(async assembly =>
                {
                    var message = new HttpRequestMessage(HttpMethod.Get, default(string));
                    var requestUri = new Uri($"{baseAddress}framework/{assembly}.dll");
                    message = SetUri(message, requestUri);
                    var responseMessage = await http.SendAsync(message);
                    return await responseMessage.Content.ReadAsStreamAsync();
                }));
            var refs = refsStreams
                .Select(stream =>
                    MetadataReference.CreateFromStream(stream));
            return CSharpCompilation.Create("assembly.dll", new[] {CSharpSyntaxTree.ParseText(text)}, refs,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        }

        public async Task OnClickRunAsync()
        {
            #region Body

            var body = new WasmCodeRunnerRequest
            {
                Succeeded = true,
                Base64Assembly =
                    "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mwAeAUABmwAeAWABmwAeAUIAaQBvAAMAAQAAAAYEmAAAAEMEnAACAAcAAwACAAkABQADAAoAGQADAAwAGwADAA4AHQADABAAHwADABIAIQADABQAIwADABYAJQAfAC4ABIAAAAEAAAAAAAAAAAAAAAAAzAAAAAQAAgABAAAAAAAAAHQA1AAAAAAABAABAAEAAAAAAAAAdABmAgAAAAAEAAIAAQAAAAAAAAB0ANECAAAAAAQAAQABAAAAAAAAAHQAvQAAAAAABAACAAEAAAAAAAAAdAChAwAAAAADAAIAKQBcAAAAADxjdXJyZW50PjVfXzEAPEZpYm9uYWNjaT5kX18xAElFbnVtZXJhYmxlYDEASUVudW1lcmF0b3JgMQBJbnQzMgA8bmV4dD41X18yADxNb2R1bGU+AFN5c3RlbS5Db2xsZWN0aW9ucy5HZW5lcmljAGdldF9DdXJyZW50TWFuYWdlZFRocmVhZElkADw+bF9faW5pdGlhbFRocmVhZElkAFRha2UASUVudW1lcmFibGUASURpc3Bvc2FibGUAU3lzdGVtLkNvbnNvbGUAY29uc29sZQBTeXN0ZW0uUnVudGltZQBXcml0ZUxpbmUAVHlwZQBTeXN0ZW0uSURpc3Bvc2FibGUuRGlzcG9zZQA8PjFfX3N0YXRlAENvbXBpbGVyR2VuZXJhdGVkQXR0cmlidXRlAERlYnVnZ2FibGVBdHRyaWJ1dGUAQXNzZW1ibHlUaXRsZUF0dHJpYnV0ZQBJdGVyYXRvclN0YXRlTWFjaGluZUF0dHJpYnV0ZQBEZWJ1Z2dlckhpZGRlbkF0dHJpYnV0ZQBBc3NlbWJseUZpbGVWZXJzaW9uQXR0cmlidXRlAEFzc2VtYmx5SW5mb3JtYXRpb25hbFZlcnNpb25BdHRyaWJ1dGUAQXNzZW1ibHlDb25maWd1cmF0aW9uQXR0cmlidXRlAENvbXBpbGF0aW9uUmVsYXhhdGlvbnNBdHRyaWJ1dGUAQXNzZW1ibHlQcm9kdWN0QXR0cmlidXRlAEFzc2VtYmx5Q29tcGFueUF0dHJpYnV0ZQBSdW50aW1lQ29tcGF0aWJpbGl0eUF0dHJpYnV0ZQBTeXN0ZW0uRGlhZ25vc3RpY3MuRGVidWcARmlib25hY2NpAGNvbnNvbGUuZGxsAFByb2dyYW0AU3lzdGVtAE1haW4AU3lzdGVtLlJlZmxlY3Rpb24ATm90U3VwcG9ydGVkRXhjZXB0aW9uAFN5c3RlbS5MaW5xAElFbnVtZXJhdG9yAFN5c3RlbS5Db2xsZWN0aW9ucy5HZW5lcmljLklFbnVtZXJhYmxlPFN5c3RlbS5JbnQzMj4uR2V0RW51bWVyYXRvcgBTeXN0ZW0uQ29sbGVjdGlvbnMuSUVudW1lcmFibGUuR2V0RW51bWVyYXRvcgAuY3RvcgBTeXN0ZW0uRGlhZ25vc3RpY3MAU3lzdGVtLlJ1bnRpbWUuQ29tcGlsZXJTZXJ2aWNlcwBEZWJ1Z2dpbmdNb2RlcwBTeXN0ZW0uUnVudGltZS5FeHRlbnNpb25zAFN5c3RlbS5Db2xsZWN0aW9ucwBPYmplY3QAU3lzdGVtLkNvbGxlY3Rpb25zLklFbnVtZXJhdG9yLlJlc2V0AEVudmlyb25tZW50AFN5c3RlbS5Db2xsZWN0aW9ucy5HZW5lcmljLklFbnVtZXJhdG9yPFN5c3RlbS5JbnQzMj4uQ3VycmVudABTeXN0ZW0uQ29sbGVjdGlvbnMuSUVudW1lcmF0b3IuQ3VycmVudABTeXN0ZW0uQ29sbGVjdGlvbnMuR2VuZXJpYy5JRW51bWVyYXRvcjxTeXN0ZW0uSW50MzI+LmdldF9DdXJyZW50AFN5c3RlbS5Db2xsZWN0aW9ucy5JRW51bWVyYXRvci5nZXRfQ3VycmVudAA8PjJfX2N1cnJlbnQATW92ZU5leHQAAAAAAPTh3kBkgaRGgtnPq+FiClEABCABAQgDIAABBSABARERBCABAQ4FIAEBEjUDIAACBRUSMQEIBCAAEwADIAAcBRUSPQEICCAAFRIxARMABCAAEkkIBwIVEjEBCAgQEAECFRI9AR4AFRI9AR4ACAMKAQgEAAEBCAMAAAgFBwMICAIEBwESDAiwP19/EdUKOgIGCAMAAAEHAAAVEj0BCAMgAAgHIAAVEjEBCAMoAAgDKAAcCAEACAAAAAAAHgEAAQBUAhZXcmFwTm9uRXhjZXB0aW9uVGhyb3dzAQgBAAcBAAAAAAwBAAdjb25zb2xlAAAKAQAFRGVidWcAAAwBAAcxLjAuMC4wAAAKAQAFMS4wLjAAABwBABdQcm9ncmFtKzxGaWJvbmFjY2k+ZF9fMQAABAEAAAAAAAAAAAAAAAAAAAAAEAAAAAAAAAAAAAAAAAAAAFQsAAAAAAAAAAAAAG4sAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAABgLAAAAAAAAAAAAAAAAF9Db3JFeGVNYWluAG1zY29yZWUuZGxsAAAAAAD/JQAgQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAMAAAAgDwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"
            };

            #endregion

            var assembly = await CompileAndEncode(Code);
            body.Base64Assembly = assembly;
            ExeBin = assembly;

            Sequence++;
            var message = new InteropMessage<WasmCodeRunnerRequest>(Sequence, body);
            CodeResult = await PostMessageAsync(JsonSerializer.Serialize(message));
        }

        #region Runner

        [Inject] public IJSRuntime jsRuntime { get; set; }

        private CodeRunner runner;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await jsRuntime.InvokeAsync<object>(
                    "BlazorInterop.install",
                    DotNetObjectReference.Create(this));

                await PostMessage(JObject.FromObject(new {ready = true}).ToString());
            }
        }

        public ValueTask<string> PostMessage(string message)
        {
            // Implemented in interop.js
            return jsRuntime.InvokeAsync<string>
                ("BlazorInterop.postMessage", message);
        }

        [JSInvokable("MLS.Blazor.PostMessageAsync")]
        public async Task<string>
            PostMessageAsync(string message)
        {
            try
            {
                var result = runner.ProcessRunRequest(message);
                var logMsg = $"Computed {JObject.FromObject(result)}";
                await jsRuntime.InvokeAsync<string>("BlazorInterop.log", logMsg);

                if (result != null)
                {
                    return JObject.FromObject(result).ToString();
                    // await PostMessage(JObject.FromObject(result).ToString());
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "failed to run");
            }

            return null;
        }

        #endregion
    }
}