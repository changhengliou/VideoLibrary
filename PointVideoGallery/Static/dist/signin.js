webpackJsonp([2],{

/***/ 40:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
Object.defineProperty(__webpack_exports__, "__esModule", { value: true });
/* WEBPACK VAR INJECTION */(function($) {/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__css_signin_css__ = __webpack_require__(41);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__css_signin_css___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_0__css_signin_css__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__js_utils__ = __webpack_require__(42);



document.getElementById('submitBtn').addEventListener('click', function (e) {
    e.preventDefault();
    var uid = document.forms.signin.uid.value,
        pwd = document.forms.signin.pwd.value;

    if (Object(__WEBPACK_IMPORTED_MODULE_1__js_utils__["a" /* isEmpty */])(uid) || Object(__WEBPACK_IMPORTED_MODULE_1__js_utils__["a" /* isEmpty */])(pwd)) return;

    $.ajax({
        url: '',
        method: 'POST',
        data: {
            uid: document.forms.signin.uid.value,
            pwd: document.forms.signin.pwd.value
        }
    }).done(function (msg) {
        return console.log(msg);
    }).fail(function (err) {
        return console.log(err);
    });
});
/* WEBPACK VAR INJECTION */}.call(__webpack_exports__, __webpack_require__(0)))

/***/ }),

/***/ 41:
/***/ (function(module, exports) {

// removed by extract-text-webpack-plugin

/***/ }),

/***/ 42:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "a", function() { return isEmpty; });
var isEmpty = function isEmpty(str) {
  return !str || /^\s*$/.test(str);
};

/***/ })

},[40]);
//# sourceMappingURL=signin.js.map