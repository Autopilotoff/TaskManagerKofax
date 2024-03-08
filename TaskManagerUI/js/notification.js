const notificationsConfig = {
	notificationsUrl: 'ws://localhost:5159/TaskManager/GetNotifications',
	requestInterval: 15000
};

const notificationContainer = document.getElementById('notification-container');

const notificationSocket = new WebSocket(notificationsConfig.notificationsUrl);

notificationSocket.onmessage = function (event) {
    const th = document.createElement('div');
    th.textContent = event.data;
    notificationContainer.appendChild(th);
}

const refreshNotificationIntervalId = setInterval(ping, notificationsConfig.requestInterval);
function ping() {
	if (notificationSocket && notificationSocket.readyState == WebSocket.OPEN) {
        notificationSocket.send('ping');
	}
	else {
		clearInterval(refreshNotificationIntervalId);
	}
}

window.addEventListener('beforeunload', function (e) {
    if (notificationSocket && notificationSocket.readyState !== WebSocket.CLOSED) {
        notificationSocket.close();
    }
});