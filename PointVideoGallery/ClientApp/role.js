import 'bootstrap-table';
import { isEmpty, addMsgbox, tableSetting, setTableViewZhTwLocal } from './js/utils';

setTableViewZhTwLocal($);

const renderCell = (value, row, index, field, attr) =>
    `
        <input class='radio' 
               type="radio" 
               ${value === 'r' ? "checked='checked'" : null} 
               data-id="${row.Id}" 
               data-t='r' 
               onchange='$.fn.statusChange(this);' 
               name='${attr}-${row.Id}'
               id='r-${row.Id}-${attr}'>
        <label for="r-${row.Id}-${attr}">讀</label>
        <input class='radio' 
               type="radio" 
               ${value === 'rw' ? "checked='checked'" : null} 
               data-id="${row.Id}" 
               data-t='rw' 
               onchange='$.fn.statusChange(this);' 
               name='${attr}-${row.Id}'
               id="rw-${row.Id}-${attr}">
        <label for="rw-${row.Id}-${attr}">讀+寫</label>
        <input class='radio' 
               type="radio" 
               ${value === 'x' ? "checked='checked'" : null} 
               data-id="${row.Id}" 
               data-t='x' 
               onchange='$.fn.statusChange(this);' 
               name='${attr}-${row.Id}'
               id="x-${row.Id}-${attr}">
        <label for="x-${row.Id}-${attr}">無權限</label>
    `;

$('#table').bootstrapTable({
    ...tableSetting,
    showRefresh: false,
    url: '/api/v1/account/users/roles',
    columns: [{
        field: 'UserName',
        title: '名稱'
    }, {
        field: 'EnablePublish',
        title: '廣告發布',
        formatter: (value, row, index, field) => renderCell(value, row, index, field, 'a')
    }, {
        field: 'EnableResource',
        title: '資源維護',
        formatter: (value, row, index, field) => renderCell(value, row, index, field, 'b')
    }, {
        field: 'EnableLocation',
        title: '廣告位置',
        formatter: (value, row, index, field) => renderCell(value, row, index, field, 'c')
    }, {
        field: 'EnableSo',
        title: 'SO管理',
        formatter: (value, row, index, field) => renderCell(value, row, index, field, 'd')
    }, {
        field: 'EnableEvent',
        title: '廣告活動',
        formatter: (value, row, index, field) => renderCell(value, row, index, field, 'e')
    }, {
        field: 'EnableAdmin',
        title: '<span style="color:red;">*</span>管理員',
        formatter: (value, row, index, field) => 
            `<label class="switch">
                <input type="checkbox" ${value ? "checked='checked'" : null} data-id="${row.Id}" id='ad-${row.Id}' onchange='$.fn.statusChange(this);'>
                <span class="slider round"></span>
            </label>`
    }]
});

const getRadioBtnVal = (type, id) => {
    var rtnVal;
    $(`input[name=${type}-${id}]`).each((i, e) => {
        if ($(e).prop('checked'))
            rtnVal = e.getAttribute('data-t');
    });
    return rtnVal;
}
$.fn.statusChange = (e) => {
    var id = e.getAttribute('data-id'),
        val = $(e).prop('checked');
    if (isEmpty(id)) {
        addMsgbox('系統錯誤!', '請嘗試重新整理', 'data-panel', 'danger');
        return;
    }
    $.ajax({
        url: '/api/v1/account/users/roles',
        method: 'PUT',
        data: {
            Id: id,
            EnableResource: getRadioBtnVal('b', id),
            EnablePublish: getRadioBtnVal('a', id),
            EnableLocation: getRadioBtnVal('c', id),
            EnableSo: getRadioBtnVal('d', id),
            EnableEvent: getRadioBtnVal('e', id),
            EnableAdmin: $(`#ad-${id}`).prop('checked') ? 1 : 0
        }
    })
    .done(res => {

    })
    .fail(err => {
        addMsgbox('更新失敗!', null, 'data-panel', 'danger');
    });
};
