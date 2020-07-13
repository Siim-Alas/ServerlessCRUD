
// This implements Quill. Reference: https://quilljs.com/docs/quickstart/

window.QuillClient = {
    initQuill: function (toolbarElement, editorElement) {
        new Quill(editorElement, {
            modules: {
                toolbar: toolbarElement
            },
            theme: 'snow'
        });
    },
    getHTML: function (editorElement) {
        return editorElement.__quill.root.innerHTML;
    }
};