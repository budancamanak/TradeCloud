function RangeParameter({ def_value }) {
  return (
    <>
      <input type="text" placeholder="Default Value" value={def_value} />
      <input type="text" placeholder="Minimum Value" value={def_value} />
      <input type="text" placeholder="Maximum Value" value={def_value} />
      <input type="text" placeholder="Increment" value={def_value} />
    </>
  );
}

export default RangeParameter;
