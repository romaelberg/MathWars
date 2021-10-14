let dropArea = document.getElementById("drop-area");
['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
    dropArea.addEventListener(eventName, preventDefaults, false)
    document.body.addEventListener(eventName, preventDefaults, false)
})

;['dragenter', 'dragover'].forEach(eventName => {
    dropArea.addEventListener(eventName, highlight, false)
})

;['dragleave', 'drop'].forEach(eventName => {
    dropArea.addEventListener(eventName, unhighlight, false)
})

// Handle dropped files
dropArea.addEventListener('drop', handleDrop, false)

function preventDefaults (e) {
    e.preventDefault()
    e.stopPropagation()
}

function highlight(e) {
    dropArea.classList.add('highlight')
}

function unhighlight(e) {
    dropArea.classList.remove('active')
}

function handleDrop(e) {
    var dt = e.dataTransfer;
    var files = dt.files;
    //const concated = [...$("input[type='file']").prop('files'), ...files];
    //console.log("CONCATED: ", concated);
    var inputFiles = [...$("input[type='file']").prop('files')];
    let list = new DataTransfer();
    for (var x = 0;x<inputFiles.length;x++) {
        list.items.add(inputFiles[x]);
    }
    for (var i = 0; i< files.length;i++) {
        list.items.add(files[i]);
    }
    $("input[type='file']")
        .prop("files", list.files)
        .closest("form")
    handleFiles(files)
}

function handleFiles(files) {
    files = [...files]
    files.forEach(previewFile)
}

function previewFile(file) {
    // alert("files preview!");
    let reader = new FileReader()
    reader.readAsDataURL(file)
    reader.onloadend = function() {
        let img = document.createElement('img')
        img.src = reader.result
        document.getElementById('gallery').appendChild(img)
    }
}