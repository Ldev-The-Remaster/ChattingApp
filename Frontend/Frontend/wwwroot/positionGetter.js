window.getElementPosition = (clientX, clientY) => {
    return {
        top: clientY + window.scrollY,
        left: clientX + window.scrollX
    };
};

let contextMenu = document.getElementById("contextMenuDiv");
function changeDivClass() {
    contextMenu.classList.add("contextMenuDiv");
}