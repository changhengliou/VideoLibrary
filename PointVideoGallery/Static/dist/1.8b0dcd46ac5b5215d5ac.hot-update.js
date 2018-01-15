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
        pageNumber: 1,
        pageSize: 2,
        pageList: [2, 10, 25, 50, 100],
        search: true,
        searchOnEnterKey: true, // search on enter press
        showHeader: true,
        showFooter: false,
        showRefresh: true,
        showToggle: false, //switch between cardView / detailView
        showPaginationSwitch: false, // show/hide pagination
        cardView: false, // if true, switch to card view
        detailView: false, // if true, show plus sign with detail enabled
        columns: [{
            field: 'id',
            title: 'Item ID',
            sortable: true
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
//# sourceMappingURL=1.8b0dcd46ac5b5215d5ac.hot-update.js.map