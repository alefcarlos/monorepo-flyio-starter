let source = null;

export function onLoad() {
    if (source) {
        source.close();
    }

    source = new EventSource("/heart-rate");

    source.addEventListener("heartRate", (event) => {
        try {
            const data = JSON.parse(event.data);
            document.getElementById("bpm").textContent = data.heartRate;
        } catch { }
    });

    // opcional: tratar erro
    source.onerror = () => {
        console.warn("SSE desconectado (o browser vai tentar reconectar)");
    };
}

export function onDispose() {
    if (source) {
        source.close();
        source = null;
    }
}