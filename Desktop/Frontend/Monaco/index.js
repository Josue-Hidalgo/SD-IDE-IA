var editor;
var txtL = true;

require(["./vs/editor/editor.main"],()=>{
    editor = monaco.editor.create(document.getElementById("container"),{
        value:'',
        language:"txt",
        theme:"vs-dark",
        automaticLayout: true
    });
});

function changeLanguage(){
    if(txtL){
        editor = monaco.editor.create(document.getElementById("container"),{
        value:'',
        language:"python",
        theme:"vs-dark",
        automaticLayout: true
    });
    }else{
        editor = monaco.editor.create(document.getElementById("container"),{
        value:'',
        language:"txt",
        theme:"vs-dark",
        automaticLayout: true
    });
    txtL = !txtL;
    }
}

function getValue(){
    return editor.getValue();
}

function setValue(value){
    editor.setValue(value);
}