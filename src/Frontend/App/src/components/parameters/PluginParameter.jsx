import { useState, useEffect } from "react";
import Select from "react-select";
import {
  getPluginParameterRange,
  getPluginParameterValueType,
} from "../../utils/helpers";
import SingleParameter from "./SingleParameter";
import RangeParameter from "./RangeParameter";
import ListParameter from "./ListParameter";

function ParameterInstance({ param, selected_type }) {
  if(!selected_type) return null;
  console.log("param instance");
  switch (selected_type) {
    case 0:
      return <SingleParameter param={param} />;
    case 1:
      return <RangeParameter param={param} />;
    case 2:
      return <ListParameter />;
    default:
      return <div>Unknown</div>;
  }
}

function PluginParameter({ param }) {
  console.log("plugin parameter>",param);
  const [parameterRange, setParameterRange] = useState(param.Range);
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
                  onChange={(e) => {
                    console.log(e);
                    setParameterRange(e.type);
                  }}
                  options={getPluginParameterRange()}
                />
              </div>
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
            </div>

            <div className="col-md-6">
              <div className="form-group">
                <ParameterInstance
                  param={param}
                  selected_type={parameterRange}
                />
              </div>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
export default PluginParameter;
