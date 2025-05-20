import { useState, useEffect, useRef, createRef } from "react";
import AuthService from "../../services/Auth.Service";
import { ToastUtility } from "../../utils/toast-utility";
import Fetcher from "../../utils/network";
import Select from "react-select";
import PluginParameter from "../../components/parameters/PluginParameter";

function NewExecution() {
  const [tickers, setTickers] = useState([]);
  const [plugins, setPlugins] = useState([]);
  const [pluginParameters, setPluginParameters] = useState([]);
  const fetcher = new Fetcher();

  useEffect(() => {
    fetcher.get("AvailableTickers").then((result) => {
      console.log("setting tickers");
      setTickers(result);
    });
  }, []);
  useEffect(() => {
    fetcher.get("AvailablePlugins").then((result) => {
      console.log("setting plugins");
      setPlugins(result);
    });
  }, []);
  useEffect(() => {
    $("#executionDateRange").daterangepicker();
  }, []);

  const getPluginDefaultParamset = (plugin) => {
    fetcher
      .get(`AvailablePluginParameters?identifier=${plugin.identifier}`)
      .then(function (result) {
        let params = Object.entries(result).flatMap((x) => [x[1]]);
        console.log(params);
        setPluginParameters(params);
      });
  };
  const saveAnalysisExecutionConfirmation = () => {
    $("#saveConfirmationModal").modal("show");
  };

  const saveAnalysisExecution = () => {
    console.log("saveAnalysisExecution", pluginParameters);
    ToastUtility.success("Will save analysis execution");
  };

  const resetExecutionParamsConfirmation = () => {
    $("#resetConfirmationModal").modal("show");
  };

  const resetExecutionParams = () => {
    console.log("resetExecutionParams");
    ToastUtility.warning("Will reset parameters");
  };

  return (
    <>
      <div className="col-md-12">
        <div className="card card-default">
          <div className="card-header">
            <h3 className="card-title">Base Information</h3>

            <div className="card-tools">
              <button
                type="button"
                className="btn btn-tool"
                data-card-widget="collapse"
              >
                <i className="fas fa-minus"></i>
              </button>
              <button
                type="button"
                className="btn btn-tool"
                data-card-widget="remove"
              >
                <i className="fas fa-times"></i>
              </button>
            </div>
          </div>

          <div className="card-body">
            <div className="row">
              <div className="col-md-6">
                <div className="form-group">
                  <label>Plugin to run</label>
                  <Select
                    getOptionLabel={(option) =>
                      `${option.name} (v${option.version})`
                    }
                    onChange={getPluginDefaultParamset}
                    getOptionValue={(option) => `${option.identifier}`}
                    options={plugins}
                  />
                </div>

                <div className="form-group">
                  <label>Symbol to run</label>
                  <Select
                    getOptionLabel={(option) =>
                      `${option.name} (${option.exchangeName})`
                    }
                    getOptionValue={(option) => `${option.id}`}
                    options={tickers}
                  />
                </div>
              </div>

              <div className="col-md-6">
                <div className="form-group">
                  <label>Date range:</label>

                  <div className="input-group">
                    <div className="input-group-prepend">
                      <span className="input-group-text">
                        <i className="far fa-calendar-alt"></i>
                      </span>
                    </div>
                    <input
                      type="text"
                      className="form-control float-right"
                      id="executionDateRange"
                    />
                  </div>
                </div>
                <div className="form-group">
                  <label>Timeframe</label>
                  <select
                    className="form-control select2bs4"
                    style={{ width: "100%" }}
                  >
                    <option value={"5m"}>5m</option>
                    <option value={"15m"}>15m</option>
                    <option value={"30m"}>30m</option>
                    <option value={"60m"}>1H</option>
                    <option value={"120m"}>2H</option>
                    <option value={"240m"}>4H</option>
                    <option value={"480m"}>8H</option>
                    <option value={"720m"}>12H</option>
                    <option value={"1440m"}>1D</option>
                  </select>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div className="card card-default">
          <div className="card-header">
            <h3 className="card-title">Plugin Parameters</h3>

            <div className="card-tools">
              <button
                type="button"
                className="btn btn-tool"
                data-card-widget="collapse"
              >
                <i className="fas fa-minus"></i>
              </button>
              <button
                type="button"
                className="btn btn-tool"
                data-card-widget="remove"
              >
                <i className="fas fa-times"></i>
              </button>
            </div>
          </div>

          <div className="card-body">
            <div className="row" style={{ gap: "15px" }}>
              {pluginParameters.map((param) => (
                <PluginParameter
                  key={param.Name}
                  param={param}
                />
              ))}
            </div>
          </div>
        </div>
        <div className="card-footer">
          <div className="row float-sm-right">
            <div className="col-md-12">
              <a
                className="btn btn-app"
                onClick={resetExecutionParamsConfirmation}
              >
                <i className="fas fa-undo"></i> Reset
              </a>
              <a
                className="btn btn-app"
                onClick={saveAnalysisExecutionConfirmation}
              >
                <i className="fas fa-save"></i> Save
              </a>
            </div>
          </div>
        </div>
      </div>

      {/* below is confirmation modals */}
      <div className="modal fade" id="resetConfirmationModal">
        <div className="modal-dialog">
          <div className="modal-content">
            <div className="modal-header">
              <h4 className="modal-title">Reset Plugin Parameters</h4>
              <button
                type="button"
                className="close"
                data-dismiss="modal"
                aria-label="Close"
              >
                <span aria-hidden="true">&times;</span>
              </button>
            </div>
            <div className="modal-body">
              <p>Do you want to reset plugin parameters?</p>
            </div>
            <div className="modal-footer justify-content-between">
              <button
                type="button"
                className="btn btn-default"
                data-dismiss="modal"
              >
                Close
              </button>
              <button
                type="button"
                className="btn btn-primary"
                onClick={resetExecutionParams}
                data-dismiss="modal"
              >
                Reset changes
              </button>
            </div>
          </div>
        </div>
      </div>
      <div className="modal fade" id="saveConfirmationModal">
        <div className="modal-dialog">
          <div className="modal-content">
            <div className="modal-header">
              <h4 className="modal-title">Save Analysis Execution</h4>
              <button
                type="button"
                className="close"
                data-dismiss="modal"
                aria-label="Close"
              >
                <span aria-hidden="true">&times;</span>
              </button>
            </div>
            <div className="modal-body">
              <p>Do you want to save analysis execution?</p>
            </div>
            <div className="modal-footer justify-content-between">
              <button
                type="button"
                className="btn btn-default"
                data-dismiss="modal"
              >
                Close
              </button>
              <button
                type="button"
                className="btn btn-primary"
                onClick={saveAnalysisExecution}
                data-dismiss="modal"
              >
                Save changes
              </button>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}

export default NewExecution;
