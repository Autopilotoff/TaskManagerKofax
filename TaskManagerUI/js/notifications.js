const notificationsConfig = {
    notificationsUrl: 'localhost:5159/TaskManager/GetNotifications',
    pingInterval: 15000,
	token: createGuid()
};

const notificationContainer = document.getElementById('notification-container');

const notificationsUrl = `ws://${notificationsConfig.notificationsUrl}?token=${notificationsConfig.token}`;

const notificationSocket = new WebSocket(notificationsUrl);
console.info('Notifications webSocket is opening...');

notificationSocket.onmessage = function (event) {
    const div = document.createElement('div');
    div.textContent = event.data;
    notificationContainer.appendChild(div);
}

console.info('Notifications ping is starting...');
let refreshPingIntervalId = null;
const pingNotificationsUrl = `http://${notificationsConfig.notificationsUrl}?token=${notificationsConfig.token}`;

notificationSocket.onopen = () => {
	refreshPingIntervalId = setInterval(() => {
		fetch(pingNotificationsUrl, { mode: 'no-cors' });
	}, notificationsConfig.pingInterval);
};

notificationSocket.onclose = () => {
	console.info('... Notifications webSocket closed.');
	clearInterval(refreshPingIntervalId);
};

window.addEventListener('beforeunload', function (e) {
    if (notificationSocket && notificationSocket.readyState !== WebSocket.CLOSED) {
        notificationSocket.close();
        console.info('...Notifications webSocket is closing.');
    }
});