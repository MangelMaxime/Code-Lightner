"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.lighten = lighten;

var _safe = require("colors/safe");

var _safe2 = _interopRequireDefault(_safe);

var _path = require("path");

var path_1 = _interopRequireWildcard(_path);

var _fs = require("fs");

var fs = _interopRequireWildcard(_fs);

var _Option = require("./fable-core/Option");

var _fastPlist = require("fast-plist");

var fast_plist = _interopRequireWildcard(_fastPlist);

var _helpers = require("./js/helpers");

var _String = require("./fable-core/String");

var _List = require("./fable-core/List");

var _Util = require("./fable-core/Util");

var _vscodeTextmate = require("vscode-textmate");

var vscode_textmate = _interopRequireWildcard(_vscodeTextmate);

var _Map = require("./fable-core/Map");

var _Seq = require("./fable-core/Seq");

var _Comparer = require("./fable-core/Comparer");

var _Comparer2 = _interopRequireDefault(_Comparer);

var _Promise = require("./src/Promise");

function _interopRequireWildcard(obj) { if (obj && obj.__esModule) { return obj; } else { var newObj = {}; if (obj != null) { for (var key in obj) { if (Object.prototype.hasOwnProperty.call(obj, key)) newObj[key] = obj[key]; } } newObj.default = obj; return newObj; } }

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

const colors = _safe2.default;

function resolve(path) {
  const segments = [process.cwd(), path];
  return path_1.resolve(...segments);
}

function loadThemeFile(filename) {
  var theme;
  var tokenColors;
  const filePath = resolve(filename);

  if (fs.existsSync(filePath)) {
    const content = fs.readFileSync(resolve(filename), (0, _Option.makeSome)(null)).toString();
    return path_1.extname(filename) === ".json" ? (theme = JSON.parse(content), tokenColors = theme.tokenColors, !(tokenColors == null) ? {
      settings: theme.tokenColors,
      name: theme.name
    } : theme) : fast_plist.parse(content);
  } else {
    console.warn(colors.yellow("Theme file not found: " + filePath));
    return null;
  }
}

const tokenMetadataDecoder = _helpers.StackElementMetadata;
const escapeHtml = _helpers.escapeHtml;

function getInlineStyleFromMetadata(metadata, colorMap) {
  const foreground = tokenMetadataDecoder.getForeground(metadata) | 0;
  const fontStyle = tokenMetadataDecoder.getFontStyle(metadata) | 0;
  let foregroundCSS;
  const color = colorMap[foreground];

  if (!(color == null) ? color !== "#000000" : false) {
    foregroundCSS = "color: " + color;
  } else {
    foregroundCSS = "";
  }

  let fontStyleCSS;

  switch (fontStyle) {
    case 1:
      fontStyleCSS = "font-style: italic";
      break;

    case 2:
      fontStyleCSS = "font-style: bold";
      break;

    case 4:
      fontStyleCSS = "font-style: underline";
      break;

    case -1:
    case 0:
      fontStyleCSS = "";
      break;

    default:
      fontStyleCSS = "";
  }

  return (0, _String.join)(";", (0, _List.filter)($var1 => !(0, _String.isNullOrWhiteSpace)($var1), (0, _List.ofArray)([foregroundCSS, fontStyleCSS])));
}

function lineToHtml(grammar, colorMap, prevState, line) {
  let html = "";
  const lineResult = grammar.tokenizeLine2(line, prevState);
  const tokensLength = ~~(~~lineResult.tokens.length / 2) | 0;

  for (let i = 0; i <= tokensLength; i++) {
    const startIndex = lineResult.tokens[2 * i];
    const nextStartIndex = i + 1 < tokensLength ? lineResult.tokens[2 * i + 2] : line.length;
    const tokenText = escapeHtml(line.substr(~~startIndex, ~~(nextStartIndex - startIndex)));

    if (tokenText === "") {} else {
      const metadata = lineResult.tokens[2 * i + 1];
      const inlineStyle = getInlineStyleFromMetadata(metadata, colorMap);

      if (inlineStyle === "") {
        html = html + "<span>" + tokenText + "</span>";
      } else {
        html = html + "<span style=\"" + inlineStyle + "\">" + tokenText + "</span>";
      }
    }
  }

  return [html, lineResult.ruleStack];
}

function createGrammarFiles(files) {
  return (0, _Util.createObj)((0, _List.map)(function (tupledArg) {
    return [tupledArg[0], tupledArg[1]];
  }, files), 0);
}

function toInlineStyle(propertyName, optionalValue) {
  if (optionalValue == null) {
    return "";
  } else {
    return propertyName + ": " + (0, _Option.getValue)(optionalValue);
  }
}

function loadGrammars(files) {
  return function (builder_) {
    return builder_.Delay(function () {
      const mapScopeName = function (map, file) {
        const filePath = resolve(file);

        if (fs.existsSync(filePath)) {
          const content = fs.readFileSync(filePath, (0, _Option.makeSome)(null));
          const grammar = vscode_textmate.parseRawGrammar(content.toString(), file);
          return (0, _Map.add)(grammar.scopeName, file, map);
        } else {
          console.warn(colors.yellow("File not found: " + filePath));
          return map;
        }
      };

      return Promise.resolve((0, _Seq.fold)(mapScopeName, (0, _Map.create)(null, new _Comparer2.default(_Util.comparePrimitives)), files));
    });
  }(_Promise.PromiseImpl.promise);
}

function lighten(config, code) {
  return function (builder_) {
    return builder_.Delay(function () {
      return loadGrammars(config.grammarFiles).then(function (_arg1) {
        const registryOptions = {
          theme: loadThemeFile(config.themeFile),
          loadGrammar: function (scopeName) {
            var matchValue;
            return matchValue = (0, _Map.tryFind)(scopeName, _arg1), matchValue == null ? function (builder__1) {
              return builder__1.Delay(function () {
                return Promise.resolve(null);
              });
            }(_Promise.PromiseImpl.promise) : function (builder__2) {
              return builder__2.Delay(function () {
                const content = fs.readFileSync(resolve((0, _Option.getValue)(matchValue)), (0, _Option.makeSome)(null));
                const rawGrammar = vscode_textmate.parseRawGrammar(content.toString(), (0, _Option.getValue)(matchValue));
                return Promise.resolve(rawGrammar);
              });
            }(_Promise.PromiseImpl.promise);
          }
        };
        const registry = new vscode_textmate.Registry(registryOptions);
        return registry.loadGrammar(config.scopeName).then(function (grammar) {
          const colorMap = registry.getColorMap();
          let prevStack = vscode_textmate.INITIAL;
          let html = "";
          const arr = (0, _String.split)(code, "\n");

          for (let idx = 0; idx <= arr.length - 1; idx++) {
            const line = arr[idx];
            const patternInput = lineToHtml(grammar, colorMap, prevStack, line);
            html = html + patternInput[0] + "\n";
            prevStack = patternInput[1];
          }

          return html;
        }).then(function (htmlCode) {
          const preStyle = (0, _String.join)(";", (0, _List.filter)($var2 => !(0, _String.isNullOrEmpty)($var2), (0, _List.ofArray)([toInlineStyle("background-color", config.backgroundColor), toInlineStyle("color", config.textColor), "padding: 1em"])));
          const openPreTag = preStyle !== "" ? "<pre style=\"" + preStyle + "\">" : "<pre>";
          const codeStyle = config.codeElementClass == null ? "" : "style= \"" + (0, _Option.getValue)(config.codeElementClass) + "\"";
          return openPreTag + "<code" + codeStyle + ">" + (0, _String.trim)(htmlCode, "both") + "</code></pre>";
        });
      });
    });
  }(_Promise.PromiseImpl.promise).then(void 0, function (error) {
    console.error(error);
    return "";
  });
}