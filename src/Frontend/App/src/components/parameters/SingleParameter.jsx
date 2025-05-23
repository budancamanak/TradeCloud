import { useState, useEffect } from "react";

function SingleParameter({ ...props }) {
  const [value, setValue] = useState(props.param.Value);
  useEffect(() => {
    props.param.Range = 0;
  }, []);
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
        />
      </div>
    </>
  );
}

export default SingleParameter;
