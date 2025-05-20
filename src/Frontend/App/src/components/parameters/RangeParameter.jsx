import { useState } from "react";

function RangeParameter({ ...props }) {
  const [min, setMin] = useState(props.param.Value.Min);
  const [max, setMax] = useState(props.param.Value.Max);
  const setMinimum = (value) => {
    props.param.Value.Min = parseInt(value);
    setMin(value);
  };
  const setMaximum = (value) => {
    props.param.Value.Max = parseInt(value);
    setMax(value);
  };
  return (
    <>
      {/* <label>Default Value:</label>
      <div className="form-group">
        <input
          type="number"
          className="form-control"
          placeholder="Default Value"
          defaultValue={param.Value.Default}
        />
      </div> */}

      <label>Minimum Value:</label>
      <div className="form-group">
        <input
          type="number"
          className="form-control"
          placeholder="Minimum Value"
          value={min}
          onChange={(e) => setMinimum(e.target.value)}
          defaultValue={props.param.Value.Min}
        />
      </div>

      <label>Maximum Value:</label>
      <div className="form-group">
        <input
          type="number"
          className="form-control"
          placeholder="Maximum Value"
          value={max}
          onChange={(e) => setMaximum(e.target.value)}
          defaultValue={props.param.Value.Max}
        />
      </div>

      <label>Increment:</label>
      <div className="form-group">
        <input
          type="number"
          className="form-control"
          placeholder="Increment"
          defaultValue={props.param.Value.Increment}
        />
      </div>
    </>
  );
}

export default RangeParameter;
