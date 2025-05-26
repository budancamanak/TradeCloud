import { useState, useEffect, useRef } from "react";
import { Route, useParams } from "react-router-dom";
import ReactECharts from "echarts-for-react";
import Fetcher from "../../utils/network";
import dayjs from "dayjs";
const upColor = "#ec0000";
const upBorderColor = "#8A0000";
const downColor = "#00da3c";
const downBorderColor = "#008F28";
function ExecutionChart() {
  const { executionId } = useParams();
  const [prices, setPrices] = useState([]);
  const [categories, setCategories] = useState([]);
  const [values, setValues] = useState([]);
  const [longCount, setLongCount] = useState(0);
  const [shortCount, setShortCount] = useState(0);
  const [analysis, setAnalysis] = useState(null);
  const chartRef = useRef(null);
  const fetcher = new Fetcher();

  useEffect(() => {
    fetcher.get("Chart/Execution/" + executionId + "/Prices").then((result) => {
      console.log("got prices", result);
      setPrices(result);
    });
    fetcher.get(`AnalysisExecutions/${executionId}/Details`).then((result) => {
      console.log("setting AnalysisExecutions", result);
      setAnalysis(result);
      if (
        result == null ||
        result.pluginExecutions == null ||
        result.pluginExecutions.length == 0
      ) {
        setLongCount(0);
        setShortCount(0);
        return;
      }
      let lc = 0;
      let sc = 0;
      result.pluginExecutions.forEach((pluginItem) => {
        pluginItem.outputs.forEach((output) => {
          if ("Open Long" === output.signalType) lc++;
          if ("Open Short" === output.signalType) sc++;
        });
      });
      setLongCount(lc);
      setShortCount(sc);
    });
  }, [executionId]);
  useEffect(() => {
    const categoryData = [];
    const values = [];
    for (var i = 0; i < prices.length; i++) {
      // open，close，lowest，highest
      categoryData.push(prices[i].timestamp);
      values.push([
        prices[i].open,
        prices[i].close,
        prices[i].low,
        prices[i].high,
      ]);
    }
    setCategories(categoryData);
    setValues(values);
  }, [prices]);

  const option = {
    tooltip: {
      trigger: "axis",
      axisPointer: {
        type: "cross",
      },
    },
    grid: {
      left: "2%",
      right: "2%",
      bottom: "2%",
    },
    xAxis: {
      type: "category",
      data: categories,
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
        name: "SERIESTEXT",
        type: "candlestick",
        data: values,
        itemStyle: {
          color: upColor,
          color0: downColor,
          borderColor: upBorderColor,
          borderColor0: downBorderColor,
        },
        markPoint: {
          label: {
            formatter: function (param) {
              return param != null ? Math.round(param.value) + "" : "";
            },
          },
          data: [
            {
              name: "Mark",
              coord: ["2013/5/31", 12300],
              value: 2300,
              itemStyle: {
                color: "rgb(41,60,85)",
              },
            },
            {
              name: "highest value",
              type: "max",
              valueDim: "highest",
            },
            {
              name: "lowest value",
              type: "min",
              valueDim: "lowest",
            },
            {
              name: "average value on close",
              type: "average",
              valueDim: "close",
            },
          ],
          tooltip: {
            formatter: function (param) {
              return param.name + "<br>" + (param.data.coord || "");
            },
          },
        },
      },
    ],
  };

  return (
    <>
      <div className="col-12">
        <div className="card">
          <div className="card-header">
            <h3 className="card-title">Execution Info #{executionId}</h3>
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
              <div className="col-md-4">
                <ul>
                  <li>
                    Plugin Name:{" "}
                    <span className="font-weight-bold">
                      {analysis?.pluginInfo?.name}
                    </span>
                  </li>
                  <li>Ticker</li>
                  <li>
                    Start Date:{" "}
                    <span className="font-weight-bold">
                      {dayjs(analysis?.startDate).format("DD/MMM/YYYY")}
                    </span>
                  </li>
                  <li>
                    End Date:{" "}
                    <span className="font-weight-bold">
                      {dayjs(analysis?.endDate).format("DD/MMM/YYYY")}
                    </span>
                  </li>
                </ul>
              </div>
              <div className="col-md-4">
                <ul>
                  <li>
                    Execution status:{" "}
                    <span className={analysis?.status}>{analysis?.status}</span>
                  </li>
                  <li>
                    Output Count:{" "}
                    <span className="font-weight-bold">
                      {longCount + shortCount}
                    </span>
                  </li>
                  <li>
                    Long Count:
                    <span className="font-weight-bold">{longCount}</span>
                  </li>
                  <li>
                    Short Count:{" "}
                    <span className="font-weight-bold">{shortCount}</span>
                  </li>
                </ul>
              </div>
            </div>
          </div>
        </div>
        <div className="card">
          <div className="card-header">
            <h3 className="card-title">Chart of Execution #{executionId}</h3>
            <div className="card-tools">
              <button
                type="button"
                className="btn btn-tool"
                // data-card-widget="collapse"
              >
                <i class="fas fa-expand"></i>
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
            <ReactECharts option={option} ref={chartRef} style={{ height: "600px" }} />
          </div>
        </div>
      </div>
    </>
  );
}

export default ExecutionChart;
