import * as Pako from 'pako';
import { Base64 } from 'js-base64';
import { TextDecoder, TextEncoder } from 'text-encoding';

export type EventConfig = {
    FriendlyName: string,
    Weight: number,
    GroupName: string,
    Data?: any
}

type JsonData = {
    [namespace: string]: { [id: string]: EventConfig }
};

export class ConfigData {
    readonly data: Map<string, EventConfig>;

    constructor(data: Map<string, EventConfig>) {
        this.data = data;
    }

    static from_encoded(encoded: string): ConfigData {
        try {
            const compressed = Base64.toUint8Array(encoded);
            const decompressed = Pako.inflate(compressed, { raw: true });
            const decompressedStr = new TextDecoder("utf-8").decode(decompressed);

            const json_data: JsonData = JSON.parse(decompressedStr);

            const data = new Map<string, EventConfig>();

            for (const namespace_key in json_data) {
                const events = json_data[namespace_key];
                for (const event_id in events) {
                    const combined_id = `${namespace_key}.${event_id}`;
                    data.set(combined_id, events[event_id]);
                }
            }

            return new ConfigData(data);
        } catch (e) {
            return new ConfigData(new Map());
        }

    }

    to_encoded(): string {
        const namespaced_data: JsonData = {};
        for (const [key, event_info] of this.data.entries()) {
            const dot_idx = key.lastIndexOf(".");
            const namespace = key.substring(0, dot_idx);
            const id = key.substring(dot_idx + 1);

            const namespaced = namespaced_data[namespace];
            if (namespaced) {
                namespaced[id] = event_info;
            } else {
                const n: { [id: string]: EventConfig } = {};
                n[id] = event_info;
                namespaced_data[namespace] = n;
            }
        }

        const data_str = JSON.stringify(namespaced_data);
        const bytes = new TextEncoder().encode(data_str);
        const compressed = Pako.deflate(bytes, { raw: true });
        const encoded = Base64.fromUint8Array(compressed, true);

        return encoded;
    }
}