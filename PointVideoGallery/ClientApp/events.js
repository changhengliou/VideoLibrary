import 'bootstrap-table';
import 'bootstrap-table/dist/bootstrap-table.css';
import 'select2';
import 'select2/dist/css/select2.css';
import './css/events.css';
import { setTableViewZhTwLocal, isEmpty, tableSetting, getDateTimeString, swap } from './js/utils';


const disableBy = fn => fn ? 'disable="disabled"' : null;
/**
 * reload select2 data
 */
(function ($) {
    $.fn.reloadSelect2 = function (data) {
        this.select2('data', data);
        var $select = $(this[0]);
        var options = data.map(function(item) {
            return '<option value="' + item.id + '">' + item.text + '</option>';
        });
        $select.html(options.join('')).change();
    };
})(jQuery);

/**
 * map data to select2 format
 * @param {Array} arr 
 */
const mapSelectData = (arr) => arr.map(obj => {
        obj.id = obj.id || obj.Id;
        obj.text = obj.text || obj.Name;
        return obj;
    });

/**
 * Given 2 array with old data and new data, return the difference
 * @param {Array} oldArr 
 * @param {Array} newArr 
 * @returns {Object} @param {Array} add data sholud be add, @param {Array} rm data should be remove
 */
const getDiff = (oldArr, newArr) => {
    const add = [], rm = [];
    var oi = 0, ni = 0;
    oldArr = oldArr.sort((x,y) => x > y);
    newArr = newArr.sort((x,y) => x > y);
    while (oi < oldArr.length && ni < newArr.length) {
        if (oldArr[oi] == newArr[ni]) {
            oi++;
            ni++;
        } else if (oldArr[oi] > newArr[ni]) {
            add.push(newArr[ni]);
            ni++;
        } else {
            rm.push(oldArr[oi]);
            oi++;
        }
    }
    for (; oi < oldArr.length; ++oi)
        rm.push(oldArr[oi]);
    for (; ni < newArr.length; ++ni)
        add.push(newArr[ni]);
    return { add: add, rm: rm };
}

$(document).ready(() => {
    var table = $('#table'),
        locTable = $('#location-table'),
        soTable = $('#so-table'),
        resTable = $('#resource-table');

    // set i18n to zh-tw
    setTableViewZhTwLocal($);
    // event list table
    table.bootstrapTable({
        ...tableSetting,
        onClickCell: (field, value, row, element) => {
            if (field !== 'Edit')
                return;
        },
        url: '/api/v1/ad/events/',
        columns: [{
            field: 'Name',
            title: '名稱'
        }, {
            field: 'Edit',
            title: '編輯',
            formatter: (value, row, index, field) => `<button class='btn btn-sm btn-default' data-id="${row.Id}" onClick='$.fn.switchView(this);'>
                                                          <span class="glyphicon glyphicon-pencil"></span>
                                                      </button>`
        }]
    });   
    locTable.bootstrapTable({
        ...tableSetting,
        showRefresh: false,
        columns: [{
            field: 'Name',
            title: '名稱'
        }, {
            field: 'Remove',
            title: '刪除',
            formatter: (value, row, index, field) => `<button class='btn btn-sm btn-default' data-id="${row.Id}" data-t='loc' onClick='$.fn.remove(this);'>
                                                          <span class="glyphicon glyphicon-remove"></span>
                                                      </button>`
        }]
    });
    soTable.bootstrapTable({
        ...tableSetting,
        showRefresh: false,
        columns: [{
            field: 'Code',
            title: '代碼'
        }, {
            field: 'Name',
            title: '名稱'
        }, {
            field: 'Remove',
            title: '刪除',
            formatter: (value, row, index, field) => `<button class='btn btn-sm btn-default' data-id="${row.Id}" data-t='so' onClick='$.fn.remove(this);'>
                                                          <span class="glyphicon glyphicon-remove"></span>
                                                      </button>`
        }]
    });
    resTable.bootstrapTable({
        ...tableSetting,
        onClickCell: (field, value, row, element) => {
            if (field !== 'PlayoutWeight')
                return;
            onEditClick(element, row, value);
        },
        showRefresh: false,
        columns: [{
            field: 'ThumbnailPath',
            title: '預覽',
            formatter: (value, row, index, field) => 
                       `<div class='preview-img table-thumbnail' style='background: url("/assets?p=${value}") no-repeat;'/>` 
        }, {
            field: 'MediaType',
            title: '類型',
        }, {
            field: 'Name',
            title: '名稱',
        }, {
            field: 'CreateTime',
            title: '創建日期',
            formatter: (value) => getDateTimeString(new Date(value))
        }, {
            field: 'Action',
            title: '動作',
            formatter: (value, row, index, field) => null
        }, {
            field: 'PlayoutWeight',
            title: '比重',
            formatter: (value, row, index, field) => value > 0 ? value : null
        }, {
            field: 'Sort/Action/Remove',
            title: '排序/動作/刪除',
            formatter: (value, row, index, field) => 
                `<div>
                    <button class='btn btn-sm btn-default' data-id="${row.Id}" data-t='up' onClick='$.fn.moveSeq(this);'>
                        <span class="glyphicon glyphicon-arrow-up"></span>
                    </button>
                    <button class='btn btn-sm btn-default' data-id="${row.Id}" data-t='down' onClick='$.fn.moveSeq(this);'>
                        <span class="glyphicon glyphicon-arrow-down"></span>
                    </button>
                    <button class='btn btn-sm btn-default' data-id="${row.Id}" onClick='$.fn.moveSeq(this);'>
                        <span class="glyphicon glyphicon-tag"></span>
                    </button>
                    <button class='btn btn-sm btn-default' data-id="${row.Id}" data-t='res' onClick='$.fn.remove(this);'>
                        <span class="glyphicon glyphicon-remove"></span>
                    </button>
                </div>`
        }]
    })

    // initialize query select box
    $('#locationSelect').select2({
        width: window.innerWidth > 700 ? '60%' : '90%',
        data: mapSelectData($._loc)
    });
    $('#soSelect').select2({
        width: window.innerWidth > 700 ? '60%' : '90%',
        data: mapSelectData($._so)
    });
    // initialize select box in edit modal
    $('#editSelect').select2({
        width: '100%'
    });

    // hide playoutweight by default
    $('#resource-table').bootstrapTable('hideColumn', 'PlayoutWeight');
    /**
     * Get detail data from server
     * @param {number} id request data id  
     */
    const requestData = (id) => {
        $.ajax({
            method: 'GET',
            url: `/api/v1/ad/events/${id}`
        })
        .done(res => {
            $._res = res;
            var resource = res.Resources.sort((x, y) => x.Sequence > y.Sequence);
            $._resSeq = resource.map(o => o.Id);
            $('#location-table').bootstrapTable('load', res.LocationTags);
            $('#so-table').bootstrapTable('load', res.SoSettings);
            $('#resource-table').bootstrapTable('load', resource);
            document.getElementById('name').value = $._res.Name;
        })
        .fail(err => {
            console.log(err);
        });
    }

    /**
     * when playout weight is click
     */
    const onEditClick = (e, row, value) => {
        console.log(row);
        if (e.find('input').length > 0)
            return;
        e.html(
            `<input class="form-control input-sm" style="display: inline-block; width:50px;" 
                    placeholder="比重" type="number" id="name_${row.Id}" value="${value}" data-id='${row.Id}'/>`
        );
        var ele = document.getElementById(`name_${row.Id}`);
        ele.focus();
        ele.addEventListener('focusout', function handler(e) {
            var _tb = $('#resource-table'),
                _id = e.target.getAttribute('data-id');
            e.target.removeEventListener('focusout', handler);
            _tb.bootstrapTable('updateByUniqueId', { 
                id: _id,
                row: { ..._tb.bootstrapTable('getRowByUniqueId', _id), PlayoutWeight: e.target.value }
            });
        });
        ele.addEventListener('keypress', function keyHandler(e) {
            if (e.keyCode == 13) {
                e.target.removeEventListener('keypress', keyHandler);
                e.target.dispatchEvent(new Event('focusout'))
            }
        });
    }

    /**
     * Switch view between edit and view panel
     */
    $.fn.switchView = (e) => {
        var id = 'null';
        const edit = document.getElementById('editPanel'),
              view = document.getElementById('viewPanel'),
              editBody = document.getElementById('editPanelBody');

        if (e instanceof HTMLElement) {
            id = e.getAttribute('data-id') || 'null';
            requestData(id);
            editBody.style.display = '';
        } else {
            editBody.style.display = 'none';
        }
        
        if (edit.style.display === '') {
            edit.setAttribute('data-id', null);
            edit.style.display = 'none';
            view.style.display = '';
        } else {
            edit.setAttribute('data-id', id);
            edit.style.display = '';
            view.style.display = 'none';
        }
        // a workaround to make scroll bar always visible
        window.dispatchEvent(new Event('resize'));
    }

    /**
     * query for ad events
     * @param {event} e 
     */
    $.fn.query = (e) => {
        const _data = {
            so: $('#soSelect').val(),
            location: $('#locationSelect').val()
        };
        if (_data.so.length === 0 && _data.location.length === 0) {
            $('#table').bootstrapTable('refresh');
            return;
        }
        $.ajax({
            url: '/api/v1/ad/events/q',
            method: 'POST',
            data: _data
        })
        .done(res => {
            $('#table').bootstrapTable('load', res);
        })
        .fail(err => {
            var msg = document.getElementById('msgBox');
            msg.style.display = 'block';
            msg.innerHTML = "查詢失敗!";
        });
    }

    /**
     * when add new row button is click
     */
    $.fn.addRow = (e) => {
        var _opt,
            _data,
            _select = $("#editSelect"),
            node = e.target.tagName === 'SPAN' ? e.target.parentElement : e.target,
            btn = document.getElementById('editSubmit');

        if (node.id === 'newLocBtn') {
            _opt = $._loc;
            _data = $._res.LocationTags.map(s => s.Id);
            btn.setAttribute('data-trigger', 'loc')
        } else if (node.id === 'newSoBtn') {
            _opt = $._so;
            _data = $._res.SoSettings.map(s => s.Id);
            btn.setAttribute('data-trigger', 'so')
        } else
            return;

        _select.reloadSelect2(_opt);
        _select.val(_data).trigger('change');
    }

    /**
     * when update row button is click
     * @param {event} e
     */
    $.fn.updateRow = (e) => {
        if (!(e.target instanceof HTMLElement))
            return;
        
        var target = e.target.getAttribute('data-trigger'),
            _data,
            _newData,
            _type;
        
        if (target === 'loc') {
            _data = $._res.LocationTags.map(o => o.Id);
            _type = 'location';
        } else if (target === 'so') {
            _data = $._res.SoSettings.map(o => o.Id);
            _type = 'so';
        } else 
            return;
        
        $.ajax({
            url: '/api/v1/ad/events/',
            method: 'PUT',
            data: {
                ...getDiff(_data, $('#editSelect').val()),
                type: _type,
                id: document.getElementById('editPanel').getAttribute('data-id')
            }
        })
        .done(res => {
            $._res = res;
            $('#location-table').bootstrapTable('load', res.LocationTags);
            $('#so-table').bootstrapTable('load', res.SoSettings);
            $("#editModal").modal('toggle');
        })
        .fail(err => {
            // error msg goes here
        });
    }

    /**
     * when delete row button is click
     * @param {HTMLElement} e
     */
    $.fn.remove = (e) => {
        if (!(e instanceof HTMLElement))
            return;
        var id = e.getAttribute('data-id'),
            type = e.getAttribute('data-t');
        
        if (type === 'res') {
            let pos = $._resSeq.indexOf(id);
            $._resSeq = $._resSeq.splice(pos, 1);
            $('#resource-table').bootstrapTable('removeByUniqueId', id);
            return;
        }
        $.ajax({
            url: '/api/v1/ad/events/',
            method: 'PUT',
            data: {
                rm: [id],
                type: type === 'so' ? 'so' : 'location',
                id: document.getElementById('editPanel').getAttribute('data-id')
            }
        })
        .done(res => {
            $._res = res;
            $('#location-table').bootstrapTable('load', res.LocationTags);
            $('#so-table').bootstrapTable('load', res.SoSettings);
        })
        .fail(err => {
            // err msg goes here
        });
    }

    /**
     * adjust resource sequence 
     * @param {HTMLElement} e
     */
    $.fn.moveSeq = (e) => {
        if (!(e instanceof HTMLElement))
            return;
        var type = e.getAttribute('data-t'),
            id = parseInt(e.getAttribute('data-id')),
            pos = $._resSeq.indexOf(id),
            _tb = $('#resource-table'),
            _row = _tb.bootstrapTable('getRowByUniqueId', id);

        if (type === 'up') {
            if (pos < 1)
                return;
            $._resSeq = swap($._resSeq, pos, pos - 1);
            _tb.bootstrapTable('removeByUniqueId', id).bootstrapTable('insertRow', {
                index: pos - 1,
                row: _row
            });
        } else if (type === 'down') {
            if (pos >= $._resSeq.length - 1)
                return;
            $._resSeq = swap($._resSeq, pos, pos + 1);
            _tb.bootstrapTable('removeByUniqueId', id).bootstrapTable('insertRow', {
                index: pos + 1,
                row: _row
            });
        }
    }

    // add event listener to go back button
    document.getElementById('goBack').addEventListener('click', $.fn.switchView);
    // add listener to query button
    document.getElementById('queryBtn').addEventListener('click', $.fn.query);
    // add listener when add new row button is click
    document.getElementById('newLocBtn').addEventListener('click', $.fn.addRow);
    document.getElementById('newSoBtn').addEventListener('click', $.fn.addRow);
    // add listener when update new row button is click
    document.getElementById('editSubmit').addEventListener('click', $.fn.updateRow);
    // hide column when playout method change
    document.getElementById('playoutMethod').addEventListener('change', (e) => {
        if (e.target.value === 'taketurn')
            $('#resource-table').bootstrapTable('hideColumn', 'PlayoutWeight');
        else if (e.target.value === 'random')
            $('#resource-table').bootstrapTable('showColumn', 'PlayoutWeight');
    });
    // create new event
    document.getElementById('newEventBtn').addEventListener('click', (e) => {
        document.getElementById('name').value = '';
        $.fn.switchView(null);
    });
    // on save event
    document.getElementById('save').addEventListener('click', (e) => {
        if (isEmpty(name))
            return;
        const name = document.getElementById('name').value,
              edit = document.getElementById('editPanel'),
              id = edit.getAttribute('data-id');
        // if new event, no data-id in htmldoc, so set id = -1
        $.ajax({
            method: 'PUT',
            url: `/api/v1/ad/events/info`,
            data: {
                Id: id === 'null' ? -1 : id,
                Name: name
            } 
        })
        .done(res => {
            console.log(res);
            edit.setAttribute('data-id', res.Id);
        })
        .fail(err => {
            console.log(err);
        });
    });
    // a workaround for close modal
    $('#editModal').on('hidden.bs.modal', function () {
        document.body.removeChild(document.querySelector('div.modal-backdrop.fade'));
    });
});