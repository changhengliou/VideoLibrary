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
            `<div>
                <button class='btn btn-sm btn-default' data-id="${row.Id}" onClick='$.fn.getUser(this);'>
                    <span class="glyphicon glyphicon-pencil"></span>
                </button>
                <button class='btn btn-sm btn-default' data-id="${row.Id}" onClick='$.fn.delu(this);'>
                    <span class="glyphicon glyphicon-remove"></span>
                </button>
            </div>`
    }, {
        field: 'Enable',
        title: '啟用/關閉帳號',
        formatter: (value, row, index, field) =>
            `<label class="switch">
                <input type="checkbox" ${value ? "checked='checked'" : null} data-id="${row.Id}" id='check-${row.Id}' onchange='$.fn.rmu(this);'>
                <span class="slider round"></span>
            </label>`
    }]
});

$.fn.getUser = (e) => {
    var id = e.getAttribute('data-id');
    if (isEmpty(id)) {
        addMsgbox('系統發生錯誤!', '請嘗試重新整理','data-panel', 'danger');
    }
    $.ajax({
        url: `/api/v1/account/${id}/user`,
        method: 'GET',
    })
    .done(res => {
        document.getElementById('editEmail').value = res.Email;
        document.getElementById('editId').value = res.UserName;
        document.getElementById('editPwd').value = atob(res.Password);
        $('#editModal').modal('toggle');
        document.getElementById('editSave').setAttribute('data-id', id);
    })
    .fail(err => {
        addMsgbox('無法取得資料!', null,'data-panel', 'danger');
    });
}

$.fn.rmu = (e) => {
    var id = e.getAttribute('data-id'),
        enable = $(`#check-${id}`).prop('checked'),
        msg = enable ? '成功開啟帳號!' : '成功關閉帳號!',
        errMsg = enable ? '開啟帳號失敗' : '關閉帳號失敗';
    if (isEmpty(id)) {
        addMsgbox('系統發生錯誤!', '請嘗試重新整理','data-panel', 'danger');
    }
    $.ajax({
        method: 'POST',
        url: `/api/v1/account/user/status`,
        data: {
            id: id,
            enable: enable ? 1 : 0
        }
    })
    .done(res => {
        addMsgbox(msg, null,'data-panel', 'success');
    })
    .fail(err => {
        addMsgbox(errMsg, null,'data-panel', 'danger');
    });
}

$.fn.delu = (e) => {
    $('#removeModal').modal('toggle');
    document.getElementById('removeConfirm').setAttribute('data-id', e.getAttribute('data-id'));
}

const updateUser = (e) => {
    var id = e.target.getAttribute('data-id'),
        url = '/api/v1/account/user',
        method = 'PUT',
        enable;

    if (isEmpty(id)) {
        url = '/api/v1/account/user/new';
        method = 'POST';
        enable = 1;
    } else {
        enable = $(`#check-${id}`).prop('checked') ? 1 : 0;
    }

    $.ajax({
        method: method,
        url: url,
        data: {
            id: id,
            Email: document.getElementById('editEmail').value,
            UserName: document.getElementById('editId').value,
            Password: btoa(document.getElementById('editPwd').value),
            enable: enable
        }
    })
    .done(res => {
        addMsgbox('成功儲存!', null,'data-panel', 'success');
        $('#table').bootstrapTable('refresh');
    })
    .fail(err => {
        addMsgbox('儲存失敗!', null,'data-panel', 'danger');
    });
    $('#editModal').modal('toggle');
}

const onCreateUserModal = (e) => {
    $('#editModal').modal('toggle');
    document.getElementById('editEmail').value = '';
    document.getElementById('editId').value = '';
    document.getElementById('editPwd').value = '';
    document.getElementById('editSave').removeAttribute('data-id');
};

const removeUser = (e) => {
    $.ajax({
        url: `/api/v1/account/drop/user/${e.target.getAttribute('data-id')}`,
        method: 'DELETE'
    })
    .done(res => {
        addMsgbox('刪除帳號成功!', null,'data-panel', 'success');
        $('#table').bootstrapTable('refresh');
    })
    .fail(err => {
        addMsgbox('刪除帳號失敗!', null,'data-panel', 'danger');
    });
    $('#removeModal').modal('toggle');
}

document.getElementById('editSave').addEventListener('click', updateUser);
document.getElementById('newRowBtn').addEventListener('click', onCreateUserModal);
document.getElementById('removeConfirm').addEventListener('click', removeUser);