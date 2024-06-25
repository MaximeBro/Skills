window.keyboardListener = (dotNetHelper, key, callback) => {
    let isBusy;
    
    document.addEventListener('keydown', function(event) {
        if (!isBusy && event.ctrlKey && event.key === key) {
            event.preventDefault();
            isBusy = true;
            return dotNetHelper.invokeMethodAsync(callback);
        }
    });
    
    document.addEventListener('keyup', function(event) {
        if(!event.ctrlKey || event.key === key) {
            isBusy = false;
        }
    });
}