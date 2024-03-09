const notificationsConfig = {
    notificationsUrl: 'ws://localhost:5159/TaskManager/GetNotifications',
    requestInterval: 15000
};

const notificationContainer = document.getElementById('notification-container');

const notificationSocket = new WebSocket(notificationsConfig.notificationsUrl);
console.info('Notifications webSocket is opening...');
notificationSocket.onmessage = function (event) {
    const th = document.createElement('div');
    th.textContent = event.data;
    notificationContainer.appendChild(th);
}

console.info('Notifications watching is starting...');
const refreshNotificationIntervalId = setInterval(ping, notificationsConfig.requestInterval);
function ping() {
    if (!notificationSocket
        || notificationSocket.readyState === WebSocket.CLOSING
        || notificationSocket.readyState === WebSocket.CLOSED) {
        clearInterval(refreshNotificationIntervalId);
        console.info('...Notifications watching stopped.');
    }
    else if (notificationSocket.readyState == WebSocket.OPEN) {
		notificationSocket.send('ping');
	}
}

window.addEventListener('beforeunload', function (e) {
    if (notificationSocket && notificationSocket.readyState !== WebSocket.CLOSED) {
        notificationSocket.close();
        console.info('...Notifications webSocket is closing.');
    }
});