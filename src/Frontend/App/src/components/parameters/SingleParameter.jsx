import { useState } from "react";

function SingleParameter({ ...props }) {
  const [value, setValue] = useState(props.param.Value);
  const setCurrentValue = (value) => {
    props.param.Value = parseInt(value);
    setValue(value);
  };
  return (
    <>
      <label>Parameter Value:</label>
      <div className="form-group">
        <input
          type="number"
          className="form-control"
          placeholder="Value"
          value={value}
          onChange={(e) => setCurrentValue(e.target.value)}
          defaultValue={props.param.Value}
        />
      </div>
    </>
  );
}

export default SingleParameter;
