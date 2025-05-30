import { useState, useEffect } from "react";

function AnalysisActionButton({ ...props }) {
  const onAction = (type) => {
    if (!props.onAction) return;
    props.onAction(type, props.execution);
  };
  return (
    <>
      <div className="btn-group">
        <button
          type="button"
          className="btn btn-default dropdown-toggle"
          data-toggle="dropdown"
        >
          {props.title || "Action"}
        </button>

        <div className="dropdown-menu" role="menu">
          <a className="dropdown-item" href="#" onClick={() => onAction("Details")}>
            Details
          </a>
          {props.execution.status !== "Running" && (
            <a className="dropdown-item" href="#" onClick={() => onAction("Edit")}>
              Edit
            </a>
          )}
          {props.execution.status === "Init" && (
            <a className="dropdown-item" href="#" onClick={() => onAction("Start")}>
              Start
            </a>
          )}
          {(props.execution.status === "Success" ||
            props.execution.status === "Failure") && (
            <a
              className="dropdown-item"
              href="#"
              onClick={() => onAction("Restart")}
            >
              Restart
            </a>
          )}
          {props.execution.status === "Running" && (
            <a
              className="dropdown-item"
              href="#"
              onClick={() => onAction("Cancel")}
            >
              Cancel
            </a>
          )}
          {(props.execution.status === "Success" ||
            props.execution.status === "Failure") && (
            <a
              className="dropdown-item"
              href="#"
              onClick={() => onAction("ViewInChart")}
            >
              View in Chart
            </a>
          )}
          <div className="dropdown-divider"></div>
          <a className="dropdown-item" href="#" onClick={() => onAction("Delete")}>
            Delete
          </a>
        </div>
      </div>
    </>
  );
}

export default AnalysisActionButton;
