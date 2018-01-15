import 'bootstrap-table';
import 'bootstrap-table/dist/bootstrap-table.css';

(function ($) {
    $.extend($.fn.bootstrapTable.defaults, {
        formatLoadingMessage: () => '資料載入中，請稍候……',
        formatRecordsPerPage: (pageNumber) => '每頁顯示 ' + pageNumber + ' 筆資料',
        formatShowingRows: (pageFrom, pageTo, totalRows) => '第 ' + pageFrom + ' 到第 ' + pageTo + ' 項記錄，總共 ' + totalRows + ' 項記錄',
        formatSearch: () => '搜尋',
        formatNoMatches: () => '沒有結果',
        formatPaginationSwitch: () => '隱藏/顯示分頁',
        formatRefresh: () => '重新整理',
        formatToggle: () => '切換',
        formatColumns: () => '列'
    });

    $('#resource-table').bootstrapTable({
        iconSize: 'sm',
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
        rowStyle: (row, index) => { return { css: { "vertical-align": "middle" } } },
        columns: [{
            field: 'type',
            title: '類型',
            sortable: true
        }, {
            field: 'name',
            title: '名稱',
            sortable: true
        }, {
            field: 'edit',
            title: '編輯',
            formatter: (value, row, index, field) => `<button class='btn btn-sm btn-default'>
                <span class="glyphicon glyphicon-pencil"></span>
            </button>`
        }],
        data: [{
            type: 1,
            name: 'Item 1',
        }, {
            type: 2,
            name: 'Item 2',
        }, {
            type: 3,
            name: 'Item 3',
        }]
    });
})(jQuery);
