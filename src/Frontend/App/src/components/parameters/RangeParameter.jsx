import { useState } from "react";

function RangeParameter({ ...props }) {
  const [min, setMin] = useState(props.param.Value.Min);
  const [max, setMax] = useState(props.param.Value.Max);
  const [increment, setIncrement] = useState(props.param.Value.Increment);

  const changeMinimum = (value) => {
    if (!props.param.Value.Min)
      props.param.Value = { ...props.param.Value, ...{ Min: 0 } };
    props.param.Value.Min = parseInt(value);
    setMin(value);
  };
  const changeMaximum = (value) => {
    if (!props.param.Value.Max)
      props.param.Value = { ...props.param.Value, ...{ Max: 0 } };
    props.param.Value.Max = parseInt(value);
    setMax(value);
  };
  const changeIncrement = (value) => {
    if (!props.param.Value.Increment)
      props.param.Value = { ...props.param.Value, ...{ Increment: 0 } };
    props.param.Value.Increment = parseInt(value);
    setIncrement(value);
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
          onChange={(e) => changeMinimum(e.target.value)}
        />
      </div>

      <label>Maximum Value:</label>
      <div className="form-group">
        <input
          type="number"
          className="form-control"
          placeholder="Maximum Value"
          value={max}
          onChange={(e) => changeMaximum(e.target.value)}
        />
      </div>

      <label>Increment:</label>
      <div className="form-group">
        <input
          type="number"
          className="form-control"
          placeholder="Increment"
          value={increment}
          onChange={(e) => changeIncrement(e.target.value)}
        />
      </div>
    </>
  );
}

export default RangeParameter;
