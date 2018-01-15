webpackHotUpdate(1,{

/***/ 37:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
Object.defineProperty(__webpack_exports__, "__esModule", { value: true });
/* WEBPACK VAR INJECTION */(function(jQuery) {/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0_bootstrap_table__ = __webpack_require__(38);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0_bootstrap_table___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_0_bootstrap_table__);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_bootstrap_table_dist_bootstrap_table_css__ = __webpack_require__(39);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1_bootstrap_table_dist_bootstrap_table_css___default = __webpack_require__.n(__WEBPACK_IMPORTED_MODULE_1_bootstrap_table_dist_bootstrap_table_css__);



(function ($) {
    $.extend($.fn.bootstrapTable.defaults, {
        formatLoadingMessage: function formatLoadingMessage() {
            return '資料載入中，請稍候……';
        },
        formatRecordsPerPage: function formatRecordsPerPage(pageNumber) {
            return '每頁顯示 ' + pageNumber + ' 筆資料';
        },
        formatShowingRows: function formatShowingRows(pageFrom, pageTo, totalRows) {
            return '第 ' + pageFrom + ' 到第 ' + pageTo + ' 項記錄，總共 ' + totalRows + ' 項記錄';
        },
        formatSearch: function formatSearch() {
            return '搜尋';
        },
        formatNoMatches: function formatNoMatches() {
            return '沒有結果';
        },
        formatPaginationSwitch: function formatPaginationSwitch() {
            return '隱藏/顯示分頁';
        },
        formatRefresh: function formatRefresh() {
            return '重新整理';
        },
        formatToggle: function formatToggle() {
            return '切換';
        },
        formatColumns: function formatColumns() {
            return '列';
        }
    });

    $('#resource-table').bootstrapTable({
        locale: 'zh-TW',
        striped: true,
        pagination: true,
        sortName: ['id'],
        columns: [{
            field: 'id',
            title: 'Item ID'
        }, {
            field: 'name',
            title: 'Item Name'
        }, {
            field: 'price',
            title: 'Price'
        }],
        data: [{
            id: 1,
            name: 'Item 1',
            price: '$1'
        }, {
            id: 2,
            name: 'Item 2',
            price: '$2'
        }, {
            id: 3,
            name: 'Item 3',
            price: '$3'
        }]
    });
})(jQuery);
/* WEBPACK VAR INJECTION */}.call(__webpack_exports__, __webpack_require__(0)))

/***/ })

})
//# sourceMappingURL=1.9f8b3bcceb70ae9e6862.hot-update.js.map