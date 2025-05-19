function SingleParameter({ param }) {
  return (
    <>
      <label>Parameter Value:</label>
      <div className="form-group">
        <input
          type="number"
          className="form-control"
          placeholder="Value"
          defaultValue={param.Value}
        />
      </div>
    </>
  );
}

export default SingleParameter;
