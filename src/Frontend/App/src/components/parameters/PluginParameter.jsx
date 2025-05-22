import { useState, useEffect } from "react";
import Select from "react-select";
import {
  getPluginParameterRange,
  getPluginParameterValueType,
} from "../../utils/helpers";
import SingleParameter from "./SingleParameter";
import RangeParameter from "./RangeParameter";
import ListParameter from "./ListParameter";

function ParameterInstance({ ...props }) {
  // if (!props.selected_type) return null;
  console.log("param instance");
  switch (props.selected_type) {
    case 0:
      return <SingleParameter param={props.param} />;
    case 1:
      return <RangeParameter param={props.param} />;
    case 2:
      return <ListParameter param={props.param} />;
    default:
      return <div>Unknown</div>;
  }
}

function PluginParameter({ ...props }) {
  console.log("plugin parameter>", props.param);
  const [parameterRange, setParameterRange] = useState(props.param.Range);

  return (
    <>
      <div className="card card-default" style={{ minWidth: "300px" }}>
        <div className="card-header">
          <h3 className="card-title">{props.param.Name}</h3>

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
                    getPluginParameterRange()[props.param.Range].name
                  }
                  defaultValue={props.param.Range}
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
                    getPluginParameterValueType()[props.param.Type].name
                  }
                  defaultValue={props.param.Type}
                  // value={param.Range}
                  // inputValue={getPluginParameterRange()[param.Range].name}
                  options={getPluginParameterValueType()}
                />
              </div>
            </div>

            <div className="col-md-6">
              <div className="form-group">
                <ParameterInstance
                  param={props.param}
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
