const WebSocket = require('ws');
const {CadenceSensor} = require('incyclist-ant-plus');
const {AntDevice} = require('incyclist-ant-plus/lib/bindings');

// WebSocketサーバーの設定
const wss = new WebSocket.Server({ port: 8080 });

// ANT+デバイスの設定
const ant = new AntDevice({ startupTimeout: 2000 });

async function main(deviceID = -1) {
    const opened = await ant.open();
    if (!opened) {
        console.log('could not open Ant Stick');
        return;
    }

    const channel = ant.getChannel();
    if (!channel) {
        console.log('could not open channel');
        return;
    }

    channel.on('data', onData);

    if (deviceID === -1) {
        console.log('Scanning for sensor(s)');
        const cadenceSensor = new CadenceSensor();
        channel.startScanner();
        channel.attach(cadenceSensor);
    } else {
        console.log(`Connecting with id=${deviceID}`);
        const cadenceSensor = new CadenceSensor(deviceID);
        channel.startSensor(cadenceSensor);
    }
}

// データ受信時に呼ばれる関数
function onData(profile, deviceID, data) {
    const cadence = data.CalculatedCadence;
    console.log(`id: ANT+${profile} ${deviceID}, cadence: ${cadence}`);

    // WebSocketクライアントにデータを送信
    wss.clients.forEach(client => {
        if (client.readyState === WebSocket.OPEN) {
            client.send(JSON.stringify({ cadence }));
        }
    });
}

// WebSocket接続の処理
wss.on('connection', (ws) => {
    console.log('WebSocket connection opened');

    ws.on('message', (message) => {
        console.log(`Received message: ${message}`);
    });

    ws.on('close', () => {
        console.log('WebSocket connection closed');
    });
});

// アプリケーションの終了処理
async function onAppExit() {
    await ant.close();
    return 0;
}

process.on('SIGINT', async () => await onAppExit());  // CTRL+C
process.on('SIGQUIT', async () => await onAppExit()); // Keyboard quit
process.on('SIGTERM', async () => await onAppExit()); // `kill` command

const args = process.argv.slice(2);
const deviceID = args.length > 0 ? args[0] : undefined;

main(deviceID);
