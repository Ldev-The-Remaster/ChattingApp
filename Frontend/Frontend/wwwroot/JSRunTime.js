window.getPageWidth = () => {
    return window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
};

window.checkElementIntersection = function (element) {
    return new Promise((resolve, reject) => {
        if (!element) {
            reject("Element not found");
            return;
        }

        const observer = new IntersectionObserver((entries, observer) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    resolve(true);
                    observer.disconnect();
                } else {
                    resolve(false);
                    observer.disconnect(); 
                }
            });
        });

        observer.observe(element);
    });
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
