
// Reference: https://developer.mozilla.org/en-US/docs/Web/Guide/HTML/Editable_content

window.DocumentEditor = {
    editElement: function (command, value) {
        document.execCommand(command, false, value);
    }
};