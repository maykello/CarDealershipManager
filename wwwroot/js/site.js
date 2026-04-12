const SESSION_TIMEOUT_MS = 10 * 60 * 1000;      
const WARNING_BEFORE_MS = 9 * 60 * 1000;       

let warningTimer;
let logoutTimer;
let countdownInterval;
let modalInstance;

function startSessionTimers() {
    clearTimeout(warningTimer);
    clearTimeout(logoutTimer);
    clearInterval(countdownInterval);

    warningTimer = setTimeout(showWarningModal, WARNING_BEFORE_MS);
    logoutTimer = setTimeout(forceLogout, SESSION_TIMEOUT_MS);
}

function showWarningModal() {
    const modalEl = document.getElementById('sessionTimeoutModal');
    if (!modalInstance) {
        modalInstance = new bootstrap.Modal(modalEl);
    }
    modalInstance.show();

    let secondsLeft = Math.floor((SESSION_TIMEOUT_MS - WARNING_BEFORE_MS) / 1000);
    document.getElementById('sessionCountdown').innerText = secondsLeft;
    
    countdownInterval = setInterval(() => {
        secondsLeft--;
        if (secondsLeft >= 0) {
            document.getElementById('sessionCountdown').innerText = secondsLeft;
        }
    }, 1000);
}

function forceLogout() {
    const logoutForm = document.getElementById('logoutFormModal');
    if (logoutForm) {
        logoutForm.submit();
    }
}

document.addEventListener("DOMContentLoaded", () => {
    const extendBtn = document.getElementById('extendSessionBtn');
    
    if (extendBtn) {
        extendBtn.addEventListener('click', function () {
            const keepAliveUrl = this.getAttribute('data-url');
            
            fetch(keepAliveUrl, {
                method: 'POST',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            }).then(response => {
                if (response.ok) {
                    modalInstance.hide();
                    startSessionTimers();
                } else {
                    forceLogout();
                }
            }).catch(() => forceLogout());
        });
    }
    startSessionTimers();
});