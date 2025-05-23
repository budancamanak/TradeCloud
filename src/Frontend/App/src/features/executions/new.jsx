// import $ from 'jquery';
import { useState, useEffect } from "react";
// import DateRangePicker from "react-bootstrap-daterangepicker";
import { ToastUtility } from "../../utils/toast-utility";
import Fetcher from "../../utils/network";
import Select from "react-select";
import { MemoizedPluginParameter } from "../../components/parameters/PluginParameter";
import { getTimeFrames } from "../../utils/helpers";

function NewExecution() {
  const [tickers, setTickers] = useState([]);
  const [plugins, setPlugins] = useState([]);
  const [pluginParameters, setPluginParameters] = useState([]);
  const fetcher = new Fetcher();
  const [model, setModel] = useState({
    pluginIdentifier: "",
    symbol: "",
    startDate: "",
    endDate: "",
    timeframe: "",
    paramSet: "",
    tradingParams: "",
  });

  useEffect(() => {
    fetcher.get("AvailableTickers").then((result) => {
      setTickers(result);
    });
    fetcher.get("AvailablePlugins").then((result) => {
      console.log("setting plugins");
      setPlugins(result);
    });
  }, []);
  useEffect(() => {
    $("#executionDateRange").daterangepicker();
    $("#executionDateRange").on("apply.daterangepicker", function (ev, picker) {
      const start = picker.startDate.toISOString();
      const end = picker.endDate.toISOString();
      console.log(model);
      setModel({ ...model, startDate: start, endDate: end });
    });
  }, [model]);

  const getPluginDefaultParamset = async (plugin) => {
    const mr = await fetcher.get(
      `AvailablePluginParameters?identifier=${plugin.identifier}`
    );
    let params = Object.entries(mr).flatMap((x) => [x[1]]);
    console.log(model);
    setPluginParameters(params);
    setModel({
      ...model,
      pluginIdentifier: plugin.identifier,
      // paramSet: params,
    });
  };

  const setSelectedTimeFrame = (tf) => {
    console.log(model);
    setModel({ ...model, timeframe: tf.value });
  };

  const setSelectedSymbol = (tf) => {
    console.log(model);
    setModel({ ...model, symbol: tf.symbol });
  };

  const saveAnalysisExecutionConfirmation = () => {
    console.log(model);
    setModel({ ...model, paramSet: JSON.stringify(pluginParameters) });
    $("#saveConfirmationModal").modal("show");
  };

  const saveAnalysisExecution = async () => {
    console.log("saveAnalysisExecution", pluginParameters);
    ToastUtility.info("Saving analysis execution...");
    // const data = JSON.stringify(model);
    const mr = await fetcher.post("AnalysisExecutions", model);
    console.log(mr);
    ToastUtility.success(`Result: ${mr.message}`);
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
            <h3 className="card-title">Plugin Information</h3>

            <div className="card-tools">
              <button
                type="button"
                className="btn btn-tool"
                data-card-widget="collapse"
              >
                <i className="fas fa-minus"></i>
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
                    onChange={setSelectedSymbol}
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
                  <Select
                    onChange={setSelectedTimeFrame}
                    getOptionLabel={(option) => `${option.name}`}
                    getOptionValue={(option) => `${option.value}`}
                    options={getTimeFrames()}
                  />
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
            </div>
          </div>

          <div className="card-body">
            <div className="row" style={{ gap: "15px" }}>
              {pluginParameters.map((param) => (
                <MemoizedPluginParameter key={param.Name} param={param} />
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
