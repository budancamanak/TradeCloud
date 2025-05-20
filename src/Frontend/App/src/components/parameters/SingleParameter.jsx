import { useState } from "react";

function SingleParameter({ param }) {
  const [value, setValue] = useState(param.Value);
  return (
    <>
      <label>Parameter Value:</label>
      <div className="form-group">
        <input
          type="number"
          className="form-control"
          placeholder="Value"
          value={value}
          defaultValue={param.Value}
        />
      </div>
    </>
  );
}

export default SingleParameter;
