import { useState, useEffect, useRef } from "react";
import { Route, useParams } from "react-router-dom";
import ReactECharts from "echarts-for-react";
import Fetcher from "../../utils/network";
import dayjs from "dayjs";
import { cloneDeep } from "lodash";

const upColor = "#ec0000";
const upBorderColor = "#8A0000";
const downColor = "#00da3c";
const downBorderColor = "#008F28";
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
      textStyle: {
        color: '#8392A5'
      },
      handleIcon:
        'path://M10.7,11.9v-1.3H9.3v1.3c-4.9,0.3-8.8,4.4-8.8,9.4c0,5,3.9,9.1,8.8,9.4v1.3h1.3v-1.3c4.9-0.3,8.8-4.4,8.8-9.4C19.5,16.3,15.6,12.2,10.7,11.9z M13.3,24.4H6.7V23h6.6V24.4z M13.3,19.6H6.7v-1.4h6.6V19.6z',
      dataBackground: {
        areaStyle: {
          color: '#8392A5'
        },
        lineStyle: {
          opacity: 0.8,
          color: '#8392A5'
        }
      },
      brushSelect: true
    },
    {
      type: 'inside'
    }
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
              return param != null ? Math.round(param.value) + "" : "";
            },
          },
          data: [],
          tooltip: {
            formatter: function (param) {
              return param.name + "<br>" + (param.data.coord || "");
            },
          },
        },
      },
    ],
  };
  const [option, setOption] = useState(default_option);
  const { executionId } = useParams();
  const [prices, setPrices] = useState([]);
  const [categories, setCategories] = useState([]);
  const [values, setValues] = useState([]);
  const [longCount, setLongCount] = useState(0);
  const [shortCount, setShortCount] = useState(0);
  const [analysis, setAnalysis] = useState(null);

  const fetcher = new Fetcher();

  useEffect(() => {
    fetcher.get("Chart/Execution/" + executionId + "/Prices").then((result) => {
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
    if (
      !analysis ||
      analysis.pluginExecutions == null ||
      analysis.pluginExecutions.length <= 0
    )
      return;
    const newOption = cloneDeep(option);
    // analysis.pluginExecutions.forEach((exec) => {
    analysis.pluginExecutions[0].outputs.forEach((signal) => {
      newOption.series[0].markPoint.data.push({
        name: "Mark",
        coord: [dayjs(signal.signalDate).format("DD/MM/YYYY HH:mm"), 88300],
        value: 8,
        // valueDim: "lowest",
        itemStyle: {
          color: "rgb(41,60,85)",
        },
      });
    });
    // });
    // newOption.series[0].markPoint.data.push({
    //   name: "lowest value",
    //   type: "min",
    //   valueDim: "lowest",
    // });

    setOption(newOption);
  }, [analysis]);

  useEffect(() => {
    const categoryData = [];
    const values = [];
    for (var i = 0; i < prices.length; i++) {
      // open，close，lowest，highest
      categoryData.push(dayjs(prices[i].timestamp).format("DD/MM/YYYY HH:mm"));
      values.push([
        prices[i].open,
        prices[i].close,
        prices[i].low,
        prices[i].high,
      ]);
    }
    setCategories(categoryData);
    setValues(values);
    const newOption = cloneDeep(option);
    newOption.series[0].data = values;
    newOption.xAxis.data = categoryData;
    setOption(newOption);
  }, [prices]);

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
            <ReactECharts option={option} style={{ height: "600px" }} />
          </div>
        </div>
      </div>
    </>
  );
}

export default ExecutionChart;
