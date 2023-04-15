import * as React from "react";
import * as ReactDOMClient from "react-dom/client";
import * as Context from "./data/context";
import { ConfigData } from "./data/types";
import { EventInfoContainer } from "./components/event_info";
import { ConfigInput, ExportSettings } from "./components/input";

const deepEqual = require('deep-equal');

import "./index.css";

function Root(): JSX.Element {
    let [config, setConfig] = React.useState(Context.DefaultData);
    let [input_config, setInputConfig] = React.useState(Context.DefaultInputData);

    if (deepEqual(config, input_config) && document.location.hash) {
        // data without the hash
        const data = document.location.hash.substring(1);

        // clear hash so that it doesn't happen again
        document.location.hash = "";

        // parsing this twice is sad, but it's a workaround for the fact that deep clones are hard
        input_config = ConfigData.from_encoded(data);
        config = ConfigData.from_encoded(data);
    }

    const state = { config, setConfig, input_config, setInputConfig };

    return (
        <div className="container-fluid">
            <Context.ConfigContext.Provider value={state}>
                <ConfigInput />
                <ExportSettings />
                <EventInfoContainer />
            </Context.ConfigContext.Provider>
        </div>
    )
}

let root = ReactDOMClient.createRoot(document.getElementById("root")!);
root.render(<Root />);
