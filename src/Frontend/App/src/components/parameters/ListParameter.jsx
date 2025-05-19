function ListParameter({ def_value }) {
  return (
    <>
      <div className="col-md-6">
        <div className="form-group">
          <label>Range Type</label>
          <input type="text" placeholder="Default Value" value={def_value} />
        </div>
        <div className="form-group">
          <input type="text" placeholder="Minimum Value" value={def_value} />
          <input type="text" placeholder="Maximum Value" value={def_value} />
          <input type="text" placeholder="Increment" value={def_value} />
        </div>
      </div>
    </>
  );
}

export default ListParameter;
