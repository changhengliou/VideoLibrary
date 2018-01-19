import 'bootstrap-table';
import 'bootstrap-table/dist/bootstrap-table.css';
import { setTableViewZhTwLocal } from './js/utils';

setTableViewZhTwLocal($);
$.editable = false;
/**
 * @param {bool} editable application state, if true, some row is in a editing state
 */
const setEditable = (editable) => {
    $.editable = editable;
    document.getElementById('newRowBtn').disabled = $.editable ? 'disabled' : '';
}


$('#table').bootstrapTable({
    url: '/api/v1/ad/location',
    uniqueId: 'Id',
    onClickCell: (field, value, row, element) => {
        console.log(row);
        if (row.Id === -1)
            return;
        if (field !== 'Name')
            return;
        if (!row || $.editable)
            return;
        setRowEditable(element, row, value);
    },
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
        field: 'Name',
        title: '名稱'
    }]
});

/**
 * when create row is clicked, create a new form. Rollback if failed to update.
 * @param {event} e click event 
 */
const onCreateRowClick = (e) => {
    if ($.editable)
        return;

    var table = $('#table');
    table.bootstrapTable('selectPage', 1);
    table.bootstrapTable('prepend', {
        Id: -1,
        Name: '',
    });
    table.find('tr[data-uniqueid=-1] > td').html(
        `<div>
            <input class="form-control input-sm" style="display: inline-block; width:50vw;" placeholder="名稱" type="text" id="updateVal"/>
            <button class="btn btn-sm btn-primary" name="createrow" id="createbtn">儲存</button>
            <button class="btn btn-sm btn-default" name="abortcreate" id="abortbtn">取消</button>
        </div>`);
    document.getElementById('createbtn').addEventListener('click', onCreateRowCellClick, false);
    document.getElementById('abortbtn').addEventListener('click', onCreateRowCellClick, false);
    setEditable(true);
}

// register event listener to create row button
document.getElementById('newRowBtn').addEventListener('click', onCreateRowClick);    


/**
 * when user click button in new row cell
 * @param {event} e 
 */
const onCreateRowCellClick = (e) => {
    /**
     * 
     * @param {HtmlElement} element button in the new row cell
     * @param {jQueryObject} table bootstrap table
     * @param {HtmlElement} button creat new row button
     */
    const removeEditRow = (element, table, button) => {
        element.removeEventListener('click', onCreateRowCellClick);
        table.bootstrapTable('removeByUniqueId', -1);
        setEditable(false);
    }

    var name = e.target.name,
        _fn = $('#table'),
        btn = document.getElementById('newRowBtn'),
        data = document.getElementById('updateVal');

    if (name === 'abortcreate') {
        removeEditRow(e.target, _fn, btn);
    } else if (name === 'createrow') {
        if (!data || !data.value) {
            toggleSystemMsg('名稱空白!');
            return;
        }

        $.ajax({
            url: `/api/v1/ad/location/${data.value}/add`,
            method: 'POST'
        })
        .done(res => {
            console.log(res);
            toggleSystemMsg('新增成功!');
            removeEditRow(e.target, _fn, btn);
            _fn.bootstrapTable('refresh');
        })
        .fail(err => {
            console.log(err);
            toggleSystemMsg('新增失敗!');
        })
    }
}

/**
 * @param {jqueryElement} e cell object
 * @param {object} row row data
 * @param {string} data cell data  
 */
const setRowEditable = (e, row, value) => {
    if ($.editable)
        return;
    e.html(`<div>
                <input class="form-control input-sm" style="display: inline-block; width:50vw;" placeholder="名稱" type="text" id="name_${row.Id}" value="${value}"/>
                <button class="btn btn-sm btn-primary" name="update" id="updatebtn_${row.Id}" data-id="${row.Id}">更新</button>
                <button class="btn btn-sm btn-default" name="abort" id="abortbtn_${row.Id}" data-id="${row.Id}">取消</button>
                <button class="btn btn-sm btn-danger" name="delete" id="deletebtn_${row.Id}" data-id="${row.Id}">刪除</button>
            </div>`);
    document.getElementById(`name_${row.Id}`).focus();
    document.getElementById(`updatebtn_${row.Id}`).addEventListener('click', onRowCellBtnClick);
    document.getElementById(`abortbtn_${row.Id}`).addEventListener('click', onRowCellBtnClick);
    document.getElementById(`deletebtn_${row.Id}`).addEventListener('click', onRowCellBtnClick);
    setEditable(true);
}
/**
 * when update click, do ajax update, if suceess, update row data, otherwise, rollback
 * @param {event} e click event
 */
const onRowCellBtnClick = (e) => {
    /**
     * perform rollback if update failed or abort
     * @param {jqueryObject} table bootstrap table
     * @param {string} id table row id
     */
    const rollback = (table, rowId) => {
        table.bootstrapTable('updateByUniqueId', {
            id: rowId,
            row: {
            }
        });
        setEditable(false);
    }
    // _id: rowId, _fn: bootstrapTable, _value: input value, btnName: click button
    var _id = e.target.getAttribute('data-id'),
        _fn = $('#table'),
        _value = document.getElementById(`name_${_id}`).value,
        btnName = e.target.name;

    e.target.removeEventListener('click', onRowCellBtnClick);
    
    if (btnName === 'abort') {
        rollback(_fn, _id);
        return;
    }

    if (btnName === 'delete') {
        $.ajax({
            url: `/api/v1/ad/location/${_id}`,
            method: 'DELETE',
        })
        .done(res => {
            _fn.bootstrapTable('removeByUniqueId', _id);
            toggleSystemMsg('刪除成功!');
            setEditable(false);
        })
        .fail(err => {
            rollback(_fn, _id);
            console.log(err);
            toggleSystemMsg('刪除失敗!');
        });
        return;
    }

    $.ajax({
        url: '/api/v1/ad/location/update',
        method: 'PUT',
        data: {
            id: _id,
            name: _value
        }
    })
    .done(res => {
        _fn.bootstrapTable('updateByUniqueId', {
            id: _id,
            row: { 
                Name: _value,
            }
        });
        toggleSystemMsg('更新成功!');
        console.log(res)
        setEditable(false);
    })
    .fail(err => {
        rollback(_fn, _id);
        toggleSystemMsg('更新失敗!');
        console.log(err)
    });
}

/**
 * show system message if param is given, otherwise hide the message box
 * @param {string} msg system message
 */
const toggleSystemMsg = (msg) => {
    var msgBox = document.getElementById('msg');
    if (!msgBox || !msg.trim().length) {
        msgBox.style.display = 'none';
        return;
    }
    msgBox.innerHTML = `<span style='color:red; font-weight: 700;'>${msg}</span>`;
    msgBox.style.display = '';
}