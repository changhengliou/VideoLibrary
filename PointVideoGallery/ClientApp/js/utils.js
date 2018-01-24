export const isEmpty = str => !str || /^\s*$/.test(str);

export const getDateString = (date) => {
    if (!date)
        date = new Date();
    else if (typeof date === 'string')
        date = new Date(date);

    var mm = date.getMonth() + 1,
        dd = date.getDate();

    return [date.getFullYear(), (mm > 9 ? '' : '0') + mm, (dd > 9 ? '' : '0') + dd].join('-');
}

export const getTimeString = (time) => {
    if (!time)
        time = new Date();
    else if (typeof date === 'string')
        time = new Date(time);

    var h = time.getHours();
    var m = time.getMinutes();
    var s = time.getSeconds();
    return [(h > 9 ? '' : '0') + h, (m > 9 ? '' : '0') + m, (s > 9 ? '' : '0') + s].join(':');
}

export const getDateTimeString = (dt) => getDateString(dt) + " " + getTimeString(dt);

export const setTableViewZhTwLocal = ($) => {
    $.extend($.fn.bootstrapTable.defaults, {
        formatLoadingMessage: () => '資料載入中，請稍候……',
        formatRecordsPerPage: (pageNumber) => '每頁顯示 ' + pageNumber + ' 筆資料',
        formatShowingRows: (pageFrom, pageTo, totalRows) => '第 ' + pageFrom + ' 到第 ' + pageTo + ' 項記錄，總共 ' + totalRows + ' 項記錄',
        formatSearch: () => '搜尋名稱',
        formatNoMatches: () => '沒有結果',
        formatPaginationSwitch: () => '隱藏/顯示分頁',
        formatRefresh: () => '重新整理',
        formatToggle: () => '切換',
        formatColumns: () => '列'
    });
}

export const tableSetting = {
    uniqueId: 'Id',
    iconSize: 'sm',
    locale: 'zh-TW',
    striped: true,
    pagination: true,
    pageNumber: 1,
    pageSize: 10,
    pageList: [10, 25, 50],
    search: false,
    showHeader: true,
    showFooter: false,
    showRefresh: true,
    showToggle: false, //switch between cardView / detailView
    showPaginationSwitch: false, // show/hide pagination
    cardView: false, // if true, switch to card view
    detailView: false, // if true, show plus sign with detail enabled
    rowStyle: (row, index) => { return { css: { "vertical-align": "middle" } } },
}

export const intersection = (a, b) => {
    var ai = 0, bi = 0;
    var result = [];

    while (ai < a.length && bi < b.length) {
        if (a[ai] < b[bi]) { ai++; }
        else if (a[ai] > b[bi]) { bi++; }
        else {
            result.push(a[ai]);
            ai++;
            bi++;
        }
    }

    return result;
}