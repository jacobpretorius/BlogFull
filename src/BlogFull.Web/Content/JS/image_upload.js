//handle drag drop + double click events
//Based on this fine work by Joe Zim
//https://codepen.io/joezimjs/pen/yPWQbd

var nameColletion = [];

var dropArea = document.getElementById("drop-area");
["dragenter", "dragover", "dragleave", "drop"].forEach(eventName => {
    dropArea.addEventListener(eventName, preventDefaults, false);
    document.body.addEventListener(eventName, preventDefaults, false);
});

["dragenter", "dragover"].forEach(eventName => {
    dropArea.addEventListener(eventName, highlight, false);
});

["dragleave", "drop"].forEach(eventName => {
    dropArea.addEventListener(eventName, unhighlight, false);
});

dropArea.addEventListener("drop", handleDrop, false);

function preventDefaults(e) {
    console.log("hit");
    e.preventDefault();
    e.stopPropagation();
}

function highlight(e) {
    dropArea.classList.add("highlight");
}

function unhighlight(e) {
    dropArea.classList.remove("active");
}

function handleDrop(e) {
    var dt = e.dataTransfer;
    var files = dt.files;

    handleFiles(files);
}

var uploadProgress = [];
var progressBar = document.getElementById("progress-bar");

function initializeProgress(numFiles) {
    progressBar.value = 0;
    uploadProgress = [];

    for (let i = numFiles; i > 0; i--) {
        uploadProgress.push(0);
    }
}

function updateProgress(fileNumber, percent) {
    uploadProgress[fileNumber] = percent;
    var total = uploadProgress.reduce((tot, curr) => tot + curr, 0) / uploadProgress.length;
    progressBar.value = total;
}

function handleFiles(files) {
    files = [...files];
    initializeProgress(files.length);
    files.forEach(uploadFile);
    files.forEach(previewFile);
}

function previewFile(file) {
    var reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onloadend = function() {
        var img = document.createElement("img");
        img.src = reader.result;
        img.id = file.name;
        img.addEventListener("click", handleImageClick);
        document.getElementById("gallery").appendChild(img);
    };
}

function uploadFile(file, i) {
    var xhr = new XMLHttpRequest();
    var formData = new FormData();
    xhr.open("POST", "/submit/upload", true);
    xhr.setRequestHeader("X-Requested-With", "XMLHttpRequest");
    
    xhr.upload.addEventListener("progress",
        function(e) {
            updateProgress(i, (e.loaded * 100.0 / e.total) || 100);
        });

    xhr.addEventListener("readystatechange",
        function(e) {
            if (xhr.readyState === 4 && xhr.status === 200) {
                updateProgress(i, 100);
                storeNewName(file.name, xhr.response.replace(/['"]+/g, ""));
            } else if (xhr.readyState === 4 && xhr.status !== 200) {
                // Error. Inform the user
                storeNewName(file.name, "Login error");
            }
        });
    formData.append("file", file);
    xhr.send(formData);
}

function storeNewName(old, newName) {
    nameColletion.push(old + "|" + newName);
}

//double click copy to clipboard
function copyStringToClipboard(str) {
    var el = document.createElement("textarea");
    el.value = str;
    el.setAttribute("readonly", "");
    el.style = { position: "absolute", left: "-9999px" };
    document.body.appendChild(el);
    el.select();
    document.execCommand("copy");
    document.body.removeChild(el);
}

//copy formatted link to clipboard
function handleImageClick() {
    var oldName = this.id;
    for (var i = 0; i < nameColletion.length; i++) {
        var stored = nameColletion[i].split("|");
        if (stored[0] === oldName) 
        {
            if (stored[1] === "Login error") {
                notifyError();
                return copyStringToClipboard("LOGIN ERROR");
            } else {
                notify();
                return copyStringToClipboard("![alt-text-here](/post-images/" + stored[1] + ")");
            }
        }
    }
    return null;
}

//fade and show notification bar
function notify() {
    var text = document.getElementById("notificationText");
    text.innerText = "Link copied to clipboard";

    var bar = document.getElementById("notification");
    bar.classList.remove("notification-bar__error");
    bar.classList.remove("toggle-show");
    bar.offsetWidth;
    bar.classList.add("toggle-show");
}

function notifyError() {
    var text = document.getElementById("notificationText");
    text.innerText ="Error. Please log in again.";

    var bar = document.getElementById("notification");
    bar.classList.add("notification-bar__error");
    bar.classList.remove("toggle-show");
    bar.offsetWidth;
    bar.classList.add("toggle-show");
}

//prevent accidental navigation away on post/edit pages
window.onbeforeunload = function() {
    return "Are you sure you want to go?";
};

function SubmitPage() {
    window.onbeforeunload = null;
}