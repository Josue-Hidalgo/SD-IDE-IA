var editor;

require(["./vs/editor/editor.main"],()=>{
    editor = monaco.editor.create(document.getElementById("container"),{
        value:'',
        language:"python",
        theme:"vs-dark",
        automaticLayout: true
    });
});

function getValue(){
    return editor.getValue();
}

function setValue(value){
    return editor.setValue(value);
}