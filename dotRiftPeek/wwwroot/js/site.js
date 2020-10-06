// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const { ipcRenderer } = require("electron");

ipcRenderer.on("redirect", (sender, path) => {
    window.location.href = path;
});

var coll = document.getElementsByClassName("collapsible");
var i;

for (i = 0; i < coll.length; i++) {
    coll[i].addEventListener("click", function () {
        this.classList.toggle("active");
        var content = this.nextElementSibling;
        if (content.style.maxHeight) {
            content.style.maxHeight = null;
            content.style.paddingTop = "0";
            content.style.paddingBottom = "0";
        } else {
            content.style.maxHeight = content.scrollHeight + "px";
            content.style.paddingTop = "10px";
            content.style.paddingBottom = "10px;"
        }
    });
}

