import 'bootstrap-table';
import 'bootstrap-table/dist/bootstrap-table.css';
import { setTableViewZhTwLocal, isEmpty } from './js/utils';

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
    var table = $('#table');

    table.bootstrapTable({
        onClickCell: (field, value, row, element) => {
            if (field !== 'Edit')
                return;
            onEditClick(row);
        },
        url: '/api/v1/setting/so',
        uniqueId: 'Id',
        iconSize: 'sm',
        locale: 'zh-TW',
        striped: true,
        pagination: true,
        pageNumber: 1,
        pageSize: 10,
        pageList: [10, 25, 50],
        search: true,
        showHeader: true,
        showFooter: false,
        showRefresh: true,
        showToggle: false, //switch between cardView / detailView
        showPaginationSwitch: false, // show/hide pagination
        cardView: false, // if true, switch to card view
        detailView: false, // if true, show plus sign with detail enabled
        rowStyle: (row, index) => { return { css: { "vertical-align": "middle" } } },
        columns: [{
            field: 'Code',
            title: '代碼'
        }, {
            field: 'Name',
            title: '名稱'
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
        name = document.getElementById('editName'),
        msgEle = document.getElementById('editModalMsg');

    if (!formValidation(code.value, name.value, msgEle))
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
        msgEle.innerHTML = '新增失敗';
        msgEle.style.display = '';
    });
}

const onDeleteRow = (e) => {
    const id = e.target.getAttribute('data-id');
    var code = document.getElementById('editCode'),
        name = document.getElementById('editName'),
        msgEle = document.getElementById('editModalMsg');

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
        msgEle.innerHTML = '';
        msgEle.style.display = 'none';
    })
    .fail(err => {
        msgEle.innerHTML = '刪除失敗';
        msgEle.style.display = '';
    });
}

const onAbortUpdateRow = (e) => {
    const msgEle = document.getElementById('editModalMsg');

    document.getElementById('editCode').value = '';
    document.getElementById('editName').value = '';
    msgEle.innerHTML = '';
    msgEle.style.display = 'none';
    $('#neditModal').modal('toggle');
}

const onNewRowClick = (e) => $('#newModal').modal('toggle');

const onCreateNewRow = (e) => {
    var code = document.getElementById('newCode'),
        name = document.getElementById('newName'),
        msgEle = document.getElementById('newModalMsg');

    if (!formValidation(code.value, name.value, msgEle))
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
        msgEle.innerHTML = '新增失敗';
        msgEle.style.display = '';
    });
}

const onAbortCreatNewRow = (e) => {
    const msgEle = document.getElementById('newModalMsg');

    document.getElementById('newCode').value = '';
    document.getElementById('newName').value = '';
    msgEle.innerHTML = '';
    msgEle.style.display = 'none';
    $('#newModal').modal('toggle');
}

/**
 * modal form validation
 * @param {string} code 
 * @param {string} name 
 * @param {HTMLElement} ele
 */
const formValidation = (code, name, ele) => {
    if (isEmpty(code) || isEmpty(name)) {
        ele.innerHTML = '欄位不能為空';
        ele.style.display = '';
        return false;
    }
    ele.innerHTML = '';
    ele.style.display = 'none';
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