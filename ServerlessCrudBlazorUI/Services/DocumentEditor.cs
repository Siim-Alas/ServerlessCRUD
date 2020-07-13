using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServerlessCrudBlazorUI.Services
{
    public class DocumentEditor
    {
        private readonly IJSRuntime _JSRuntime;

        public DocumentEditor(IJSRuntime jSRuntime)
        {
            _JSRuntime = jSRuntime;
        }

        public enum Commands
        {
            backColor,
            bold,
            copy,
            createLink,
            cut,
            defaultParagraphSeparator,
            delete,
            fontName,
            fontSize,
            foreColor,
            formatBlock,
            forwardDelete,
            indent,
            insertHorizontalRule,
            insertHTML,
            insertImage,
            insertLineBreak,
            insertOrderedList,
            insertParagraph,
            insertText,
            insertUnorderedList,
            italic,
            justifyCenter,
            justifyFull,
            justifyLeft,
            justifyRight,
            outdent,
            paste,
            redo,
            selectAll,
            strikethrough,
            styleWithCss,
            subscript,
            superscript,
            underline,
            undo,
            unlink,
            useCSS
        }

        public static Dictionary<string, string> Formats { get; } = new Dictionary<string, string>()
        {
            { "Heading 1", "h1" },
            { "Heading 2", "h2" },
            { "Heading 3", "h3" },
            { "Heading 4", "h4" },
            { "Heading 5", "h5" },
            { "Heading 6", "h6" },
            { "Paragraph", "p" },
            { "Preformatted", "pre" }
        };
        public static string[] Fonts { get; } = new string[]
        {
            "Arial", 
            "Arial Black", 
            "Courier New", 
            "Times New Roman"
        };
        public static string[] FontSizes { get; } = new string[]
        {
            "1", 
            "2", 
            "3", 
            "4", 
            "5", 
            "6", 
            "7"
        };
        public string[] Colors { get; } = new string[]
        {
            "red", 
            "green", 
            "blue", 
            "black"
        };

        public async Task ExecEditCommand(Commands command, string argument = null)
        {
            await _JSRuntime.InvokeVoidAsync("DocumentEditor.editElement", command.ToString(), argument);
        }
    }
}
