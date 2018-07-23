"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.map = map;
exports.mapError = mapError;
exports.bind = bind;

var _Symbol = require("./Symbol");

var _Symbol2 = _interopRequireDefault(_Symbol);

var _Util = require("./Util");

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

class Result {
  constructor(tag, data) {
    this.tag = tag | 0;
    this.data = data;
  }
  Equals(other) {
    return (0, _Util.equalsUnions)(this, other);
  }
  CompareTo(other) {
    return (0, _Util.compareUnions)(this, other);
  }
  [_Symbol2.default.reflection]() {
    return {
      type: "Microsoft.FSharp.Core.FSharpResult",
      interfaces: ["FSharpUnion", "System.IEquatable", "System.IComparable"],
      cases: [["Ok", (0, _Util.GenericParam)("T")], ["Error", (0, _Util.GenericParam)("TError")]]
    };
  }
}
exports.default = Result;
function map(f, result) {
  return result.tag === 0 ? new Result(0, f(result.data)) : result;
}
function mapError(f, result) {
  return result.tag === 1 ? new Result(1, f(result.data)) : result;
}
function bind(f, result) {
  return result.tag === 0 ? f(result.data) : result;
}