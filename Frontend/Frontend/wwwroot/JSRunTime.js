window.getPageWidth = () => {
    return window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
};

window.intersectionObserver = {
    observers: new Map(),

    observeElement: function (element, dotNetHelper) {
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                dotNetHelper.invokeMethodAsync('OnIntersectionChange', entry.isIntersecting);
            });
        });

        observer.observe(element);

        this.observers.set(element, observer);
    },

    unobserveElement: function (element) {
        const observer = this.observers.get(element);

        if (observer) {
            observer.unobserve(element);
            this.observers.delete(element);
        }
    }
};

window.scrollToBottom = function (elementId) {
    var element = document.getElementById(elementId);
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
};

window.scrollToMessageByHash= function (hash) {
    var container = document.getElementById('messageContainer');
    if (container) {
        var element = document.getElementById(hash);
        if (element) {
            var position = element.offsetTop;
            container.scrollTop = position;
        }
    }
};

window.getFirstMessageElementId = function () {
    var container = document.getElementById('messageContainer');
    if (container) {
        console.log("Found container");
        var firstMessageElement = container.querySelector(".message");
        if (firstMessageElement) {
            return firstMessageElement.id;
        }
    }

    return "";
};
