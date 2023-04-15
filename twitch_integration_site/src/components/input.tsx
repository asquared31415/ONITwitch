import * as React from "react";
import * as Context from "../data/context";
import { ConfigData } from "../data/types";
const deepEqual = require('deep-equal');

export function ConfigInput(): JSX.Element {
    const { setConfig, setInputConfig } = React.useContext(Context.ConfigContext);

    function onChange(event: React.ChangeEvent): void {
        let input = event.target as HTMLInputElement;
        // parsing this twice is sad, but it's a workaround for the fact that deep clones are hard
        var new_input_config = ConfigData.from_encoded(input.value);
        setInputConfig(new_input_config);
        let new_config = ConfigData.from_encoded(input.value);
        setConfig(new_config);
    }

    return (
        <div className="row my-3 justify-content-center">
            <div className="col-6">
                <div className="input-group">
                    <span className="input-group-text col-2 text-bg-primary border-primary text-break">Input data</span>
                    <input id="main-input" className="form-control col-10 border-primary" type="text" onChange={onChange} />
                </div>
            </div>
        </div>
    );
}

export function ExportSettings(): JSX.Element {
    const { config, input_config } = React.useContext(Context.ConfigContext);

    function onClick(): void {
        const encoded = config.to_encoded();
        console.log(encoded);
        navigator.clipboard.writeText(encoded).then(
            () => { alert("Copied to clipboard") },
            () => { alert("Unable to copy to clipboard, check log") }
        );
    }

    const is_unchanged = deepEqual(config, input_config);

    let text_classes = "text-center";
    if (!is_unchanged) {
        text_classes += " visibility-none";
    }

    let button_classes = "btn col-2 col-xxl-1";
    if (is_unchanged) {
        button_classes += " btn-outline-danger disabled";
    } else {
        button_classes += " btn-success";
    }

    return (
        <>
            <div className="row justify-content-center">
                <button type="button" className={button_classes} onClick={onClick}>Export Settings</button>
            </div>
            <div className="row mb-2">
                <p className={text_classes}>Data has not been changed</p>
            </div>
        </>
    );
}