// import $ from 'jquery';
import 'bootstrap-table';
import 'bootstrap-table/dist/bootstrap-table.css';
import 'select2';
import 'select2/dist/css/select2.css';
import 'bootstrap-datepicker/js/bootstrap-datepicker.js';
import 'bootstrap-datepicker/dist/css/bootstrap-datepicker.css';
import './css/events.css';
import { setTableViewZhTwLocal, isEmpty, tableSetting, getDateTimeString, swap, addMsgbox, getDateString } from './js/utils';

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
            formatter: (value, row, index, field) => `<div>
                                                          <button class='btn btn-sm btn-default' data-id="${row.Id}" onClick='$.fn.switchView(this);'>
                                                              <span class="glyphicon glyphicon-pencil"></span>
                                                          </button>
                                                          <button class='btn btn-sm btn-default' data-id="${row.Id}" onClick='$.fn.addCalendar(this);'>
                                                              <span class='glyphicon glyphicon-calendar'></span>
                                                          </button>
                                                          <button class='btn btn-sm btn-default' data-id="${row.Id}" data-t='eve' onClick='$.fn.remove(this);'>
                                                              <span class="glyphicon glyphicon-remove"></span>
                                                          </button>
                                                      </div>`
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
        onLoadSuccess: () => {
            window.dispatchEvent(new Event('resize'));
        },
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
            field: 'PlayoutWeight',
            title: '比重',
            formatter: (value, row, index, field) => value > 0 ? value : null
        }, {
            field: 'Sort/Action/Remove',
            title: '排序/動作/刪除',
            formatter: (value, row, index, field) => 
                `<div>
                    <button class='btn btn-sm btn-default' data-id="${row.Id}" data-seq="${index}" data-t='up' onClick='$.fn.moveSeq(this);'>
                        <span class="glyphicon glyphicon-arrow-up"></span>
                    </button>
                    <button class='btn btn-sm btn-default' data-id="${row.Id}" data-seq="${index}" data-t='down' onClick='$.fn.moveSeq(this);'>
                        <span class="glyphicon glyphicon-arrow-down"></span>
                    </button>
                    <button class='btn btn-sm btn-default' data-id="${row.Id}" data-seq="${index}" onClick='$.fn.editAction(this);'>
                        <span class="glyphicon glyphicon-tag"></span>
                    </button>
                    <button class='btn btn-sm btn-default' data-id="${row.Id}" data-seq="${index}" data-t='res' onClick='$.fn.remove(this);'>
                        <span class="glyphicon glyphicon-remove"></span>
                    </button>
                </div>`
        }]
    });
    // resource selection table
    $('#res-select-table').bootstrapTable({
        ...tableSetting,
        url: '/api/v1/ad/resource/table',
        sidePagination: 'server',
        showRefresh: false,
        pageSize: 5,
        pageList: [5],
        columns: [{
            field: 'Checked',
            title: '選擇',
            checkbox: true
        }, {
            field: 'ThumbnailPath',
            title: '預覽',
            formatter: (value, row, index, field) => 
                       `<div class='preview-img table-thumbnail' style='height:"5vh";background: url("/assets?p=${value}") no-repeat;'/>` 
        }, {
            field: 'MediaType',
            title: '類型',
        }, {
            field: 'Name',
            title: '名稱',
        }, {
            field: 'CreateTime',
            title: '創建日期',
            formatter: (value) => getDateString(new Date(value))
        }]
    });

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
    // initialize date picker
    $('#datepicker').datepicker({
        format: 'yyyy-mm-dd',
        autoclose: true,
        startDate: '+1d'
    });

    // hide playoutweight by default
    resTable.bootstrapTable('hideColumn', 'PlayoutWeight');

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
            $._res = {...res, Resources: res.Resources.sort((x, y) => x.Sequence > y.Sequence)};
            $('#location-table').bootstrapTable('load', res.LocationTags);
            $('#so-table').bootstrapTable('load', res.SoSettings);
            $('#resource-table').bootstrapTable('load', $._res.Resources);
            // set value
            document.getElementById('name').value = $._res.Name;
            document.getElementById('playoutMethod').value = res.PlayOutMethod || 'taketurn';
            document.getElementById('playoutSeq').value = res.PlayOutSequence || 'byasset';
            document.getElementById('playoutSec').value = res.PlayOutTimeSpan;
            // show column
            document.getElementById('playoutMethod').dispatchEvent(new Event('change'));
        })
        .fail(err => {
            console.log(err);
        });
    }

    /**
     * when playout weight is click
     */
    const onEditClick = (e, row, value) => {
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
     * toggle calendar modal
     */
    $.fn.addCalendar = (e) => {
        $('#calendarModal').modal('toggle');
        document.getElementById('calendarUpdate').setAttribute('data-id', e.getAttribute('data-id'));
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
        
        if (type === 'eve') {
            $.ajax({
                url: `/api/v1/ad/events/rm/${id}`,
                method: 'DELETE',
            })
            .then(res => {
                addMsgbox("刪除成功!", null, "event-list-panel", "success");
                $('#table').bootstrapTable('load', res); 
            })
            .catch(res => {
                addMsgbox("刪除失敗!", null, "event-list-panel", "danger");
            });
            return;
        }
        if (type === 'res') {
            var seq = parseInt(e.getAttribute('data-seq'));
            $('#resource-table').bootstrapTable('remove', { 
                field: 'Sequence', 
                values: [ seq ]
            });
            $._res.Resources.map((obj, index) => {
                obj.Sequence = index
            });
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
            index = parseInt(e.getAttribute('data-seq')),
            _tb = $('#resource-table'),
            _row = _tb.bootstrapTable('getData', false)[index];

        if (type === 'up') {
            if (index === 0)
                return;
            _tb.bootstrapTable('remove', {
                field: 'Sequence', 
                values: [ index ]
            }).bootstrapTable('insertRow', {
                index: index - 1,
                row: _row
            });
        } else if (type === 'down') {
            if (index === $._res.Resources.length - 1)
                return;
            _tb.bootstrapTable('remove', {
                field: 'Sequence', 
                values: [ index ]
            }).bootstrapTable('insertRow', {
                index: index + 1,
                row: _row
            });
        }
        $._res.Resources.map((obj, index) => {
            obj.Sequence = index;
        });
    }

    /**
     * edit resource action 
     * @param {HTMLElement} e
     */
    $.fn.editAction = (e) => {
        var eventId = document.getElementById('editPanel').getAttribute('data-id'),
            resourceSeq = e.getAttribute('data-seq');

        document.getElementById('actionEditSubmit').setAttribute('data-seq', resourceSeq);
        $.ajax({
            url: '/api/v1/ad/events/res/action', 
            method: 'POST',
            data: {
                e: eventId,
                r: resourceSeq
            }
        })
        .done(res => {
            $('#actionModal').modal('toggle');
            if (!Array.isArray(res))
                throw new Error();
            res.map(obj => {
                var enable = document.forms.action[`${obj.Color}Enable`], 
                    type = document.forms.action[`${obj.Color}Type`],
                    action = document.forms.action[`${obj.Color}Action`],
                    param = document.forms.action[`${obj.Color}Param`];
                $(`form#action input[name=${obj.Color}Enable]`).prop('checked', obj.Checked == 1);
                // obj.Checked == 1 ? enable.setAttribute('checked', 'checked') : enable.removeAttribute('checked');
                type.value = obj.Type;
                action.value = obj.Action;
                param.value = obj.Parameter;
            });
        })
        .fail(err => {
            addMsgbox("獲取資料失敗!", "", "res-panel", "danger");
        });
    }

    const onActionSave = (e) => {
        var data = $('#action').serializeArray(),
            index = e.target.getAttribute('data-seq'),
            _t = { redEnable: null, okEnable: null, blueEnable: null, yellowEnable: null, greenEnable: null };
        
        data.map(obj => {
            if (obj.name.indexOf('Enable') !== -1)
                _t[obj.name] = 1;
            else
                _t[obj.name] = obj.value;
        });

        $('#resource-table').bootstrapTable('updateRow', { 
            index: index,
            row: {
                Actions: _t
            } 
        });
        $('#actionModal').modal('toggle');
    }

    const onAddSchedule = (e) => {
        const eventId = e.target.getAttribute('data-id'),
              picker = $('#datepicker').data('datepicker');

        if (isEmpty(eventId) || !picker) {
            addMsgbox('系統錯誤!', '請嘗試重新整理', 'panel-body-msg', 'danger');
            return;
        }

        if (picker.dates.length == 0) {
            addMsgbox('尚未選擇時間', null, 'panel-body-msg', 'warning');
            return;
        }

        $.ajax({
            url: '/api/v1/schedule',
            method: 'POST',
            data: {
                eventId: eventId,
                s: getDateString(picker.dates[0]),
                e: getDateString(picker.dates[1])
            }
        })
        .done(res => {
            addMsgbox('成功加入排程!', '<a href="/DashBoard/Publish">點此查看</a>', 'panel-body-msg', 'success');
        })
        .fail(err => {
            addMsgbox('加入排程失敗!', null, 'panel-body-msg', 'danger');
        });
    }

    const onEventInfoSave = (e) => {
        const name = document.getElementById('name').value,
              edit = document.getElementById('editPanel'),
              id = edit.getAttribute('data-id');
        if (isEmpty(name))
              return;
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
            edit.setAttribute('data-id', res.Id);
            if (id === 'null') {
                // after successfully create new event with name, show edit panel body
                var editBody = document.getElementById('editPanelBody');
                editBody.style.display = '';
                $._res = {
                    Id: res.Id,
                    LocationTags: [],
                    Name: name,
                    PlayOutMethod: 'taketurn',
                    PlayOutSequence: 'byasset',
                    PlayOutTimeSpan: 0,
                    Resources: [],
                    SoSettings: []
                };
                $('#location-table').bootstrapTable('load', $._res.LocationTags);
                $('#so-table').bootstrapTable('load', $._res.SoSettings);
                $('#resource-table').bootstrapTable('load', $._res.Resources);
            } else {
                $._res =  { ...$._res, Name: name };
            }
            addMsgbox("成功儲存!", null, "info-panel");
        })
        .fail(err => {
            console.log(err);
            addMsgbox("儲存失敗!", "", "info-panel", "danger");
        });
    };

    /**
     * transform serialize input to val object
     * @param {Object} val 
     */
    const actionMapTransform = (val) => {
        var color = ['red', 'green', 'yellow', 'blue', 'ok'],
            actions = [];
        if (val === null)
            return;

        color.map((c, i) => {
            actions.push({
                color: color[i],
                type: val[`${c}Type`],
                action: val[`${c}Action`],
                parameter: val[`${c}Param`],
                checked: val[`${c}Enable`]
            });
        });
        return actions;
    };

    const onResourcesSave = (e) => {
        var _tb = $('#resource-table'),
            _data = _tb.bootstrapTable('getData'),
            data = JSON.parse(JSON.stringify(_data));
        data.map(o => {
            o.Actions = o.Actions ? actionMapTransform(o.Actions) : null;
        });

        $.ajax({
            url: '/api/v1/ad/events/res',
            method: 'PUT', 
            data: {
                id: $._res.Id,
                resources: data,
                playOutMethod: document.getElementById('playoutMethod').value,
                playOutSequence: document.getElementById('playoutSeq').value,
                playOutTimeSpan: document.getElementById('playoutSec').value
            }
        })
        .done(res => {
            addMsgbox("更新成功", "", "res-panel", "success");
            console.log(res);
        })
        .fail(err => {
            console.log(err);
            addMsgbox("更新失敗", "", "res-panel", "danger");
        });
    };

    const onResourcesChange = (e) => {
        var tb = $('#res-select-table'),
            dataTb = $('#resource-table');
        tb.bootstrapTable('getSelections').map(obj => {
            dataTb.bootstrapTable('append', obj);
        });
        $._res.Resources = dataTb.bootstrapTable('getData', false);
        $._res.Resources.map((obj, index) => {
            obj.Sequence = index;
        });

        $('#resourceModal').modal('toggle');
    }

    // add event listener to go back button
    document.getElementById('goBack').addEventListener('click', (e) => {
        $.fn.switchView();
        $('#table').bootstrapTable('refresh');
    });
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

    // on save event info
    document.getElementById('save').addEventListener('click', onEventInfoSave);
    // a workaround for close modal
    $('#editModal,#calendarModal').on('hidden.bs.modal', function () {
        document.body.removeChild(document.querySelector('div.modal-backdrop.fade'));
    });
    $('#resourceModal').on('hidden.bs.modal', function () {
        document.body.removeChild(document.querySelector('div.modal-backdrop.fade'));
        $('#res-select-table').bootstrapTable('uncheckAll');
    });
    $('#actionModal').on('hidden.bs.modal', function () {
        document.body.removeChild(document.querySelector('div.modal-backdrop.fade'));
        $('form#action input[type=text]').each(function(){$(this).val('')});
        $('form#action select').each(function(){$(this).val('image')});
        $('form#action input[type=checkbox]').each(function(){$(this).prop('checked', false)});
    });

    // add new resource
    document.getElementById('resEditSubmit').addEventListener('click', onResourcesChange);

    // save resources
    document.getElementById('saveRes').addEventListener('click', onResourcesSave);

    // save actions
    document.getElementById('actionEditSubmit').addEventListener('click', onActionSave);

    // add schedule task
    document.getElementById('calendarUpdate').addEventListener('click', onAddSchedule);
});