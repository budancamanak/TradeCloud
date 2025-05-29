import { useState } from "react";

function PluginExecutionPaginator({ ...props }) {
  const [current, setCurrent] = useState(1);
  const [leftDisabled, setLeftDisabled] = useState(true);
  const [rightDisabled, setRightDisabled] = useState(false);
  const onAction = (increment) => {
    if (!props.onAction) return;
    if (!props.executions) return;
    let next = current + increment;
    if (next < 1) next = 1;
    setLeftDisabled(next == 1);
    if (next >= props.executions.length) next = props.executions.length - 1;
    setRightDisabled(next == props.executions.length - 1);
    if (next == current) return;
    setCurrent(next);
    props.onAction(props.executions[next]);
  };
  return (
    <>
      <button
        type="button"
        title="Previous Plugin Execution"
        style={{ cursor: "pointer" }}
        disabled={leftDisabled}
        className="btn btn-tool"
        onClick={() => onAction(-1)}
      >
        <i className="fas fa-arrow-left"></i>
      </button>
      <span>
        {current} / {props.executions?.length}
      </span>
      <button
        type="button"
        title="Next Plugin Execution"
        style={{ cursor: "pointer" }}
        disabled={rightDisabled}
        className="btn btn-tool"
        onClick={() => onAction(1)}
      >
        <i className="fas fa-arrow-right"></i>
      </button>
    </>
  );
}

export default PluginExecutionPaginator;
