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
