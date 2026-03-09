
const itemConnection = new signalR.HubConnectionBuilder()
    .withUrl('/itemHub')
    .withAutomaticReconnect()
    .build();

export { itemConnection };