var editor;

require(["./vs/editor/editor.main"], () => {
    editor = monaco.editor.create(document.getElementById("container"), {
        value: "",
        language: "plaintext",
        theme: "vs-dark",
        automaticLayout: true,
    });
});

function changeLanguage(lang) {
    if (!editor) return;
    var model = editor.getModel();
    monaco.editor.setModelLanguage(model, lang || "python");
}

function getValue() {
    if (!editor) return "";
    return editor.getValue();
}

function setValue(value) {
    if (!editor) return;
    editor.setValue(value);
}
