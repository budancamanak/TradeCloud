import { useState } from "react";
import React, { KeyboardEventHandler } from "react";

import CreatableSelect from "react-select/creatable";

function ListParameter({ ...props }) {
  const components = {
    DropdownIndicator: null,
  };
  const createOption = (label) => ({
    label,
    value: label,
  });
  const [inputValue, setInputValue] = useState("");
  const [value, setValue] = React.useState([]);

  const setListValues = (new_value) => {
    setValue((prev) => [...prev, createOption(new_value)]);
    if (!Array.isArray(props.param.Value)) props.param.Value = [];
    props.param.Value = [...props.param.Value, new_value];
    console.log(props.param.Value);
  };
  const xOnChange = (values) => {
    console.log(values);
    setValue(values);
    if (!Array.isArray(props.param.Value)) props.param.Value = [];
    props.param.Value = values.map((f) => f.value);
  };
  const handleKeyDown = (event) => {
    if (!inputValue) return;
    switch (event.key) {
      case "Enter":
      case "Tab":
        setListValues(inputValue);
        setInputValue("");
        event.preventDefault();
    }
  };
  return (
    <>
      <div className="col-md-6">
        <div
          className="form-group"
          style={{ minWidth: "120px", maxWidth: "120px" }}
        >
          <label>List of Values</label>
          <CreatableSelect
            components={components}
            inputValue={inputValue}
            isClearable
            isMulti
            menuIsOpen={false}
            onChange={(newValue) => xOnChange(newValue)}
            onInputChange={(newValue) => setInputValue(newValue)}
            onKeyDown={handleKeyDown}
            placeholder="Type something and press enter..."
            value={value}
          />
        </div>
      </div>
    </>
  );
}

export default ListParameter;
