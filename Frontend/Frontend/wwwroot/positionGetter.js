window.getElementPosition = (clientX, clientY) => {
    return {
        top: clientY + window.scrollY,
        left: clientX + window.scrollX
    };
};

window.getPageWidth = () => {
    return window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
};

let contextMenu = document.getElementById("contextMenuDiv");
function changeDivClass() {
    contextMenu.classList.add("contextMenuDiv");
}
