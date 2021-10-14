const editMde = new SimpleMDE({
    element: document.getElementById('editor'),
    initialValue: '',
    hideIcons: ['guide', 'fullscreen', 'side-by-side']
    // guide implmentation is open to you
    // fullscreen and sxs muss up layout
});
// editMde.value('# Sample Markdown\n- one\n- two\n- three');

// document.getElementById('reading').innerHTML =
//     marked('# Sample Markdown\n[Don\'t use Simplemde for read-only content](https://github.com/sparksuite/simplemde-markdown-editor/issues/274#issuecomment-196614437).  Use [marked](https://github.com/markedjs/marked)\n- one\n- two\n- three');