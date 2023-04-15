import { createContext } from "react";
import { ConfigData } from "./types";

export const DefaultData: ConfigData = new ConfigData(new Map());
export const DefaultInputData: ConfigData = new ConfigData(new Map());

export const ConfigContext = createContext({
    config: DefaultData,
    setConfig: (_: ConfigData) => { },
    input_config: DefaultInputData,
    setInputConfig: (_: ConfigData) => { },
});