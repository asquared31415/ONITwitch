import * as React from "react";
import * as Context from "../data/context";
import { ConfigData, EventConfig } from "../data/types";

export function EventInfoContainer(): JSX.Element {
    const { config } = React.useContext(Context.ConfigContext);

    let elems = new Array<JSX.Element>();
    for (let [id, data] of config.data) {
        elems.push(
            <div key={id} className="mb-4 px-1 w-25">
                <EventInfo id={id} config={data}></EventInfo>
            </div>
        );
    }

    return (<>
        <div className="row mb-2">
            <p className="text-center">{`${config.data.size} events registered`}</p>
        </div>
        <div className="d-flex flex-wrap justify-content-around">
            {elems}
        </div>
    </>);
}

type EventInfoProps = {
    id: string
    config: EventConfig
}

export function EventInfo({ id, config: { FriendlyName, Weight, GroupName, Data } }: EventInfoProps): JSX.Element {
    const { config, setConfig } = React.useContext(Context.ConfigContext);

    function onNameEdit(event: React.ChangeEvent): void {
        const input = event.target as HTMLInputElement;
        const original = config.data.get(id);
        if (original) {
            original.FriendlyName = input.value;
            // this must create a new object, to cause an update
            setConfig(new ConfigData(config.data));
        }
    }

    function onWeightEdit(event: React.ChangeEvent): void {
        const input = event.target as HTMLInputElement;
        const original = config.data.get(id);
        if (original) {
            let parsedWeight = Number.parseInt(input.value);
            if (!isFinite(parsedWeight)) {
                parsedWeight = 0;
            }
            original.Weight = parsedWeight;
            // this must create a new object, to cause an update
            setConfig(new ConfigData(config.data));
        }
    }

    function onGroupEdit(event: React.ChangeEvent): void {
        const input = event.target as HTMLInputElement;
        const original = config.data.get(id);
        if (original) {
            original.GroupName = input.value.trim();
            // this must create a new object, to cause an update
            setConfig(new ConfigData(config.data));
        }
    }

    var groupInputElement;
    if (GroupName != "" && !GroupName.startsWith("__<nogroup>__")) {
        groupInputElement = (
            <input className="form-control col-8" type="text" value={GroupName} onChange={onGroupEdit} />
        );
    } else {
        groupInputElement = (
            <input className="form-control col-8" type="text" placeholder="No Group" onChange={onGroupEdit} />
        );
    }

    return (
        <div className="card">
            <div className="card-header">
                <h6 className="text-break">{id}</h6>
            </div>
            <div className="card-body">
                <div className="input-group mb-3">
                    <span className="input-group-text col-4">Friendly Name</span>
                    <input className="form-control col-8" type="text" placeholder="No Name" value={FriendlyName} onChange={onNameEdit}></input>
                </div>
                <div className="input-group mb-3">
                    <span className="input-group-text col-4">Weight</span>
                    <input className="form-control col-8" type="number" value={Weight} onChange={onWeightEdit} />
                </div>
                <div className="input-group mb-3">
                    <span className="input-group-text col-4">Group</span>
                    {groupInputElement}
                </div>
                <label>Data</label>
                <div className="card card-body text-bg-light p-2 text-wrap">
                    {JSON.stringify(Data)}
                </div>
            </div>
        </div>
    )
}