"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.PromiseImpl = exports.Promise = undefined;

var _Result = require("../fable-core/Result");

var _Result2 = _interopRequireDefault(_Result);

var _Symbol2 = require("../fable-core/Symbol");

var _Symbol3 = _interopRequireDefault(_Symbol2);

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

const _Promise = function (__exports) {
  const result = __exports.result = function (a) {
    return a.then($var1 => new _Result2.default(0, $var1), $var2 => new _Result2.default(1, $var2));
  };

  const mapResult = __exports.mapResult = function (fn, a) {
    return a.then(function (result_1) {
      return (0, _Result.map)(fn, result_1);
    });
  };

  const bindResult = __exports.bindResult = function (fn, a) {
    return a.then(function (a_1) {
      return a_1.tag === 1 ? Promise.resolve(new _Result2.default(1, a_1.data)) : result(fn(a_1.data));
    });
  };

  const mapResultError = __exports.mapResultError = function (fn, a) {
    return a.then(function (result_1) {
      return (0, _Result.mapError)(fn, result_1);
    });
  };

  const tap = __exports.tap = function (fn, a) {
    return a.then(function (x) {
      fn(x);
      return x;
    });
  };

  const PromiseBuilder = __exports.PromiseBuilder = class PromiseBuilder {
    [_Symbol3.default.reflection]() {
      return {
        type: "Fable.PowerPack.Promise.PromiseBuilder",
        properties: {}
      };
    }

    constructor() {}

    For_0(seq, body) {
      let p = Promise.resolve(null);

      for (let a of seq) {
        p = p.then(() => body(a));
      }

      return p;
    }

    While(guard, p) {
      if (guard()) {
        return p.then(() => this.While(guard, p));
      } else {
        return Promise.resolve(null);
      }
    }

    TryFinally(p, compensation) {
      return p.then(x => {
        compensation();
        return x;
      }, er => {
        compensation();
        throw er;
      });
    }

    Delay(generator) {
      return {
        then: (f1, f2) => {
          try {
            return generator().then(f1, f2);
          } catch (er) {
            if (f2 == null) {
              return Promise.reject(er);
            } else {
              try {
                return Promise.resolve(f2(er));
              } catch (er_1) {
                return Promise.reject(er_1);
              }
            }
          }
        },
        catch: f => {
          try {
            return generator().catch(f);
          } catch (er_2) {
            try {
              return Promise.resolve(f(er_2));
            } catch (er_3) {
              return Promise.reject(er_3);
            }
          }
        }
      };
    }

    Using(resource, binder) {
      return this.TryFinally(binder(resource), () => {
        resource.Dispose();
      });
    }

  };
  (0, _Symbol2.setType)("Fable.PowerPack.Promise.PromiseBuilder", PromiseBuilder);
  return __exports;
}({});

exports.Promise = _Promise;

const PromiseImpl = exports.PromiseImpl = function (__exports) {
  const promise = __exports.promise = new _Promise.PromiseBuilder();
  return __exports;
}({});