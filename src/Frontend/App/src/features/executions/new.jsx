import { useState, useEffect } from "react";
// import $ from "jquery";
import AuthService from "../../services/Auth.Service";
import { ToastUtility } from "../../utils/toast-utility";
import Fetcher from "../../utils/network";
import DataTable from "datatables.net-bs4";
import Select from "react-select";
import { getPluginParameterRange, getPluginParameterValueType } from "../../utils/helpers";
import PluginParameter from "../../components/parameters/PluginParameter";
// import "../../lib/plugins/jquery/jquery.min";
// import "../../lib/plugins/jquery-ui/jquery-ui.min";
// import "../../lib/plugins/datatables/jquery.dataTables.min";

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
              {pluginParameters.map(function (param) {
                return (
                  <PluginParameter key={param.Name} param={param} />
                );
              })}
            </div>
          </div>
        </div>
        <div className="card-footer">Visit documentation </div>
      </div>
    </>
  );
}

export default NewExecution;
