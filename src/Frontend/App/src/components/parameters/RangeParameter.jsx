function RangeParameter({ param }) {
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
          defaultValue={param.Value.Min}
        />
      </div>

      <label>Maximum Value:</label>
      <div className="form-group">
        <input
          type="number"
          className="form-control"
          placeholder="Maximum Value"
          defaultValue={param.Value.Max}
        />
      </div>

      <label>Increment:</label>
      <div className="form-group">
        <input
          type="number"
          className="form-control"
          placeholder="Increment"
          defaultValue={param.Value.Increment}
        />
      </div>
    </>
  );
}

export default RangeParameter;
