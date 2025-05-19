import { useState } from "react";
import React, { KeyboardEventHandler } from "react";

import CreatableSelect from "react-select/creatable";

function ListParameter({ param }) {
  const components = {
    DropdownIndicator: null,
  };
  const createOption = (label) => ({
    label,
    value: label,
  });
  const [inputValue, setInputValue] = useState("");
  const [value, setValue] = React.useState([]);
  const handleKeyDown = (event) => {
    if (!inputValue) return;
    switch (event.key) {
      case "Enter":
      case "Tab":
        setValue((prev) => [...prev, createOption(inputValue)]);
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
            onChange={(newValue) => setValue(newValue)}
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
