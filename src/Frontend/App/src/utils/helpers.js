export function validateEmail(mail) {
  return /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(mail);
}

export const getPluginParameterRange = () => {
  return [
    { type: 0, name: "Single" },
    { type: 1, name: "Range" },
    { type: 2, name: "List" },
  ];
};
export const getPluginParameterValueType = () => {
  return [
    { type: 0, name: "Integer" },
    { type: 1, name: "Decimal" },
    { type: 2, name: "String" },
  ];
};

export const getTimeFrames = () => {
  return [
    { value: "5m", name: "5m" },
    { value: "15m", name: "15m" },
    { value: "30m", name: "30m" },
    { value: "1h", name: "1H" },
    { value: "2h", name: "2H" },
    { value: "4h", name: "4H" },
    // { value: "8h", name: "8H" },
    { value: "12h", name: "12H" },
    { value: "1D", name: "1D" },
  ];
};

// export default async function fetcher(url, options) {
//   return (await fetch(url, updateOptions(options))).json();
// }

// public enum ParameterType
// {
//     Int,
//     Double,
//     Str
// }

// public enum ParameterRange
// {
//     Single,
//     Range,
//     List
// }
