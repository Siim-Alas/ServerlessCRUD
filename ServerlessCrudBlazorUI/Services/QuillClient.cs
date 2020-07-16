using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ServerlessCrudBlazorUI.Services.JSInteropHelpers;
using System.Threading.Tasks;

namespace ServerlessCrudBlazorUI.Services
{
    public class QuillClient
    {
        private readonly IJSRuntime _JSRuntime;

        public QuillClient(IJSRuntime jSRuntime)
        {
            _JSRuntime = jSRuntime;
        }

        public async Task InitAsync(ElementReference toolbarReference, ElementReference editorReference)
        {
            await _JSRuntime.InvokeVoidAsync("QuillClient.initQuill", toolbarReference, editorReference);
        }
        public async Task<object> InsertImage(ElementReference editorReference, string imageUrl)
        {
            return await _JSRuntime.InvokeAsync<object>("QuillClient.insertImage", editorReference, imageUrl);
        }
        public async Task<MarkupString> GetHTML(ElementReference editorReference)
        {
            return (MarkupString)await _JSRuntime.InvokeAsync<string>("QuillClient.getHTML", editorReference);
        }
    }
}
