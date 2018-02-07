import 'bootstrap-table';
import 'bootstrap-table/dist/bootstrap-table.css';
import { setTableViewZhTwLocal, isEmpty, tableSetting, addMsgbox } from './js/utils';

// edit button click
const onEditClick = (row) => {
    var code = document.getElementById('editCode'),
        name = document.getElementById('editName');
    document.getElementById('editUpdate').setAttribute('data-id', row.Id);
    document.getElementById('editDelete').setAttribute('data-id', row.Id);
    code.value = row.Code;
    name.value = row.Name;
}

$(document).ready(() => {
    setTableViewZhTwLocal($);
    var table = $('#table');

    table.bootstrapTable({
        ...tableSetting,
        search: true,
        onClickCell: (field, value, row, element) => {
            if (field !== 'Edit')
                return;
            onEditClick(row);
        },
        url: '/api/v1/setting/so',
        columns: [{
            field: 'Code',
            title: '代碼',
            sortable: true
        }, {
            field: 'Name',
            title: '名稱',
            sortable: true
        }, {
            field: 'Edit',
            title: '編輯',
            formatter: (value, row, index, field) => `<button class='btn btn-sm btn-default' data-id="${row.Id}" data-toggle="modal" data-target="#editModal">
                                                          <span class="glyphicon glyphicon-pencil"></span>
                                                      </button>`
        }]
    });   
});

const onUpdateRow = (e) => {
    var code = document.getElementById('editCode'),
        name = document.getElementById('editName');

    if (!formValidation(code.value, name.value))
        return;
    
    $.ajax({
        url: '/api/v1/setting/so/',
        method: 'PUT',
        data: { name: name.value, code: code.value, id: e.target.getAttribute('data-id') }
    })
    .done(res => {
        $('#table').bootstrapTable('refresh');
        $('#editModal').modal('toggle');
        code.value = '';
        name.value = '';
    })
    .fail(err => {
        addMsgbox(err.status === 403 ? '權限不足!' : '新增失敗!', null, 'panel', 'danger');
    });
}

const onDeleteRow = (e) => {
    const id = e.target.getAttribute('data-id');
    var code = document.getElementById('editCode'),
        name = document.getElementById('editName');

    $.ajax({
        url: `/api/v1/setting/so/${id}`,
        method: 'DELETE',
        data: { name: name.value, code: code.value }
    })
    .done(res => {
        $('#table').bootstrapTable('refresh');
        $('#editModal').modal('toggle');
        code.value = '';
        name.value = '';
    })
    .fail(err => {
        addMsgbox(err.status === 403 ? '權限不足!' : '刪除失敗!', null, 'panel', 'danger');
    });
}

const onAbortUpdateRow = (e) => {
    document.getElementById('editCode').value = '';
    document.getElementById('editName').value = '';
    $('#neditModal').modal('toggle');
}

const onNewRowClick = (e) => $('#newModal').modal('toggle');

const onCreateNewRow = (e) => {
    var code = document.getElementById('newCode'),
        name = document.getElementById('newName');

    if (!formValidation(code.value, name.value))
        return;

    $.ajax({
        url: '/api/v1/setting/so/',
        method: 'POST',
        data: { name: name.value, code: code.value }
    })
    .done(res => {
        $('#table').bootstrapTable('refresh');
        $('#newModal').modal('toggle');
        code.value = '';
        name.value = '';
    })
    .fail(err => {
        addMsgbox(err.status === 403 ? '權限不足!' : '新增失敗!', null, 'panel', 'danger');
    });
}

const onAbortCreatNewRow = (e) => {
    document.getElementById('newCode').value = '';
    document.getElementById('newName').value = '';
    $('#newModal').modal('toggle');
}

/**
 * modal form validation
 * @param {string} code 
 * @param {string} name 
 * @param {HTMLElement} ele
 */
const formValidation = (code, name) => {
    if (isEmpty(code) || isEmpty(name)) {
        addMsgbox('欄位不能為空!', null, 'panel', 'danger');
        return false;
    }
    return true;
}

const handler = {
    newRowBtn: onNewRowClick,
    newCreate: onCreateNewRow,
    newAbort: onAbortCreatNewRow,
    editUpdate: onUpdateRow,
    editDelete: onDeleteRow,
    editAbort: onAbortUpdateRow
}

Object.keys(handler).map(id => document.getElementById(id).addEventListener('click', handler[id]));