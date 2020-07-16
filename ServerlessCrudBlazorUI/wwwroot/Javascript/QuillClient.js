
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
    insertImage: function (editorElement, imageUrl) {
        var currentIndex;
        var Delta = Quill.import('delta');

        if (editorElement.__quill.getSelection() !== null) {
            currentIndex = editorElement.__quill.getSelection().index;
        } else {
            currentIndex = 0;
        }

        return editorElement.__quill.updateContents(
            new Delta()
                .retain(currentIndex)
                .insert(
                    {image: imageUrl},
                    {alt: imageUrl})
        );
    },
    getHTML: function (editorElement) {
        return editorElement.__quill.root.innerHTML;
    }
};