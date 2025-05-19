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
