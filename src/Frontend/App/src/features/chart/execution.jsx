import { useState, useEffect, useRef } from "react";
import { Route, useParams } from "react-router-dom";
import ReactECharts from "echarts-for-react";
import Fetcher from "../../utils/network";
import dayjs from "dayjs";
import { cloneDeep } from "lodash";
import PluginExecutionPaginator from "../../components/actionButtons/PluginExecutionPaginator";
import Popover from "react-bootstrap/Popover";
import OverlayTrigger from "react-bootstrap/OverlayTrigger";

const downColor = "#ec0000";
const downBorderColor = "#8A0000";
const upColor = "#00da3c";
const upBorderColor = "#008F28";
function ExecutionChart() {
  const default_option = {
    tooltip: {
      trigger: "axis",
      axisPointer: {
        type: "cross",
      },
    },
    grid: {
      left: "0%",
      right: "5%",
      bottom: "5%",
    },
    xAxis: {
      type: "category",
      data: [],
      boundaryGap: false,
      axisLine: { onZero: false },
      splitLine: { show: false },
      min: "dataMin",
      max: "dataMax",
    },
    yAxis: {
      scale: true,
      position: "right",
      splitArea: {
        show: true,
      },
    },
    dataZoom: [
      {
        type: "inside",
        start: 50,
        end: 100,
      },
      {
        show: true,
        type: "slider",
        top: "90%",
        start: 50,
        end: 100,
      },
    ],
    series: [
      {
        name: "ExecutionChart",
        type: "candlestick",
        data: [],
        itemStyle: {
          color: upColor,
          color0: downColor,
          borderColor: upBorderColor,
          borderColor0: downBorderColor,
        },
        markPoint: {
          label: {
            formatter: function (param) {
              return param != null ? `${param.data.label}` : "";
            },
          },
          data: [],
          tooltip: {
            formatter: function (param) {
              return (
                param.name +
                "<br>" +
                `${param.data.label}@${param.data.coord[1]}`
              );
            },
          },
        },
      },
    ],
  };
  const default_zoom = {
    start: 50,
    startValue: 50,
    end: 100,
    endValue: 100,
  };
  const [option, setOption] = useState(default_option);
  const { executionId } = useParams();
  const [prices, setPrices] = useState({});
  const [analysis, setAnalysis] = useState(null);
  const [showPopover, setShowPopover] = useState(false);
  const [selectedPluginExecution, setSelectedPluginExecution] = useState(null);

  const [selectedPluginExecutionIndex, setSelectedPluginExecutionIndex] =
    useState(0);
  const chartRef = useRef(null);
  const [currentZoom, setCurrentZoom] = useState(default_zoom);

  const fetcher = new Fetcher();

  useEffect(() => {
    fetcher.get("Chart/Execution/" + executionId + "/Prices").then((result) => {
      const map = {};
      result.forEach((item) => {
        map[dayjs(item.timestamp).format("DD/MM/YYYY HH:mm")] = item;
      });
      setPrices(map);
    });
    fetcher.get(`AnalysisExecutions/${executionId}/Details`).then((result) => {
      result.pluginExecutions.forEach((item) => {
        const parsed = JSON.parse(item.paramSet);
        item.parameters = parsed;
        delete item.paramSet;
      });
      setAnalysis(result);
      if (result.pluginExecutions.length > 0)
        setSelectedPluginExecution(result.pluginExecutions[0]);
    });
  }, [executionId]);

  useEffect(() => {
    if (
      !analysis ||
      analysis.pluginExecutions == null ||
      analysis.pluginExecutions.length <= 0
    )
      return;
    const newOption = cloneDeep(option);
    let counter = 0;
    selectedPluginExecution.outputs.forEach((signal) => {
      if (
        "Open Long" !== signal.signalType &&
        "Open Short" !== signal.signalType
      )
        return;
      const date = dayjs(signal.signalDate).format("DD/MM/YYYY HH:mm");
      let value = prices[date].high;
      let rotate = 180;
      let symbolOffset = [0, "-15"];
      if ("Open Long" == signal.signalType) {
        value = prices[date].low;
        rotate = 0;
        symbolOffset = [0, "15"];
      }
      newOption.series[0].markPoint.data.push({
        symbol: "triangle",
        symbolSize: 25,
        symbolRotate: rotate,
        symbolOffset: symbolOffset,
        name: `Output${++counter}`,
        coord: [date, value],
        // value: value,
        label: signal.signalType,
        // valueDim: "lowest",
        itemStyle: {
          color: "rgb(41,60,85)",
        },
      });
    });

    chartRef.current.getEchartsInstance().setOption(newOption, true);
    chartRef.current.getEchartsInstance().dispatchAction({
      type: "dataZoom",
      start: currentZoom.start,
      startValue: currentZoom.startValue,
      end: currentZoom.end,
      endValue: currentZoom.endValue,
    });
  }, [selectedPluginExecution]);

  useEffect(() => {
    const categoryData = [];
    const values = [];
    const keys = Object.keys(prices);
    for (var i = 0; i < keys.length; i++) {
      const item = prices[keys[i]];
      // open，close，lowest，highest
      categoryData.push(dayjs(item.timestamp).format("DD/MM/YYYY HH:mm"));
      values.push([item.open, item.close, item.low, item.high]);
    }
    const newOption = cloneDeep(option);
    newOption.series[0].data = values;
    newOption.xAxis.data = categoryData;
    setOption(newOption);
    // chartRef.current.getEchartsInstance().setOption(newOption);
  }, [prices]);

  const onDataZoom = (params) => {
    if (params.batch) {
      currentZoom.start = params.batch[0].start;
      currentZoom.end = params.batch[0].end;
    } else {
      currentZoom.startValue = params.startValue;
      currentZoom.endValue = params.endValue;
    }
    setCurrentZoom(currentZoom);
  };

  const onEvents = {
    datazoom: onDataZoom,
  };

  const hidePopover = () => {
    setShowPopover(false);
  };

  const formatDate = (date) => {
    return dayjs(date).format("DD/MM/YYYY HH:mm");
  };

  const longCountOfSelectedExecution = (signalType) => {
    if (!selectedPluginExecution) return 0;
    let count = 0;
    selectedPluginExecution.outputs.forEach((output) => {
      if (!signalType) {
        if (!output.signalType.includes("Close")) count++;
      } else if (signalType === output.signalType) count++;
    });
    return count;
  };

  const parametersOfSelectedExecution = (
    <>
      {selectedPluginExecution?.parameters && (
        <>
          {Object.keys(selectedPluginExecution.parameters).map((key) => (
            <div key={key}>
              <span>
                <strong>{key}: </strong>
              </span>
              <span>{selectedPluginExecution.parameters[key]}</span>
            </div>
          ))}
        </>
      )}
    </>
  );

  const pluginExecutionDetails = (
    <div style={{ display: "flex", flexDirection: "column", gap: "5px" }}>
      {selectedPluginExecution && (
        <>
          <span
            className={selectedPluginExecution?.status}
            style={{ marginRight: "5px" }}
          >
            <strong>Status: </strong>
            {selectedPluginExecution?.status === "Success" && (
              <>
              <i className="fas fa-check-circle"></i>
              </>
            )}
            {selectedPluginExecution?.status === "Failure" && (
              <>
              <i className="fas fa-exclamation-circle"></i>
              </>
            )}
            {selectedPluginExecution?.status}
          </span>
          <span>
            <strong>Queued date: </strong>
            {formatDate(selectedPluginExecution.queuedDate)}
          </span>
          <span>
            <strong>Start date: </strong>
            {formatDate(selectedPluginExecution.runStartDate)}
          </span>
          <span>
            <strong>End date: </strong>
            {formatDate(selectedPluginExecution.finishDate)}
          </span>
          <span>
            <strong>Total Signal #: </strong>
            {longCountOfSelectedExecution("")}
          </span>
          <span>
            <strong>Long Signal #: </strong>
            {longCountOfSelectedExecution("Open Long")}
          </span>
          <span>
            <strong>Short Signal #: </strong>
            {longCountOfSelectedExecution("Open Short")}
          </span>
          <span style={{ textDecoration: "underline" }}>
            <strong>Parameters: </strong>
          </span>
          {parametersOfSelectedExecution}
        </>
      )}
    </div>
  );

  return (
    <>
      <div className="col-12">
        <div className="card">
          <div className="card-header">
            <h3 className="card-title">
              <span className={analysis?.status} style={{ marginRight: "5px" }}>
                {analysis?.status === "Success" && (
                  <i className="fas fa-check-circle"></i>
                )}
                {analysis?.status === "Failure" && (
                  <i className="fas fa-exclamation-circle"></i>
                )}
              </span>
              <span className="font-weight-bold" style={{ marginRight: "5px" }}>
                {analysis?.pluginInfo?.name}
              </span>
              <span style={{ marginRight: "5px" }}>
                {analysis?.ticker}({analysis?.timeframe}) @
              </span>
              <span className="font-weight-bold" style={{ marginRight: "5px" }}>
                {dayjs(analysis?.startDate).format("DD/MMM/YYYY")} -
              </span>
              <span className="font-weight-bold" style={{ marginRight: "5px" }}>
                {dayjs(analysis?.endDate).format("DD/MMM/YYYY")}
              </span>
            </h3>
            <div className="card-tools">
              <PluginExecutionPaginator
                executions={analysis?.pluginExecutions}
                onAction={(index, item) => {
                  setSelectedPluginExecution(item);
                  setSelectedPluginExecutionIndex(index);
                }}
              />
              <OverlayTrigger
                trigger="click"
                key={"bottom"}
                show={showPopover}
                onToggle={(next) => setShowPopover(next)}
                placement={"bottom"}
                overlay={
                  <Popover id={`popover-positioned-${"bottom"}`}>
                    <Popover.Header as="h3">
                      {
                        <>
                          Details of execution #
                          {selectedPluginExecutionIndex + 1}
                          <i
                            className="fas fa-times float-right"
                            style={{ cursor: "pointer" }}
                            onClick={hidePopover}
                          ></i>
                        </>
                      }
                    </Popover.Header>
                    <Popover.Body>{pluginExecutionDetails}</Popover.Body>
                  </Popover>
                }
              >
                <button type="button" className="btn btn-tool">
                  <i className="fas fa-info"></i>
                </button>
              </OverlayTrigger>

              <button
                type="button"
                className="btn btn-tool"
                // data-card-widget="collapse"
              >
                <i className="fas fa-expand"></i>
              </button>
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
            <ReactECharts
              option={option}
              ref={chartRef}
              style={{ height: "600px" }}
              onEvents={onEvents}
            />
          </div>
        </div>
      </div>
    </>
  );
}

export default ExecutionChart;
