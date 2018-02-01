import 'bootstrap-table';
import 'bootstrap-table/dist/bootstrap-table.css';
import './css/account.css';
import { isEmpty, addMsgbox, tableSetting, setTableViewZhTwLocal } from './js/utils';

setTableViewZhTwLocal($);

$('#table').bootstrapTable({
    ...tableSetting,
    url: '/api/v1/account/users',
    columns: [{
        field: 'UserName',
        title: '名稱'
    }, {
        field: 'Edit',
        title: '編輯',
        formatter: (value, row, index, field) => 
            `<button class='btn btn-sm btn-default' data-id="${row.Id}" onClick='$.fn.getUser(this);'>
                <span class="glyphicon glyphicon-pencil"></span>
            </button>`
    }, {
        field: 'Enable',
        title: '啟用/關閉帳號',
        formatter: (value, row, index, field) =>
            `<label class="switch">
                <input type="checkbox" ${value ? "checked='checked'" : null} data-id="${row.Id}">
                <span class="slider round"></span>
            </label>`
    }]
});

$.fn.getUser = (e) => {

}

$.fn.rmu = (e) => {
    var id = e.getAttribute('data-id');
    if (isEmpty(id)) {
        addMsgbox('系統發生錯誤!', '請嘗試重新整理','data-panel', 'danger');
    }
    $.ajax({
        method: 'POST',
        url: `/api/v1/account/user/status`,
        data: {
            id: id
        }
    })
    .done(res => {
        addMsgbox('成功關閉帳號!', null,'data-panel', 'success');
    })
    .fail(err => {
        addMsgbox('關閉帳號失敗!', null,'data-panel', 'danger');
    });
}