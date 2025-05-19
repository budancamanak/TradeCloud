import { useState, useEffect } from "react";
import Select from "react-select";
import { getPluginParameterRange, getPluginParameterValueType } from "../../utils/helpers";

function PluginParameter({ param }) {
  return (
    <>
      <div className="card card-default" style={{ minWidth: "300px" }}>
        <div className="card-header">
          <h3 className="card-title">{param.Name}</h3>

          <div className="card-tools">
            <button
              type="button"
              className="btn btn-tool"
              data-card-widget="collapse"
            >
              <i className="fas fa-minus"></i>
            </button>
            <button
              type="button"
              className="btn btn-tool"
              data-card-widget="remove"
            >
              <i className="fas fa-times"></i>
            </button>
          </div>
        </div>

        <div className="card-body">
          <div className="row">
            <div className="col-md-6">
              <div className="form-group">
                <label>Range Type</label>
                <Select
                  getOptionLabel={(option) => `${option.name}`}
                  getOptionValue={(option) => `${option.type}`}
                  defaultInputValue={
                    getPluginParameterRange()[param.Range].name
                  }
                  defaultValue={param.Range}
                  // value={param.Range}
                  // inputValue={getPluginParameterRange()[param.Range].name}
                  options={getPluginParameterRange()}
                />
              </div>

              <div className="form-group">
                <label>Symbol to run</label>

              </div>
            </div>

            <div className="col-md-6">
              <div className="form-group">
                <label>Value Type:</label>

                <Select
                  getOptionLabel={(option) => `${option.name}`}
                  getOptionValue={(option) => `${option.type}`}
                  defaultInputValue={
                    getPluginParameterValueType()[param.Type].name
                  }
                  defaultValue={param.Type}
                  // value={param.Range}
                  // inputValue={getPluginParameterRange()[param.Range].name}
                  options={getPluginParameterValueType()}
                />
              </div>
              <div className="form-group">
                <label>Timeframe</label>
                <select
                  className="form-control select2bs4"
                  style={{ width: "100%" }}
                >
                  <option value={"5m"}>5m</option>
                  <option value={"15m"}>15m</option>
                  <option value={"30m"}>30m</option>
                  <option value={"60m"}>1H</option>
                  <option value={"120m"}>2H</option>
                  <option value={"240m"}>4H</option>
                  <option value={"480m"}>8H</option>
                  <option value={"720m"}>12H</option>
                  <option value={"1440m"}>1D</option>
                </select>
              </div>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
export default PluginParameter;
