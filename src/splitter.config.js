const path = require("path");
const fableUtils = require("fable-utils");

function resolve(filePath) {
  return path.resolve(__dirname, filePath)
}


module.exports = {
  entry: resolve("./code_lightner.fsproj"),
  outDir: resolve("../output"),
  babel: fableUtils.resolveBabelOptions({
    plugins: ["transform-es2015-modules-commonjs"],
  }),
}
