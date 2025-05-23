import { useState, useEffect } from "react";

function AnalysisActionButton({ ...props }) {
  const onAction = (type) => {
    if (!props.onAction) return;
    props.onAction(type, props.execution);
  };
  return (
    <>
      <div class="btn-group">
        <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">
          {props.title || "Action"}
        </button>

        <div class="dropdown-menu" role="menu">
          <a class="dropdown-item" href="#" onClick={() => onAction("Details")}>
            Details
          </a>
          {props.execution.status === "Init" && (
            <a class="dropdown-item" href="#" onClick={() => onAction("Start")}>
              Start
            </a>
          )}
          {(props.execution.status === "Success" ||
            props.execution.status === "Failure") && (
            <a
              class="dropdown-item"
              href="#"
              onClick={() => onAction("Restart")}
            >
              Restart
            </a>
          )}
          {props.execution.status === "Running" && (
            <a
              class="dropdown-item"
              href="#"
              onClick={() => onAction("Cancel")}
            >
              Cancel
            </a>
          )}
          {props.execution.status === "Success" && (
            <a
              class="dropdown-item"
              href="#"
              onClick={() => onAction("ViewInChart")}
            >
              View in Chart
            </a>
          )}
          <div class="dropdown-divider"></div>
          <a class="dropdown-item" href="#" onClick={() => onAction("Delete")}>
            Delete
          </a>
        </div>
      </div>
    </>
  );
}

export default AnalysisActionButton;
